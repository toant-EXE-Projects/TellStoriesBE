using StoryTeller.Data.Entities;
using StoryTeller.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class StoryPublishCreateRequest
    {
        public Guid StoryId { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? RequestNotes { get; set; }
    }
}
