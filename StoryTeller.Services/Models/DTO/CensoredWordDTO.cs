using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class CensoredWordDTO
    {
        public Guid Id { get; set; }
        public string Word { get; set; } = null!;
        public bool IsWildcard { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserMinimalDTO? CreatedBy { get; set; }
    }
}
