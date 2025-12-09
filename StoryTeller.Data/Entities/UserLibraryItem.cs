using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class UserLibraryItem : BaseEntity
    {
        public Guid StoryId { get; set; }
        public Story Story { get; set; } = null!;

        public Guid UserCollectionId { get; set; }
        public UserLibrary UserLibrary { get; set; } = null!;

        public DateTime AddedAt { get; set; }
    }
}
