using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Repositories.Interfaces;
using StoryTeller.Services.AutoMapperMapping;
using StoryTeller.Services.Interfaces;

namespace Domain.Tests
{
    public class SetupTest : IDisposable
    {
        protected readonly IMapper _mapper;
        protected readonly Fixture _fixture;
        protected readonly Mock<IUnitOfWork> _unitOfWorkMock;
        protected readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        protected readonly StoryTellerContext _dbContext;

        //RepositoryMock
        protected readonly Mock<IUserRepository> _userRepositoryMock;
        protected readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
        protected readonly Mock<IStoryRepository> _storyRepositoryMock;
        protected readonly Mock<IUserSubscriptionRepository> _userSubscriptionRepositoryMock;

        //ServiceMock
        protected readonly Mock<IUserService> _userServiceMock;
        protected readonly Mock<ISubscriptionService> _subscriptionServiceMock;
        protected readonly Mock<IStoryService> _storyServiceMock;
        protected readonly Mock<IUserWalletService> _userWalletService;

        public SetupTest()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });
            _mapper = mappingConfig.CreateMapper();
            _fixture = new Fixture();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();

            var options = new DbContextOptionsBuilder<StoryTellerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new StoryTellerContext(options);

            _userRepositoryMock = new Mock<IUserRepository>();
            _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
            _storyRepositoryMock = new Mock<IStoryRepository>();
            _userSubscriptionRepositoryMock = new Mock<IUserSubscriptionRepository>();

            _userServiceMock = new Mock<IUserService>();
            _subscriptionServiceMock = new Mock<ISubscriptionService>();
            _storyServiceMock = new Mock<IStoryService>();
            _userWalletService = new Mock<IUserWalletService>();

            _dateTimeProviderMock.Setup(x => x.GetSystemCurrentTime()).Returns(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TimeConstants.Vietnam)));
            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
