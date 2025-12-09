using AutoFixture;
using AutoMapper;
using Domain.Tests;
using FluentAssertions;
using GreenDonut;
using Microsoft.Extensions.Configuration;
using Moq;
using StoryTeller.Data.Constants;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using StoryTeller.Data.Logger;
using StoryTeller.Services.Implementations;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class SubscriptionServiceTests : SetupTest
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionServiceTests()
        {
            _subscriptionService = new SubscriptionService(
                _unitOfWorkMock.Object,
                _mapper,
                _dateTimeProviderMock.Object,
                new Mock<IEmailService>().Object,
                _userWalletService.Object,
                new Mock<ILoggerService>().Object
            );
        }

        [Fact]
        public async Task GetSubscriptionAsync_ShouldReturnCorrentData()
        {
            //arrange
            var mocks = _fixture.Build<Subscription>().CreateMany(100).ToList();
            var expectedResult = _mapper.Map<List<SubscriptionDTO>>(mocks);

            _unitOfWorkMock.Setup(x => x.Subscriptions.GetAllActiveAsync(null, default)).ReturnsAsync(mocks);

            //act
            var result = await _subscriptionService.GetAllActiveAsync();

            //assert
            _unitOfWorkMock.Verify(x => x.Subscriptions.GetAllActiveAsync(null, default), Times.Once());
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetSubscriptionByIdAsync_ShouldReturnCorrentData()
        {
            //arrange
            var mocks = _fixture.Build<Subscription>().Create();
            var expectedResult = _mapper.Map<SubscriptionDTO>(mocks);

            _unitOfWorkMock.Setup(x => x.Subscriptions.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync(mocks);

            //act
            var result = await _subscriptionService.GetByIdAsync(It.IsAny<Guid>());

            //assert
            _unitOfWorkMock.Verify(x => x.Subscriptions.GetByIdAsync(It.IsAny<Guid>(), default), Times.Once());
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetUserActiveSubscriptionAsync_ShouldReturnCorrentData()
        {
            //arrange
            var mocks = _fixture.Build<UserSubscription>().Create();
            var expectedResult = mocks;

            _unitOfWorkMock.Setup(x => x.UserSubscriptions.GetUserActiveSubscription(It.IsAny<string>(), default)).ReturnsAsync(mocks);

            //act
            var result = await _subscriptionService.GetUserActiveSubscriptionAsync(It.IsAny<string>());

            //assert
            _unitOfWorkMock.Verify(x => x.UserSubscriptions.GetUserActiveSubscription(It.IsAny<string>(), default), Times.Once());
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetUserActiveSubscriptionDTOAsync_ShouldReturnCorrentData()
        {
            //arrange
            var mocks = _fixture.Build<UserSubscription>().Create();
            var expectedResult = _mapper.Map<UserSubscriptionDTO>(mocks);

            _unitOfWorkMock.Setup(x => x.UserSubscriptions.GetUserActiveSubscription(It.IsAny<string>(), default)).ReturnsAsync(mocks);

            //act
            var result = await _subscriptionService.GetUserActiveSubscriptionDTOAsync(It.IsAny<string>());

            //assert
            _unitOfWorkMock.Verify(x => x.UserSubscriptions.GetUserActiveSubscription(It.IsAny<string>(), default), Times.Once());
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task MarkExpiredSubscriptionsAsync_ShouldReturnCorrentData()
        {
            //arrange
            var mocks = _fixture.Build<UserSubscription>().CreateMany(10).ToList();

            _unitOfWorkMock
                .Setup(x => x.UserSubscriptions.FindAsync(It.IsAny<Expression<Func<UserSubscription, bool>>>(), default))
                .ReturnsAsync(mocks);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(mocks.Count);

            //act
            var result = await _subscriptionService.MarkExpiredSubscriptionsAsync();

            //assert
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
            result.Should().BePositive();
        }

        [Fact]
        public async Task UpcomingExpirationsAsync_ShouldReturnCorrentData()
        {
            //arrange
            var mocks = _fixture.Build<UserSubscription>().CreateMany(10).ToList();
            var expectedResult = _mapper.Map<List<UserSubscriptionDTO>>(mocks);

            _unitOfWorkMock
                .Setup(x => x.UserSubscriptions.FindAsync(It.IsAny<Expression<Func<UserSubscription, bool>>>(), default))
                .ReturnsAsync(mocks);

            //act
            var result = await _subscriptionService.UpcomingExpirationsAsync(It.IsAny<int>());

            //assert
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task SubscribeOrExtendAsync_ShouldReturnTrue()
        {
            //arrange
            var newSubMocks = _fixture.Build<Subscription>().Create();
            var currentSubMocks = _fixture.Build<Subscription>().Create();
            var userSubMocks = _fixture.Build<UserSubscription>().Create();
            var userMocks = _fixture.Build<ApplicationUser>().Create();

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(newSubMocks);

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.GetByIdAsync(userSubMocks.SubscriptionId, default))
                .ReturnsAsync(currentSubMocks);

            _unitOfWorkMock
                .Setup(x => x.UserSubscriptions.GetUserActiveSubscription(It.IsAny<string>(), default))
                .ReturnsAsync(userSubMocks);

            //act
            var result = await _subscriptionService.SubscribeOrExtendAsync(It.IsAny<Guid>(), userMocks);

            //assert
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RedeemWithPointsAsync_ShouldReturnTrue()
        {
            // arrange
            var subMocks = _fixture.Build<Subscription>()
                .With(s => s.IsDeleted, false)
                .With(s => s.IsActive, true)
                .With(s => s.PurchaseMethod, SubscriptionPurchaseMethod.PointsOnly)
                .With(s => s.PointsCost, 500) // must be > 0
                .Create();

            var walletMocks = _fixture.Build<UserWallet>()
                .Do(w => w.SetBalance(int.MaxValue))
                .Create();

            var userMocks = _fixture.Build<ApplicationUser>()
                .With(u => u.Id, Guid.NewGuid().ToString())
                .Create();

            var userSubMocks = _fixture.Build<UserSubscription>().Create();

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(subMocks);

            _unitOfWorkMock
                .Setup(x => x.UserSubscriptions.GetUserActiveSubscription(It.IsAny<string>(), default))
                .ReturnsAsync(userSubMocks);

            _userWalletService
                .Setup(x => x.GetOrCreateUserWallet(userMocks.Id, userMocks, default))
                .ReturnsAsync(walletMocks);

            // Act
            var result = await _subscriptionService.RedeemWithPointsAsync(subMocks.Id, userMocks);

            // Assert
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.AtLeastOnce());
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_ShouldCalledRepositoryMethod()
        {
            //arrange
            var subMocks = _fixture.Build<Subscription>().Create();
            var userMocks = _fixture.Build<ApplicationUser>().Create();

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.CreateAsync(It.IsAny<Subscription>(), It.IsAny<ApplicationUser>(), default))
                .Returns(Task.CompletedTask);

            //act
            var result = await _subscriptionService.CreateAsync(It.IsAny<SubscriptionCreateRequest>(), userMocks);

            //assert
            _unitOfWorkMock.Verify(x => x.Subscriptions.CreateAsync(It.IsAny<Subscription>(), It.IsAny<ApplicationUser>(), default), Times.Once());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once());
        }

        [Fact]
        public async Task Delete_ShouldCalledRepositoryMethod()
        {
            //arrange
            var subMocks = _fixture.Build<Subscription>().Create();
            var userMocks = _fixture.Build<ApplicationUser>().Create();

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(subMocks);

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.SoftRemove(It.IsAny<Subscription>(), It.IsAny<ApplicationUser>(), default))
                .Returns(Task.CompletedTask);

            //act
            var hardDeleteResult = await _subscriptionService.HardDelete(It.IsAny<Guid>());
            var softDeleteResult = await _subscriptionService.SoftDelete(It.IsAny<Guid>(), userMocks);

            //assert
            _unitOfWorkMock.Verify(x => x.Subscriptions.Remove(It.IsAny<Subscription>()), Times.Once());
            _unitOfWorkMock.Verify(x => x.Subscriptions.SoftRemove(It.IsAny<Subscription>(), It.IsAny<ApplicationUser>(), default), Times.Once());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Exactly(2));

            hardDeleteResult.Should().BeTrue();
            softDeleteResult.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_ShouldCalledRepositoryMethod()
        {
            //arrange
            var subMocks = _fixture.Build<Subscription>().Create();
            var userMocks = _fixture.Build<ApplicationUser>().Create();
            var expectedResult = _mapper.Map<SubscriptionDTO>(subMocks);
            var requestMocks = _mapper.Map<SubscriptionUpdateRequest>(subMocks);

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.GetByIdAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync(subMocks);
            
            _unitOfWorkMock
                .Setup(x => x.Subscriptions.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<ApplicationUser>(), default))
                .Returns(Task.CompletedTask);

            //act
            var result = await _subscriptionService.UpdateAsync(requestMocks, userMocks);

            //assert
            _unitOfWorkMock.Verify(x => x.Subscriptions.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<ApplicationUser>(), default), Times.Once());
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(default), Times.Once());

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task SubscriptionDashboard_ShouldReturnCurrentData()
        {
            //arrange
            var subMocks = _fixture.Build<Subscription>().CreateMany(100).ToList();
            var userMocks = _fixture.Build<ApplicationUser>().CreateMany(100).ToList();
            var userSubMocks = _fixture.Build<UserSubscription>()
                .With(us => us.SubscriptionId, subMocks[new Random().Next(99)].Id)
                .With(us => us.UserId, userMocks[new Random().Next(99)].Id)
                .CreateMany(100)
                .ToList();
            var billingRecMocks = _fixture.Build<BillingRecord>().CreateMany(100).ToList();

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.GetAllActiveAsync(It.IsAny<SubscriptionPurchaseMethod?>(), default))
                .ReturnsAsync(subMocks);

            _unitOfWorkMock
                .Setup(x => x.UserSubscriptions.GetAllAsync(default))
                .ReturnsAsync(userSubMocks);

            _unitOfWorkMock
                .Setup(x => x.Users.GetAllAsync())
                .ReturnsAsync(userMocks);

            _unitOfWorkMock
                .Setup(x => x.BillingRecords.GetAllAsync(default))
                .ReturnsAsync(billingRecMocks);

            //act
            var result = await _subscriptionService.SubscriptionDashboard();

            //assert
            result.Should().NotBeNull();
            result.Should().NotBeEquivalentTo(new SubscriptionDashboardResponse());
        }

        [Fact]
        public async Task SubscriptionDashboardGetSubscribers_ShouldReturnCorrentData()
        {
            //arrange
            var subMocks = _fixture.Build<Subscription>().CreateMany(100).ToList();
            var userMocks = _fixture.Build<ApplicationUser>().CreateMany(100).ToList();
            var userSubMocks = _fixture.Build<UserSubscription>()
                .With(us => us.SubscriptionId, subMocks[new Random().Next(99)].Id)
                .With(us => us.UserId, userMocks[new Random().Next(99)].Id)
                .With(us => us.Status, SubscriptionConstants.StatusActive)
                .With(us => us.IsDeleted, false)
                .CreateMany(100)
                .ToList();

            var index = It.IsAny<int>();

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.GetAllActiveAsync(It.IsAny<SubscriptionPurchaseMethod?>(), default))
                .ReturnsAsync(subMocks);

            _unitOfWorkMock
                .Setup(x => x.UserSubscriptions.GetAllAsync(default))
                .ReturnsAsync(userSubMocks);

            _unitOfWorkMock
                .Setup(x => x.Users.GetAllAsync())
                .ReturnsAsync(userMocks);

            _unitOfWorkMock
                .Setup(x => x.Subscriptions.GetByIdAsync(userSubMocks[index].SubscriptionId, default))
                .ReturnsAsync(subMocks.FirstOrDefault(s => s.Id == userSubMocks[index].SubscriptionId));

            _unitOfWorkMock
                .Setup(x => x.Users.GetByIdAsync(userSubMocks[index].UserId))
                .ReturnsAsync(userMocks.FirstOrDefault(s => s.Id == userSubMocks[index].UserId));

            //act
            var subscribers = await _subscriptionService.SubscriptionDashboardGetSubscribers();
            var newSubscribers = await _subscriptionService.SubscriptionDashboardGetNewSubscribers();
            var quitSubscribers = await _subscriptionService.SubscriptionDashboardGetQuitSubscribers();
            var subscribersBySubscription = await _subscriptionService.SubscriptionDashboardGetSubscribersBySubscription();
            var recentSubscribers = await _subscriptionService.SubscriptionDashboardGetRecentSubscribers(It.IsAny<int>());

            //assert
            subscribers.Should().NotBeNull();
            newSubscribers.Should().NotBeNull();
            quitSubscribers.Should().NotBeNull();
            subscribersBySubscription.Should().NotBeNull();
            recentSubscribers.Should().NotBeNull();
        }
    }
}
