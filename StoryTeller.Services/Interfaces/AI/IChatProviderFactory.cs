using StoryTeller.Data.Entities.AI;
using StoryTeller.Services.Models.AI;

namespace StoryTeller.Services.Interfaces.AI
{
    public interface IChatProviderFactory
    {
        IChatProvider GetProvider(ChatProviderType providerType);
    }
}
