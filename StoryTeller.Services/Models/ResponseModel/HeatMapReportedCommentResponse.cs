using StoryTeller.Services.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.ResponseModel
{
    public class HeatMapReportedCommentResponse
    {
        public UserMinimalDTO User { get; set; } = null!;
        public List<ReportedComment> ReportedComments { get; set; } = [];

    }

    public class ReportedComment
    {
        public CommentDTO Comment { get; set; } = null!;
        public int ReportCount { get; set; } = 0;
        //public List<UserMinimalDTO> ReportBy { get; set; } = [];
    }
}
