using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Entities.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.DBContext
{
    public class StoryTellerContext : IdentityDbContext<ApplicationUser>
    {
        public StoryTellerContext() { }
        public StoryTellerContext(DbContextOptions<StoryTellerContext> options)
            : base(options)
        {
        }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        //ApplicationUser
        //BaseEntity
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FeaturedStories> FeaturedStories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Story> Stories { get; set; }
        public DbSet<StoryPanel> StoryPanels { get; set; }
        public DbSet<StoryPanelWord> StoryPanelWords { get; set; }
        public DbSet<StoryTag> StoryTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UsageLog> UsageLogs { get; set; }
        public DbSet<UserLibrary> UserLibraries { get; set; }
        public DbSet<UserLibraryItem> UserLibraryItems { get; set; }
        public DbSet<Subscription> Subscription { get; set; }
        public DbSet<UserSubscription> UserSubscription { get; set; }
        public DbSet<StoryPublishRequest> StoryPublishRequests { get; set; }
        public DbSet<BillingRecord> BillingRecords { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<CensoredWord> CensoredWords { get; set; }
        public DbSet<IssueReport> IssueReports { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<SystemConfiguration> SystemConfigurations { get; set; }
        public DbSet<NotificationRead> NotificationReads { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // [*1] Global Query Filter for soft delete
            // Use .IgnoreQueryFilters() in LINQ to ignore this !IMPORTANT! - Duc
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var entity = builder.Entity(entityType.ClrType);

                    //entity.HasQueryFilter(ConvertFilterExpression<BaseEntity>(e => !e.IsDeleted, entityType.ClrType)); // <--- Exclude All IsDeleted Entries. Edit: cant do this anymore comments are threaded :/

                    entity.HasOne(typeof(ApplicationUser), nameof(BaseEntity.CreatedBy))
                          .WithMany().OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(typeof(ApplicationUser), nameof(BaseEntity.UpdatedBy))
                          .WithMany().OnDelete(DeleteBehavior.Restrict);

                    entity.HasOne(typeof(ApplicationUser), nameof(BaseEntity.DeleteBy))
                          .WithMany().OnDelete(DeleteBehavior.Restrict);
                }
            }

            // Fluent API
            //-------------Story-------------//
            builder.Entity<Story>()
                .HasMany(s => s.Panels)
                .WithOne(p => p.Story)
                .HasForeignKey(p => p.StoryId);

            builder.Entity<Story>()
                .HasMany(s => s.FeaturedStories)
                .WithOne(f => f.Story)
                .HasForeignKey(f => f.StoryId);

            builder.Entity<StoryPanel>()
                .HasMany(p => p.StoryPanelWords)
                .WithOne(w => w.StoryPanel)
                .HasForeignKey(w => w.StoryPanelId);
            //-------------StoryTag-------------//
            builder.Entity<StoryTag>()
                .HasKey(st => new { st.TagId, st.StoryId });

            builder.Entity<StoryTag>()
                .HasOne(st => st.Story)
                .WithMany(s => s.StoryTags)
                .HasForeignKey(st => st.StoryId);

            builder.Entity<StoryTag>()
                .HasOne(st => st.Tag)
                .WithMany(t => t.StoryTags)
                .HasForeignKey(st => st.TagId);

            builder.Entity<StoryTag>()
                .HasOne(st => st.User)
                .WithMany()
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            //-------------UserLibrary-------------//
            builder.Entity<UserLibrary>()
                .HasOne(ul => ul.User)
                .WithMany()
                .HasForeignKey(ul => ul.UserId);

            builder.Entity<UserLibraryItem>()
                .HasOne(uli => uli.Story)
                .WithMany()
                .HasForeignKey(uli => uli.StoryId);

            builder.Entity<UserLibraryItem>()
                .HasOne(uli => uli.UserLibrary)
                .WithMany(ul => ul.LibraryItems)
                .HasForeignKey(uli => uli.UserCollectionId);
            //-------------ActivityLog-------------//
            builder.Entity<ActivityLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId);
            //-------------Comment-------------//
            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Content)
                      .IsRequired()
                      .HasMaxLength(5000);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Story)
                      .WithMany(s => s.Comments)
                      .HasForeignKey(e => e.StoryId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ParentComment)
                      .WithMany(e => e.Replies)
                      .HasForeignKey(e => e.ReplyTo)
                      .OnDelete(DeleteBehavior.Restrict); // or SetNull
            });
            //-------------Rating-------------//
            builder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Story)
                      .WithMany()
                      .HasForeignKey(e => e.StoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            //-------------UsageLog-------------//
            builder.Entity<UsageLog>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            //-------------Notification-------------//
            builder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(n => n.Reads)
                      .WithOne(r => r.Notification)
                      .HasForeignKey(r => r.NotificationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Title)
                      .IsRequired();

                entity.Property(e => e.Message)
                      .IsRequired();

                entity.Property(e => e.Type)
                      .IsRequired();
            });
            //-------------Announcement-------------//
            builder.Entity<Announcement>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.CreatedBy)
                      .WithMany()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Title)
                      .IsRequired();

                entity.Property(e => e.Content)
                      .IsRequired();

                entity.Property(e => e.Type)
                      .IsRequired();
            });
            //-------------Subscription-------------//
            //builder.Entity<UserSubscription>(entity =>
            //{
            //    entity.HasOne(u => u.Subscription)
            //        .WithMany()
            //        .HasForeignKey(u => u.SubscriptionId);
            //    entity.HasOne(u => u.User)
            //        .WithMany()
            //        .HasForeignKey(u => u.UserId);
            //});
            builder.Entity<UserSubscription>()
                .HasOne(us => us.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Delete subscriptions when user is deleted

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ActiveSubscription)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.ActiveSubscriptionId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade path cycle

            //-------------ChatSession-------------//
            builder.Entity<ChatSession>(entity =>
            {
                entity.HasKey(cs => cs.Id);

                entity.HasOne(cs => cs.User)
                      .WithMany()
                      .HasForeignKey(cs => cs.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(cs => cs.Title).HasMaxLength(200);
                entity.Property(cs => cs.ProviderUsed).HasMaxLength(100);
                entity.Property(cs => cs.Language).HasMaxLength(50);
            });

            //-------------ChatMessage-------------//
            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(cm => cm.Id);

                entity.Property(cm => cm.Role)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(cm => cm.Content)
                      .IsRequired();

                entity.HasOne(cm => cm.ChatSession)
                      .WithMany(cs => cs.Messages)
                      .HasForeignKey(cm => cm.ChatSessionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            //-------------StoryPublishRequest-------------//
            builder.Entity<StoryPublishRequest>(entity =>
            {
                entity.ToTable("StoryPublishRequests");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.RequestNotes)
                      .HasMaxLength(1000);

                entity.Property(x => x.ReviewNotes)
                      .HasMaxLength(1000);

                entity.Property(x => x.CreatedDate)
                      .IsRequired();

                entity.HasOne(x => x.Story)
                      .WithMany()
                      .HasForeignKey(x => x.StoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            //-------------CensoredWord-------------//
            builder.Entity<CensoredWord>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Word)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.IsWildcard)
                      .HasDefaultValue(false);

                entity.HasIndex(e => e.Word).IsUnique(false);
            });
            //-------------IssueReport-------------//
            builder.Entity<IssueReport>()
                .HasOne(ir => ir.User)
                .WithMany()
                .HasForeignKey(ir => ir.UserId);
            //-------------UserWallet-------------//
            builder.Entity<UserWallet>(entity =>
            {
                entity.HasIndex(w => w.UserId).IsUnique(); // 1 wallet per user

                entity.Property(w => w.RowVersion).IsRowVersion();

                entity.HasOne(w => w.User)
                      .WithOne(u => u.Wallet)
                      .HasForeignKey<UserWallet>(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();
            });
            //-------------UserWalletTransaction-------------//

            // UserWalletTransaction -> UserWallet
            builder.Entity<UserWalletTransaction>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserWalletTransaction -> ApplicationUser (PerformedBy)
            builder.Entity<UserWalletTransaction>()
                .HasOne(t => t.PerformedByUser)
                .WithMany()
                .HasForeignKey(t => t.PerformedByUserId)
                .OnDelete(DeleteBehavior.Restrict);


            //-------------NotificationRead-------------//
            builder.Entity<NotificationRead>(entity =>
            {
                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Notification)
                      .WithMany(n => n.Reads)
                      .HasForeignKey(r => r.NotificationId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(r => r.ReadAt)
                      .IsRequired();
            });
        }

        // Dynamic filter expressions for [*1] - Duc
        private static LambdaExpression ConvertFilterExpression<TBase>(Expression<Func<TBase, bool>> filterExpression, Type entityType)
        {
            var parameter = Expression.Parameter(entityType, "e");
            var body = ReplacingExpressionVisitor.Replace(filterExpression.Parameters[0], parameter, filterExpression.Body);
            return Expression.Lambda(body, parameter);
        }
    }
}
