using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class UserLibraryDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<UserLibraryItemDTO> LibraryItems { get; set; } = new List<UserLibraryItemDTO>();
    }
}
