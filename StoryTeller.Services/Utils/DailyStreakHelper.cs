using StoryTeller.Data.Constants;

namespace StoryTeller.Services.Utils
{
    public static class DailyStreakHelper
    {
        public static int CalculateDailyStreakPoints(int streakCount, int dailyRewardPoints)
        {
            //if (streakCount < 5) return 5;
            //if (streakCount < 10) return 10;
            //if (streakCount < 30) return 20;
            return dailyRewardPoints;
            //return streakCount switch
            //{
            //    1 => 10,   // First day
            //    3 => 20,   // 3-day streak
            //    7 => 50,   // 7-day streak
            //    30 => 200, // 30-day streak
            //    _ => 5     // Default daily reward
            //};
        }
    }
}
