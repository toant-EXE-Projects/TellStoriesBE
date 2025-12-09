using HotChocolate.Authorization;
using StoryTeller.Data.Constants;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Models.DTO;
using StoryTeller.Services.Models.ResponseModel;

namespace StoryTeller.API.GraphQL
{
    [ExtendObjectType(Name = "Query")]
    [Authorize(Policy = Policies.StaffOnly)]
    public class StoryQuery
    {
        //public async Task<List<StoryDTO>> GetStories(
        //    [Service] IStoryService storyService,
        //    int skip = 0,
        //    int take = 10)
        //{
        //    return await storyService.GetAll(skip, take);
        //}
    }
}
