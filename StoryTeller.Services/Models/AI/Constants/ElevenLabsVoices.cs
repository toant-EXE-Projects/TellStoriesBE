using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StoryTeller.Services.Models.AI.Constants
{
    // Gender_Region_Name(optional)
    // Male_Southern_Nam
    public class VoiceInfo
    {
        public string Name { get; set; } = string.Empty;
        public string VoiceId { get; set; } = string.Empty;
    }
    public static class ElevenLabsVoices
    {

        public const string Male_Southern_TrieuDuong = "UsgbMVmY3U59ijwK5mdh";
        public const string Male_Southern_TVSang = "7WNWm0yUcEolHsfg5Bhk";
        //public const string Male_Southern_DonHung = "LPldyaIkUUSOPCRFrgYJ";
        public const string Female_Southern_Linh = "1l0C0QA9c9jN22EmWiB0";
        public const string Female_Standard_NNgan = "DvG3I1kDzdBY3u4EzYh6";
        //public const string Female_Standard_KhanhLy = "HQZkBNMmZF5aISnrU842";

        public static IEnumerable<VoiceInfo> GetAllVoices()
        {
            var fields = typeof(ElevenLabsVoices).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    yield return new VoiceInfo
                    {
                        Name = field.Name,
                        VoiceId = (string)field.GetRawConstantValue()
                    };
                }
            }
        }

    }
}
