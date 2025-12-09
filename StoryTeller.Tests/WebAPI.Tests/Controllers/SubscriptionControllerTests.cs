using AutoFixture;
using Domain.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using StoryTeller.API.Controllers;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Models;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.RequestModel;
using StoryTeller.Services.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Tests.Controllers
{
    public class SubscriptionControllerTests : SetupTest
    {
        private readonly SubscriptionController _subscriptionController;
        private readonly Mock<IUserContextService> _userContext = new Mock<IUserContextService>();
        public SubscriptionControllerTests()
        {
            _subscriptionController = new SubscriptionController(_subscriptionServiceMock.Object, _mapper, _userContext.Object);
        }

        [Fact]
        public async Task GetAllSubscription_ShouldReturnCorrectData()
        {
            // arrange
            var mocks = _fixture.Build<SubscriptionDTO>().CreateMany(100).ToList();

            _subscriptionServiceMock.Setup(x => x.GetAllByAdminAsync()).ReturnsAsync(mocks);

            // act
            var result = await _subscriptionController.GetAll();

            // assert
            _subscriptionServiceMock.Verify(x => x.GetAllByAdminAsync(), Times.Once());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(APIResponse<List<SubscriptionDTO>>.SuccessResponse(mocks));
        }

        [Fact]
        public async Task GetAllActiveSubscription_ShouldReturnCorrectData()
        {
            // arrange
            var mocks = _fixture.Build<SubscriptionDTO>().CreateMany(100).ToList();
            
            _subscriptionServiceMock.Setup(x => x.GetAllActiveAsync(null, default)).ReturnsAsync(mocks);

            // act
            var result = await _subscriptionController.GetAllActive();

            // assert
            _subscriptionServiceMock.Verify(x => x.GetAllActiveAsync(null, default), Times.Once());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(APIResponse<List<SubscriptionDTO>>.SuccessResponse(mocks));
        }

        [Fact]
        public async Task GetById_ShouldReturnCorrectData()
        {
            // arrange
            var mocks = _fixture.Build<SubscriptionDTO>().Create();

            _subscriptionServiceMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(mocks);

            // act
            var result = await _subscriptionController.GetById(Guid.NewGuid().ToString());

            // assert
            _subscriptionServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(APIResponse<SubscriptionDTO>.SuccessResponse(mocks));
        }

        [Fact]
        public async Task Create_ShouldReturnCorrectData()
        {
            // arrange
            var mocks = _fixture.Build<SubscriptionDTO>().Create();
            var userMocks = _fixture.Build<ApplicationUser>().Create();

            _subscriptionServiceMock.Setup(x => x.CreateAsync(It.IsAny<SubscriptionCreateRequest>(), It.IsAny<ApplicationUser>())).ReturnsAsync(mocks);
            _userContext.Setup(u => u.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userMocks);
            // act
            var result = await _subscriptionController.Create(It.IsAny<SubscriptionCreateRequest>());

            // assert
            _subscriptionServiceMock.Verify(x => x.CreateAsync(It.IsAny<SubscriptionCreateRequest>(), It.IsAny<ApplicationUser>()), Times.Once());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(APIResponse<SubscriptionDTO>.SuccessResponse(mocks, "Create subscription successful!"));
        }

        [Fact]
        public async Task Update_ShouldReturnCorrectData()
        {
            // arrange
            var mocks = _fixture.Build<SubscriptionDTO>().Create();
            var userMocks = _fixture.Build<ApplicationUser>().Create();

            _subscriptionServiceMock.Setup(x => x.UpdateAsync(It.IsAny<SubscriptionUpdateRequest>(), It.IsAny<ApplicationUser>())).ReturnsAsync(mocks);
            _userContext.Setup(u => u.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userMocks);
            // act
            var result = await _subscriptionController.Update(It.IsAny<SubscriptionUpdateRequest>());

            // assert
            _subscriptionServiceMock.Verify(x => x.UpdateAsync(It.IsAny<SubscriptionUpdateRequest>(), It.IsAny<ApplicationUser>()), Times.Once());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(APIResponse<SubscriptionDTO>.SuccessResponse(mocks, "Update subscription successful!"));
        }

        [Fact]
        public async Task Delete_ShouldReturnCorrectData()
        {
            // arrange
            var userMocks = _fixture.Build<ApplicationUser>().Create();

            _subscriptionServiceMock.Setup(x => x.SoftDelete(It.IsAny<Guid>(), It.IsAny<ApplicationUser>())).ReturnsAsync(true);
            _userContext.Setup(u => u.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userMocks);
            // act
            var result = await _subscriptionController.Delete(Guid.NewGuid().ToString());

            // assert
            _subscriptionServiceMock.Verify(x => x.SoftDelete(It.IsAny<Guid>(), It.IsAny<ApplicationUser>()), Times.Once());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(APIResponse<SubscriptionDTO>.SuccessResponse(true, "Delete subscription successful!"));
        }

        [Fact]
        public async Task SubscriptionDashboard_ShouldReturnCorrectData()
        {
            // arrange
            var mocks = _fixture.Build<SubscriptionDashboardResponse>().Create();

            _subscriptionServiceMock.Setup(x => x.SubscriptionDashboard()).ReturnsAsync(mocks);
            // act
            var result = await _subscriptionController.SubscriptionDashboard();

            // assert
            _subscriptionServiceMock.Verify(x => x.SubscriptionDashboard(), Times.Once());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(APIResponse<SubscriptionDashboardResponse>.SuccessResponse(mocks));
        }

        [Fact]
        public async Task RedeemSubscriptionWithPoints_ShouldReturnCorrectData()
        {
            // arrange
            var mocks = _fixture.Build<SubscriptionDTO>().Create();
            var userMocks = _fixture.Build<ApplicationUser>().Create();

            _subscriptionServiceMock.Setup(x => x.RedeemWithPointsAsync(It.IsAny<Guid>(), It.IsAny<ApplicationUser>(), default)).ReturnsAsync(true);
            _userContext.Setup(u => u.GetCurrentUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(userMocks);
            // act
            var result = await _subscriptionController.RedeemSubscriptionWithPoints(Guid.NewGuid(), default);

            // assert
            _subscriptionServiceMock.Verify(x => x.RedeemWithPointsAsync(It.IsAny<Guid>(), It.IsAny<ApplicationUser>(), default), Times.Once());

            var okResult = Assert.IsType<OkObjectResult>(result);
            okResult.Value.Should().BeEquivalentTo(APIResponse<SubscriptionDashboardResponse>.SuccessResponse("Subscription redeemed with points."));
        }
    }
}
