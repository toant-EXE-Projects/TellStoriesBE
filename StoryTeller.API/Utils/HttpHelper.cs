namespace StoryTeller.API.Utils
{
    public static class HttpHelper
    {
        public static bool IsMobileDevice(string userAgent)
        {
            return userAgent.Contains("iphone") ||
                   userAgent.Contains("android") ||
                   userAgent.Contains("ipad") ||
                   userAgent.Contains("mobile");
        }
    }
}
