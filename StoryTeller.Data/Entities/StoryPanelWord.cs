using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Entities
{
    public class StoryPanelWord : BaseEntity
    {
        public Guid StoryPanelId { get; set; }
        public StoryPanel StoryPanel { get; set; }

        public string Text { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
    }
}
