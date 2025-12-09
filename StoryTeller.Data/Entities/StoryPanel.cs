using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class StoryPanel : BaseEntity
    {
        public Guid StoryId { get; set; }
        public Story Story { get; set; }

        public string? ImageUrl { get; set; }
        public string? AudioUrl { get; set; }
        public string? Content { get; set; }

        public bool IsEndPanel { get; set; }
        public string LanguageCode { get; set; }
        public int PanelNumber { get; set; }

        public ICollection<StoryPanelWord> StoryPanelWords { get; set; }
    }
}
