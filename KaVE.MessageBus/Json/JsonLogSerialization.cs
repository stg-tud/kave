using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;

namespace KaVE.MessageBus.Json
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
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
        };

        internal static readonly Encoding Encoding = new UTF8Encoding(false);
    }
}