using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using CodeCompletion.Model.Names;
using CodeCompletion.Model.Names.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CompletionEventSerializer
{
    [Export("JSON", typeof(ISerializer))]
    public class JsonSerializer : ISerializer
    {
        public void AppendTo<TInstance>(Stream targetStream, TInstance instance)
        {
            var streamWriter = new StreamWriter(targetStream, new UTF8Encoding(false));
            var jsonWriter = new JsonTextWriter(streamWriter) {Formatting = Formatting.None};
            var serializer = Newtonsoft.Json.JsonSerializer.Create();
            serializer.Serialize(jsonWriter, instance);
            streamWriter.WriteLine();
            streamWriter.Flush();
        }

        public TInstance Read<TInstance>(Stream source)
        {
            var json = "";
            while (source.Position < source.Length)
            {
                var nextChar = Encoding.UTF8.GetString(new[] { (byte) source.ReadByte() });
                if ((nextChar == "\r" || nextChar == "\n") && json.Length > 0)
                {
                    break;
                }
                json += nextChar;
            }
            return JsonConvert.DeserializeObject<TInstance>(json);
        }
    }

    public abstract class NameToJsonConverter<TName> : JsonConverter where TName : IName
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("identifier");
            writer.WriteValue((value as IName).Identifier);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var typeIdentifier = JObject.Load(reader).GetValue("identifier").ToString();
            return ConvertJson(typeIdentifier);
        }

        protected abstract TName ConvertJson(string typeIdentifier);

        public override bool CanConvert(Type objectType)
        {
            return typeof(TName) == objectType;
        }
    }

    public class TypeNameJsonConverter : NameToJsonConverter<TypeName>
    {
        protected override TypeName ConvertJson(string identifier)
        {
            return TypeName.Get(identifier, null);
        }
    }
}
