using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class StoryPanelDTO
    {
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? AudioUrl { get; set; }
        public bool? IsEndPanel { get; set; }
        public string? LanguageCode { get; set; }
        public int? PanelNumber { get; set; }
    }
}
