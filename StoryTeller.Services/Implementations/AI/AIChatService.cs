using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Mscc.GenerativeAI;
using StoryTeller.Data.Constants;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Entities.AI;
using StoryTeller.Services.Interfaces.AI;
using StoryTeller.Services.Models.AI.OptionsModel;
using StoryTeller.Services.Models.AI.RequestModel;
using StoryTeller.Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Implementations.AI
{
    public class AIChatService
    {
        private readonly IChatProviderFactory _providerFactory;
        private readonly StoryTellerContext _dbContext;
        private readonly IConfiguration _config;

        public AIChatService(IChatProviderFactory providerFactory, StoryTellerContext dbContext, IConfiguration config)
        {
            _providerFactory = providerFactory;
            _dbContext = dbContext;
            _config = config;
        }
        private string GetDefaultSystemInstruction()
        {
            return AISystemInstruction.DefaultSystemInstruction
                   ?? "You are a helpful assistant.";
        }
        public async Task<string> ChatAsync(ChatProviderType providerType, string prompt, IEnumerable<ChatMessageDTO> history, ChatGenerationOptions? options = null, CancellationToken ct = default)
        {
            options ??= new ChatGenerationOptions();
            options.SystemInstruction ??= GetDefaultSystemInstruction();

            var result = await _providerFactory.GetProvider(providerType).GetResponseAsync(prompt, history, options, ct);

            //var sessionId = history.FirstOrDefault()?.ChatSessionId ?? Guid.NewGuid();
            //var session = await _dbContext.Set<ChatSession>().FindAsync(new object[] { sessionId }, ct) ?? new ChatSession { Id = sessionId, CreatedDate = DateTime.UtcNow, Messages = new() };

            //var newMessage = new ChatMessage
            //{
            //    Id = Guid.NewGuid(),
            //    Role = "assistant",
            //    Content = result,
            //    Order = session.Messages.Count + 1,
            //    ChatSessionId = session.Id,
            //    CreatedDate = DateTime.UtcNow
            //};

            //session.Messages.Add(newMessage);

            //_dbContext.Update(session);
            //await _dbContext.SaveChangesAsync(ct);

            return result;
        }

        public async Task<string> GenerateStoryAsync(ChatProviderType providerType, string prompt, IEnumerable<ChatMessageDTO> history, ChatGenerationOptions? options = null, CancellationToken ct = default)
        {
            options ??= new ChatGenerationOptions();
            options.SystemInstruction ??= GetDefaultSystemInstruction();

            var result = await _providerFactory.GetProvider(providerType).GetStoryResponseAsync(prompt, history, options, ct);

            //var sessionId = history.FirstOrDefault()?.ChatSessionId ?? Guid.NewGuid();
            //var session = await _dbContext.Set<ChatSession>().FindAsync(new object[] { sessionId }, ct) ?? new ChatSession { Id = sessionId, CreatedDate = DateTime.UtcNow, Messages = new() };

            //var newMessage = new ChatMessage
            //{
            //    Id = Guid.NewGuid(),
            //    Role = "assistant",
            //    Content = result,
            //    Order = session.Messages.Count + 1,
            //    ChatSessionId = session.Id,
            //    CreatedDate = DateTime.UtcNow
            //};

            //session.Messages.Add(newMessage);

            //_dbContext.Update(session);
            //await _dbContext.SaveChangesAsync(ct);

            return result;
        }

        public async IAsyncEnumerable<string> StreamChatAsync(ChatProviderType providerType, string prompt, IEnumerable<ChatMessageDTO> history, ChatGenerationOptions? options = null, [EnumeratorCancellation] CancellationToken ct = default)
        {
            options ??= new ChatGenerationOptions();
            options.SystemInstruction ??= GetDefaultSystemInstruction();

            var provider = _providerFactory.GetProvider(providerType);
            //var sessionId = history.FirstOrDefault()?.ChatSessionId ?? Guid.NewGuid();
            //var session = await _dbContext.Set<Data.Entities.AI.ChatSession>().FindAsync(new object[] { sessionId }, ct) ?? new Data.Entities.AI.ChatSession { Id = sessionId, CreatedDate = DateTime.UtcNow, Messages = new() };

            await foreach (var part in provider.StreamResponseAsync(prompt, history, options, ct))
            {
                if (!string.IsNullOrWhiteSpace(part))
                    yield return part;
                if (ct.IsCancellationRequested)
                    yield break;
            }

            //var complete = stringBuilder.ToString();

            //var message = new Data.Entities.AI.ChatMessage
            //{
            //    Id = Guid.NewGuid(),
            //    Role = "assistant",
            //    Content = complete,
            //    Order = session.Messages.Count + 1,
            //    ChatSessionId = session.Id,
            //    CreatedDate = DateTime.UtcNow
            //};

            //session.Messages.Add(message);
            //_dbContext.Update(session);
            //await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<List<Data.Entities.AI.ChatMessage>> GetChatHistoryAsync(ChatHistoryFilter filter, CancellationToken ct = default)
        {
            var query = _dbContext.Set<Data.Entities.AI.ChatMessage>().AsQueryable();
            query = query.Where(m => m.ChatSessionId == filter.SessionId);

            if (!filter.IncludeErrors)
                query = query.Where(m => !m.HasError);

            if (!string.IsNullOrWhiteSpace(filter.RoleFilter))
                query = query.Where(m => m.Role == filter.RoleFilter);

            return await query
                .OrderByDescending(m => m.CreatedDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(ct);
        }

        public async Task<List<object>> GenerateImages(ChatProviderType providerType, AIImageGenerationRequest request, CancellationToken ct = default)
        {
            var result = await _providerFactory.GetProvider(providerType).GenerateImagesAsync(request, ct);
            return result;
        }
    }
}
