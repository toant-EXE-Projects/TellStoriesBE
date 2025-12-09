using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class UserMinimalDTO
    {
        public string? Id { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
