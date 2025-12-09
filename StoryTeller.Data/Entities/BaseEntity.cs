using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public ApplicationUser? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ApplicationUser? UpdatedBy { get; set; }

        public DateTime? DeletionDate { get; set; }

        public ApplicationUser? DeleteBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
