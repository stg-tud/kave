using System.Text;
using Newtonsoft.Json;

namespace KAVE.KAVE_MessageBus.Json
{
    static class JsonLogSerialization
    {
        internal static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Converters =
            {
                new NameToJsonConverter()
            },
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        internal static readonly Encoding Encoding = new UTF8Encoding(false);
    }
}