using StoryTeller.Data.DBContext;
using StoryTeller.Data.Repositories.Interfaces;

namespace StoryTeller.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoryTellerContext _context;

        public IUserRepository Users { get; }
        public IStoryRepository Stories { get; }
        public IActivityLogRepository ActivityLogs { get; }
        public ISubscriptionRepository Subscriptions { get; }
        public IUserSubscriptionRepository UserSubscriptions { get; }
        public ITagRepository Tags { get; }
        public IStoryTagRepository StoryTags { get; }
        public IUserLibraryRepository UserLibraries { get; }
        public IUserLibraryItemRepository UserLibraryItems { get; }
        public ICommentRepository Comments { get; }
        public IStoryPublishRequestRepository StoryPublishRequests { get; }
        public ICensoredWordRepository CensoredWords { get; }
        public IIssueReportRepository IssueReports { get; }
        public IBillingRecordRepository BillingRecords { get; }
        public INotificationRepository Notifications { get; }
        public IUserWalletRepository UserWallets { get; }
        public ISystemConfigurationRepository SystemConfigurations { get; }
        public INotificationReadRepository NotificationReads { get; }
        public IUserWalletTransactionRepository UserWalletTransactions { get; }

        public UnitOfWork(
            StoryTellerContext context
            , IUserRepository userRepo
            , IStoryRepository storyRepo
            , IActivityLogRepository activityLogs
            , ISubscriptionRepository subscriptions
            , IUserSubscriptionRepository userSubscriptions
            , ITagRepository tags
            , IStoryTagRepository storytags
            , IUserLibraryRepository userLibraries
            , IUserLibraryItemRepository userLibraryItems
            , ICommentRepository comments
            , IStoryPublishRequestRepository storyPublishRequests
            , ICensoredWordRepository censoredWords
            , IIssueReportRepository issueReports
            , IBillingRecordRepository billingRecords
            , INotificationRepository notifications
            , IUserWalletRepository userWallets
            , ISystemConfigurationRepository systemConfigurations
            , INotificationReadRepository notificationReads
            , IUserWalletTransactionRepository userWalletTransactions

            )
        {
            _context = context;
            Users = userRepo;
            Stories = storyRepo;
            ActivityLogs = activityLogs;
            Subscriptions = subscriptions;
            UserSubscriptions = userSubscriptions;
            Tags = tags;
            StoryTags = storytags;
            UserLibraries = userLibraries;
            UserLibraryItems = userLibraryItems;
            Comments = comments;
            StoryPublishRequests = storyPublishRequests;
            CensoredWords = censoredWords;
            IssueReports = issueReports;
            BillingRecords = billingRecords;
            Notifications = notifications;
            UserWallets = userWallets;
            SystemConfigurations = systemConfigurations;
            NotificationReads = notificationReads;
            UserWalletTransactions = userWalletTransactions;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
