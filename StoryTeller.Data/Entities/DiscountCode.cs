using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoryTeller.Data.Entities
{
    public class DiscountCode : BaseEntity
    {
        [MaxLength(100)]
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(18,0)")]
        public decimal DiscountValue { get; set; }

        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
    }
}
