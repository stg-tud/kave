using KaVE.JetBrains.Annotations;
using Newtonsoft.Json;

namespace KaVE.Utils.Serialization
{
    public static class JsonExtensions
    {
        [NotNull]
        public static string ToJson([NotNull] this object instance, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(
                instance,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = converters
                });
        }
    }
}