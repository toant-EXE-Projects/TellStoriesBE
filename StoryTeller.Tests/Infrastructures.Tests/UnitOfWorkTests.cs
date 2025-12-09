using AutoFixture;
using Domain.Tests;
using FluentAssertions;
using Moq;
using StoryTeller.Data;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Repositories.Interfaces;

namespace Infrastructures.Tests
{
    public class UnitOfWorkTests : SetupTest
    {
        private readonly IUnitOfWork _uow;
        public UnitOfWorkTests()
        {
            _uow = new UnitOfWork(
                _dbContext,
                _userRepositoryMock.Object,
                _storyRepositoryMock.Object,
                new Mock<IActivityLogRepository>().Object,
                _subscriptionRepositoryMock.Object,
                _userSubscriptionRepositoryMock.Object,
                new Mock<ITagRepository>().Object,
                new Mock<IStoryTagRepository>().Object,
                new Mock<IUserLibraryRepository>().Object,
                new Mock<IUserLibraryItemRepository>().Object,
                new Mock<ICommentRepository>().Object,
                new Mock<IStoryPublishRequestRepository>().Object,
                new Mock<ICensoredWordRepository>().Object,
                new Mock<IIssueReportRepository>().Object,
                new Mock<IBillingRecordRepository>().Object,
                new Mock<INotificationRepository>().Object,
                new Mock<IUserWalletRepository>().Object,
                new Mock<ISystemConfigurationRepository>().Object,
                new Mock<INotificationReadRepository>().Object,
                new Mock<IUserWalletTransactionRepository>().Object
            );
        }

        [Fact]
        public async Task TestUnitOfWork()
        {
            // arrange
            var userMockData = _fixture.Build<ApplicationUser>().CreateMany(10).ToList();
            var subscriptionMockData = _fixture.Build<Subscription>().CreateMany(10).ToList();
            var storyMockData = _fixture.Build<Story>().CreateMany(10).ToList();
            var userSubscriptionMockData = _fixture.Build<UserSubscription>().CreateMany(10).ToList();

            _userRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(userMockData);
            _subscriptionRepositoryMock.Setup(sub => sub.GetAllAsync(default)).ReturnsAsync(subscriptionMockData);
            _storyRepositoryMock.Setup(s => s.GetAllAsync(default)).ReturnsAsync(storyMockData);
            _userSubscriptionRepositoryMock.Setup(us => us.GetAllAsync(default)).ReturnsAsync(userSubscriptionMockData);

            // act
            var users = await _uow.Users.GetAllAsync();
            var subscriptions = await _uow.Subscriptions.GetAllAsync();
            var storys = await _uow.Stories.GetAllAsync();
            var userSubscriptions = await _uow.UserSubscriptions.GetAllAsync();

            // assert
            users.Should().BeEquivalentTo(userMockData);
            subscriptions.Should().BeEquivalentTo(subscriptionMockData);
            storys.Should().BeEquivalentTo(storyMockData);
            userSubscriptions.Should().BeEquivalentTo(userSubscriptionMockData);

        }
    }
}
