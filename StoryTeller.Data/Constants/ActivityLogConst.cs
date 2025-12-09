using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StoryTeller.Data.Constants
{
    public static class ActivityLogConst
    {
        public static class TargetType
        {
            public const string STORY = "Story";
            public const string COMMENT = "Comment";
            public const string USER = "User";
        }
        public static bool IsTargetType(string str)
            => str.Equals(TargetType.COMMENT, StringComparison.OrdinalIgnoreCase)
            || str.Equals(TargetType.STORY, StringComparison.OrdinalIgnoreCase)
            || str.Equals(TargetType.USER, StringComparison.OrdinalIgnoreCase);

        public static class Action
        {
            public const string SYSTEM = "System";
            public const string PUBLISH = "Publish";
            public const string COMMENT = "Comment";
            public const string RATING = "Rating";
            public const string VIEW = "View";
            public const string REPORT = "Report";
        }
        public static bool IsAction(string str)
            => str.Equals(Action.SYSTEM, StringComparison.OrdinalIgnoreCase)
            || str.Equals(Action.PUBLISH, StringComparison.OrdinalIgnoreCase)
            || str.Equals(Action.COMMENT, StringComparison.OrdinalIgnoreCase)
            || str.Equals(Action.RATING, StringComparison.OrdinalIgnoreCase)
            || str.Equals(Action.VIEW, StringComparison.OrdinalIgnoreCase)
            || str.Equals(Action.REPORT, StringComparison.OrdinalIgnoreCase);
    }
}
