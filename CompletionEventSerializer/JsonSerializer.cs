using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeCompletion.Model.Names;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CompletionEventSerializer
{
    [Export("JSON", typeof(ISerializer))]
    public class JsonSerializer : ISerializer
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                Converters =
                    {
                        new NameToJsonConverter()
                    },
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };

        public void AppendTo<TInstance>(Stream targetStream, TInstance instance)
        {
            var streamWriter = new StreamWriter(targetStream, new UTF8Encoding(false));
            var jsonWriter = new JsonTextWriter(streamWriter);
            var serializer = Newtonsoft.Json.JsonSerializer.Create(Settings);
            serializer.Serialize(jsonWriter, instance);
            streamWriter.WriteLine();
            streamWriter.Flush();
        }

        public TInstance Read<TInstance>(Stream source)
        {
            var json = ReadLine(source);
            return JsonConvert.DeserializeObject<TInstance>(json, Settings);
        }

        /// <summary>
        /// Reads the next non-empty lime from the stream. Asures that the stream position remains before the next line.
        /// </summary>
        private static string ReadLine(Stream source)
        {
            var json = "";
            while (source.Position < source.Length)
            {
                var nextChar = Encoding.UTF8.GetString(new[] {(byte) source.ReadByte()});
                if ((nextChar == "\r" || nextChar == "\n") && json.Length > 0)
                {
                    break;
                }
                json += nextChar;
            }
            return json;
        }
    }

    public class NameToJsonConverter : JsonConverter
    {
        private const string TypePropertyName = "type";
        private const string IdentifierPropertyName = "identifier";
        private const string NameQualifierPrefix = "CodeCompletion.Model.Names.";

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(TypePropertyName);
            writer.WriteValue(AliasFor(value.GetType()));
            writer.WritePropertyName(IdentifierPropertyName);
            writer.WriteValue(((IName) value).Identifier);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var jName = JObject.Load(reader);
            var factoryMethod = GetFactoryMethod(jName);
            var identifier = jName.GetValue(IdentifierPropertyName).ToString();
            return factoryMethod.Invoke(null, new object[] { identifier });
        }

        private static MethodInfo GetFactoryMethod(JObject jName)
        {
            var typeAlias = jName.GetValue(TypePropertyName).ToString();
            var type = TypeFrom(typeAlias);
            var factoryMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m =>
            {
                var parameterInfos = m.GetParameters();
                return parameterInfos.Count() == 1 && parameterInfos[0].ParameterType == typeof (string);
            }).First();
            return factoryMethod;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IName).IsAssignableFrom(objectType);
        }

        public static string AliasFor(Type nameType)
        {
            return nameType.FullName.Substring(NameQualifierPrefix.Length);
        }

        public static Type TypeFrom(string alias)
        {
            var assemblyName = typeof (IName).Assembly.FullName;
            var assemblyQualifiedTypeName = NameQualifierPrefix + alias + ", " + assemblyName;
            var type = Type.GetType(assemblyQualifiedTypeName);
            if (type == null)
            {
                throw new TypeLoadException("Could not load required type " + assemblyQualifiedTypeName);
            }
            return type;
        }
    }
}
