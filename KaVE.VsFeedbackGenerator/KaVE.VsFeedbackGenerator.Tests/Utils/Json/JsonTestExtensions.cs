using System.IO;
using System.Text;
using KaVE.VsFeedbackGenerator.Utils.Json;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    internal static class JsonTestExtensions
    {
        public static byte[] AsBytes(this string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        public static string AsString(this MemoryStream stream)
        {
            return Encoding.Default.GetString(stream.ToArray());
        }

        public static string Serialize(this object obj)
        {
            var stream = new MemoryStream();
            using (var writer = new JsonLogWriter<object>(stream))
            {
                writer.Write(obj);
            }
            return stream.AsString();
        }

        public static TModel Deserialize<TModel>(this string serialization) where TModel : class
        {
            var stream = new MemoryStream(serialization.AsBytes());
            using (var reader = new JsonLogReader<TModel>(stream))
            {
                return reader.ReadNext();
            }
        }
    }
}