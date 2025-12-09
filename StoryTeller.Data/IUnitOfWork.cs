using StoryTeller.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IUserRepository Users { get; }
        IStoryRepository Stories { get; }
        IActivityLogRepository ActivityLogs { get; }
        ISubscriptionRepository Subscriptions { get; }
        IUserSubscriptionRepository UserSubscriptions { get; }
        ITagRepository Tags { get; }
        IStoryTagRepository StoryTags { get; }
        IUserLibraryRepository UserLibraries { get; }
        IUserLibraryItemRepository UserLibraryItems { get; }
        ICommentRepository Comments { get; }
        IStoryPublishRequestRepository StoryPublishRequests { get; }
        ICensoredWordRepository CensoredWords { get; }
        IIssueReportRepository IssueReports { get; }
        IBillingRecordRepository BillingRecords { get; }
        INotificationRepository Notifications { get; }
        IUserWalletRepository UserWallets { get; }
        ISystemConfigurationRepository SystemConfigurations { get; }
        INotificationReadRepository NotificationReads { get; }
        IUserWalletTransactionRepository UserWalletTransactions { get; }


        Task<int> SaveChangesAsync(CancellationToken ct = default);

    }
}
