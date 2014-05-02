using System.IO;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    internal static class MemoryStreamUtils
    {
        public static string AsString(this MemoryStream stream)
        {
            return JsonSerialization.Encoding.GetString(stream.ToArray());
        }

        public static MemoryStream AsStream(this string data)
        {
            return new MemoryStream(JsonSerialization.Encoding.GetBytes(data));
        }
    }
}