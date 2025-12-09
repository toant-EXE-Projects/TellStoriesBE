using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using StoryTeller.Data.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StoryTeller.Data.Base;

namespace StoryTeller.Services.Configurations
{
    public class TokenWithExpiry
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
    }

    public class CustomTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : class
    {
        private readonly TokenSettings _options;
        private readonly StoryTellerContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CustomTokenProvider(IConfiguration _config, StoryTellerContext dbContext, IDateTimeProvider dateTimeProvider)
        {
            _options = _config.GetSection("TokenOptions").Get<TokenSettings>() ?? new TokenSettings();
            _dbContext = dbContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
            => Task.FromResult(true);

        public async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var rawToken = GenerateSixCharToken();

            var data = new TokenWithExpiry
            {
                Token = rawToken,
                ExpiresAt = _dateTimeProvider.GetSystemCurrentTime().AddMinutes(_options.TokenLifespanMinutes)
            };

            var json = JsonSerializer.Serialize(data);

            await manager.RemoveAuthenticationTokenAsync(user, _options.Provider, purpose);
            await manager.SetAuthenticationTokenAsync(user, _options.Provider, purpose, json);

            return rawToken;
        }

        public async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            var stored = await manager.GetAuthenticationTokenAsync(user, _options.Provider, purpose);
            if (string.IsNullOrWhiteSpace(stored)) return false;

            TokenWithExpiry? data;
            try
            {
                data = JsonSerializer.Deserialize<TokenWithExpiry>(stored);
            }
            catch
            {
                return false;
            }

            var isValid = data?.Token == token && data?.ExpiresAt > _dateTimeProvider.GetSystemCurrentTime();

            return isValid;
        }

        private string GenerateSixCharToken()
        {
            var tokenChars = new char[_options.Length];
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[_options.Length];
            rng.GetBytes(bytes);

            for (int i = 0; i < _options.Length; i++)
            {
                tokenChars[i] = _options.Characters[bytes[i] % _options.Characters.Length];
            }

            return new string(tokenChars);
        }
    }
}
