using StoryTeller.Data.Entities.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.AI.RequestModel
{
    public class AIImageGenerationRequest
    {
        public ChatProviderType Provider { get; set; }
        public string Prompt { get; set; }
        public int? Width { get; set; } = 512;
        public int? Height { get; set; } = 512;
        public int Count { get; set; } = 1;
    }
}
