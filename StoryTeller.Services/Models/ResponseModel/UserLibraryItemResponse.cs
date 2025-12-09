using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.ResponseModel
{
    public class UserLibraryItemResponse
    {
        public Guid Id { get; set; }
        public Guid StoryId { get; set; }
        public Guid UserCollectionId { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
