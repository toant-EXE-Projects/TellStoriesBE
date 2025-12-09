using Microsoft.Extensions.DependencyInjection;
using StoryTeller.Data.Entities.AI;
using StoryTeller.Services.Implementations.Providers;
using StoryTeller.Services.Interfaces.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Implementations.AI
{
    public class ChatProviderFactory : IChatProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ChatProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IChatProvider GetProvider(ChatProviderType providerType)
        {
            return providerType switch
            {
                //ChatProviderType.ChatGPT => _serviceProvider.GetRequiredService<OpenAiChatProvider>(),
                ChatProviderType.Gemini => _serviceProvider.GetRequiredService<GeminiChatProvider>(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
