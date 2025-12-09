using StoryTeller.Data.Entities;
using StoryTeller.Services.Models.RequestModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class TagDTO
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
    }
}
