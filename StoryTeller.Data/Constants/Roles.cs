using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string User = "User";

        public static readonly string[] All = { User, Moderator, Admin };
        public static readonly string[] Staff = { Moderator, Admin };
    }
}
