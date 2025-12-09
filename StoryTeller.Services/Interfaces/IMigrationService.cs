namespace StoryTeller.Services.Interfaces
{
    public interface IMigrationService
    {
        public Task<int> MigrateAllUsersAsync(CancellationToken ct = default);
        public Task<bool> MigrateUserAsync(string userId, CancellationToken ct = default);
        public Task<int> MigrateStoryTypesAsync(int type, CancellationToken cancellationToken = default);
    }
}
