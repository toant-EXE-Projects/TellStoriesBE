using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Data.Constants
{
    public record DefaultLibraryTemplate(string Title, string Description);
    public static class UserLibraryDefaults
    {
        public static readonly List<DefaultLibraryTemplate> DefaultCollections = new()
        {
            new("Câu Truyện Đã Thích", "Những câu truyện bạn đã thích"),
            new("Truyện Của Tôi", "Truyện Của Tôi"),
            //new("Lịch sử", "Truyện bạn đã đọc")
        };
    }
}
