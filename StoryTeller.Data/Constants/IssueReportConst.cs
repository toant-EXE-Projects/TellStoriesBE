using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Constants
{
    public static class IssueReportConst
    {
        public static class TargetType
        {
            public const string COMMENT = "Comment";
            public const string STORY = "Story";
            public const string BUG = "Bug";
            public const string OTHER = "Other";
        
            public static string ToActivityLogConst(string targetType)
            {
                switch (targetType)
                {
                    case COMMENT:
                        return ActivityLogConst.TargetType.COMMENT;
                    
                    case STORY:
                        return ActivityLogConst.TargetType.STORY;

                    case BUG:
                        return "Lỗi";

                    case OTHER:
                        return "Khác";

                    default:
                        return string.Empty;
                }
            }
        }
        public static bool IsTargetType(string str)
            => str.Equals(TargetType.COMMENT, StringComparison.OrdinalIgnoreCase)
            || str.Equals(TargetType.STORY, StringComparison.OrdinalIgnoreCase)
            || str.Equals(TargetType.BUG, StringComparison.OrdinalIgnoreCase)
            || str.Equals(TargetType.OTHER, StringComparison.OrdinalIgnoreCase)
            ;

        public static class Status
        {
            public const string PENDING = "Pending";
            public const string RESOLVED = "Resolved";
        }

        public static class IssueType
        {
            public const string SPAM = "Spam";
            public const string HARASSMENT = "Harassment";
            public const string TOXICBEHAVIOR = "Toxic Behavior";

        }
        public static bool IsIssueTypeComment(string str)
            => str.Equals(IssueType.SPAM, StringComparison.OrdinalIgnoreCase)
            || str.Equals(IssueType.HARASSMENT, StringComparison.OrdinalIgnoreCase)
            || str.Equals(IssueType.TOXICBEHAVIOR, StringComparison.OrdinalIgnoreCase);
    }
}
