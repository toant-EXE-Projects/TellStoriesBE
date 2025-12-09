using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.ResponseModel
{
    public class UserLibraryResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string UserId { get; set; } = null!;
        public List<UserLibraryItemResponse> Items { get; set; } = new();
    }
}
