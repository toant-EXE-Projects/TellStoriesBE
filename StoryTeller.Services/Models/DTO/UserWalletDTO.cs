using StoryTeller.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.DTO
{
    public class UserWalletDTO
    {
        public string UserId { get; set; }
        public UserMinimalDTO User { get; set; }

        public int Balance { get; set; }
        public bool IsLocked { get; set; }
    }
}
