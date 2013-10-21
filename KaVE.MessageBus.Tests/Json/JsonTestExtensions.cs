using System.IO;
using System.Text;

namespace KaVE.MessageBus.Tests.Json
{
    static class JsonTestExtensions
    {
        public static byte[] AsBytes(this string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        public static string AsString(this MemoryStream stream)
        {
            return Encoding.Default.GetString(stream.ToArray());
        }
    }
}