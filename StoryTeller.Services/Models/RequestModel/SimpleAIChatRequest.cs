using StoryTeller.Data.Entities.AI;
using StoryTeller.Services.Models.AI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class SimpleAIChatRequest
    {
        public ChatProviderType Provider { get; set; }
        public string Prompt { get; set; } = string.Empty;
    }
}
