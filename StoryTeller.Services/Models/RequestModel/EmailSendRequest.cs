using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class EmailSendRequest
    {
        public string Recipients { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string? ccRecipients { get; set; }
        public string? bccRecipients { get; set; }
    }
}
