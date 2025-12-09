using StoryTeller.Data.Entities.AI;
using StoryTeller.Services.Models.AI.OptionsModel;
using StoryTeller.Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.AI.RequestModel
{
    public class AIChatRequest
    {
        public ChatProviderType Provider { get; set; }
        public string Prompt { get; set; } = string.Empty;
        public ChatGenerationOptions? Options { get; set; }
        public List<ChatMessageDTO> History { get; set; } = new();
    }
}
