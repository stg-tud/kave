using System.IO;
using System.Text;

namespace CompletionEventSerializer.Tests
{
    static class StreamTestExtensions
    {
        public static string AsString(this MemoryStream stream)
        {
            return Encoding.Default.GetString(stream.ToArray());
        }
    }
}