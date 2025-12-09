using AutoFixture;
using Domain.Tests;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using StoryTeller.Data.Entities;

namespace Infrastructures.Tests
{
    public class StoryTellerContextTests : SetupTest, IDisposable
    {

        //Subscription
        [Fact]
        public async Task StoryTellerContext_SubscriptionsDbSetShouldReturnCorrectData()
        {

            var mockData = _fixture.Build<Subscription>().CreateMany(10).ToList();
            await _dbContext.Subscription.AddRangeAsync(mockData);

            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.Subscription.ToListAsync();
            result.Should().BeEquivalentTo(mockData);
        }

        [Fact]
        public async Task StoryTellerContext_SubscriptionsDbSetShouldReturnEmptyListWhenHavingNoData()
        {
            var result = await _dbContext.Subscription.ToListAsync();
            result.Should().BeEmpty();
        }

        //ApplicationUser
        [Fact]
        public async Task StoryTellerContext_ApplicationUsersDbSetShouldReturnCorrectData()
        {

            var mockData = _fixture.Build<ApplicationUser>().CreateMany(10).ToList();
            await _dbContext.Users.AddRangeAsync(mockData);

            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.Users.ToListAsync();
            result.Should().BeEquivalentTo(mockData);
        }

        [Fact]
        public async Task StoryTellerContext_ApplicationUsersDbSetShouldReturnEmptyListWhenHavingNoData()
        {
            var result = await _dbContext.Users.ToListAsync();
            result.Should().BeEmpty();
        }

        //Story
        [Fact]
        public async Task StoryTellerContext_StorysDbSetShouldReturnCorrectData()
        {

            var mockData = _fixture.Build<Story>().CreateMany(10).ToList();
            await _dbContext.Stories.AddRangeAsync(mockData);

            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.Stories.ToListAsync();
            result.Should().BeEquivalentTo(mockData);
        }

        [Fact]
        public async Task StoryTellerContext_StorysDbSetShouldReturnEmptyListWhenHavingNoData()
        {
            var result = await _dbContext.Stories.ToListAsync();
            result.Should().BeEmpty();
        }

        //UserSubscription
        [Fact]
        public async Task StoryTellerContext_UserSubscriptionsDbSetShouldReturnCorrectData()
        {

            var mockData = _fixture.Build<UserSubscription>().CreateMany(10).ToList();
            await _dbContext.UserSubscription.AddRangeAsync(mockData);

            await _dbContext.SaveChangesAsync();

            var result = await _dbContext.UserSubscription.ToListAsync();
            result.Should().BeEquivalentTo(mockData);
        }

        [Fact]
        public async Task StoryTellerContext_UserSubscriptionsDbSetShouldReturnEmptyListWhenHavingNoData()
        {
            var result = await _dbContext.UserSubscription.ToListAsync();
            result.Should().BeEmpty();
        }
    }
}
