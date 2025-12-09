using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class UserLibraryItemDTO
    {
        public Guid Id { get; set; }
        public Guid UserCollectionId { get; set; }
        public StoryDTO Story { get; set; } = null!;
        public DateTime AddedAt { get; set; }
    }
}
