using StoryTeller.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.ResponseModel
{
    public class DashboardResponse
    {
        public int NewAccount { get; set; } = 0;
        public float? NewAccountFluct { get; set; } = 0;

        public int ActiveAccount { get; set; } = 0;
        public float? ActiveAccountFluct { get; set; } = 0;

        public int PublishedStories { get; set; } = 0;
        public float? PublishedStoriesFluct { get; set; } = 0;

        public int StoriesViews { get; set; } = 0;
        public float? StoriesViewsFluct { get; set; } = 0;

        public List<Statistic> Statistics { get; set; } = [];
    }

    public class Statistic
    {
        public DateTime Date { get; set; }
        public int NewAccount { get; set; } = 0;
        public int ActiveAccount { get; set; } = 0;
        public int PublishedStories { get; set; } = 0;
        public int StoriesViews { get; set; } = 0;
    }
}
