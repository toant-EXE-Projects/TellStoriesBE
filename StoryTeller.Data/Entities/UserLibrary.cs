using StoryTeller.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace StoryTeller.Data.Entities
{
    public class UserLibrary : BaseEntity
    {
        [StringLength(50, ErrorMessage = "Title name cannot exceed 50 characters.")]
        public string Title { get; set; } = null!;
        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public LibraryAccessType AccessType { get; set; } = LibraryAccessType.Private;
        public bool IsSystemDefined { get; set; } = false;
        public bool IsDeletable { get; set; } = true;

        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<UserLibraryItem> LibraryItems { get; set; } = new List<UserLibraryItem>();
    }
}
