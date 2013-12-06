using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils.Json
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
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
        };

        internal static readonly Encoding Encoding = new UTF8Encoding(false);
    }
}