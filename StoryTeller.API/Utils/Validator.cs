using Microsoft.AspNetCore.Mvc;
using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.DTO;

namespace StoryTeller.API.Utils
{
    public class Validator
    {
        public IActionResult ValidateAccess<T>(T item, ApplicationUser user)
            where T : BaseEntity
        {
            if (item == null)
                return new NotFoundObjectResult("Item not found");

            if (item.CreatedBy == null)
                return new NotFoundObjectResult("Creator not found");

            if (item.CreatedBy.Id != user.Id)
                return new UnauthorizedObjectResult("You are not the owner of this item.");

            return null;
        }
    }
}
