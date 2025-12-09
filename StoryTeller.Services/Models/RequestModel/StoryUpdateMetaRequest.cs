using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class StoryUpdateMetaRequest
    {
        [Required]
        public Guid Id { get; set; }
        public StoryMeta? Meta { get; set; }
    }
}
