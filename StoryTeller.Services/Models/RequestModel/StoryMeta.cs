using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class StoryMeta
    {
        public bool? IsFeatured { get; set; } = false;
        public bool? IsCommunity { get; set; } = false;
        public bool? IsPublished { get; set; } = false;
    }
}
