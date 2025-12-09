using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class StoryPanelCreateRequest
    {
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public string? AudioUrl { get; set; }
        public bool IsEndPanel { get; set; }
        public string LanguageCode { get; set; }
        public int PanelNumber { get; set; }
    }
}
