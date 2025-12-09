using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class ChatMessageDTO
    {
        public string Role { get; set; } = "user";
        public string Content { get; set; } = string.Empty;
    }
}
