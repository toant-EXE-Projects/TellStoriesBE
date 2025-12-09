using StoryTeller.Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class CommentMeta
    {
        public bool IsHidden { get; set; }
        public bool IsEdited { get; set; }
        public bool IsFlagged { get; set; }
    }
}
