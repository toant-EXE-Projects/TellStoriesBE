using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.RequestModel
{
    public class UserQueryRequest
    {
        public string? SearchValue { get; set; }
        public string? FilterByRole { get; set; }
        public string? FilterByStatus { get; set; }
        public string? OrderBy { get; set; }
        public string? SortOrder { get; set; }
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 0;

    }
}
