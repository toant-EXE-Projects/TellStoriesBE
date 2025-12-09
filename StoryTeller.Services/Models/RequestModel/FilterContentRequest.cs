using StoryTeller.Data.Entities.AI;

namespace StoryTeller.Services.Models.RequestModel
{
    public class FilterContentRequest
    {
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// If true, the API will reject profane text. If false, it will censor it (replace with ****).
        /// </summary>
        public bool BlockIfProfanity { get; set; } = false;
    }
}
