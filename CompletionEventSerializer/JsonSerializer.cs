﻿using System;
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
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {
                Converters =
                    {
                        new NameJsonConverter(),
                        new AssemblyNameJsonConverter(),
                        new AssemblyVersionJsonConverter(),
                        new FieldNameJsonConverter(),
                        new MethodNameJsonConverter(),
                        new NamespaceNameJsonConverter(),
                        new ParameterNameJsonConverter(),
                        new PropertyNameJsonConverter(),
                        new TypeNameJsonConverter()
                    },
                Formatting = Formatting.None
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

    public abstract class NameToJsonConverter<TName, TIName> : JsonConverter
        where TName : class, IName
        where TIName : IName
    {
        private readonly ConvertJson _converter;

        protected delegate TName ConvertJson(string identifier);

        protected NameToJsonConverter(ConvertJson converter)
        {
            _converter = converter;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("identifier");
            writer.WriteValue(((IName) value).Identifier);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var identifier = JObject.Load(reader).GetValue("identifier").ToString();
            return _converter.Invoke(identifier);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TName) == objectType || typeof(TIName) == objectType;
        }
    }

    class NameJsonConverter : NameToJsonConverter<Name, IName>
    {
        public NameJsonConverter() : base(Name.Get) {}
    }

    class AssemblyNameJsonConverter : NameToJsonConverter<AssemblyName, IAssemblyName>
    {
        public AssemblyNameJsonConverter() : base(AssemblyName.Get) {}
    }

    class AssemblyVersionJsonConverter : NameToJsonConverter<AssemblyVersion, IAssemblyVersion>
    {
        public AssemblyVersionJsonConverter() : base(AssemblyVersion.Get) {}
    }

    class FieldNameJsonConverter : NameToJsonConverter<FieldName, IFieldName>
    {
        public FieldNameJsonConverter() : base(FieldName.Get) {}
    }

    class MethodNameJsonConverter : NameToJsonConverter<MethodName, IMemberName>
    {
        public MethodNameJsonConverter() : base(MethodName.Get) {}
    }

    class NamespaceNameJsonConverter : NameToJsonConverter<NamespaceName, INamespaceName>
    {
        public NamespaceNameJsonConverter() : base(NamespaceName.Get) {}
    }

    class ParameterNameJsonConverter : NameToJsonConverter<ParameterName, IParameterName>
    {
        public ParameterNameJsonConverter() : base(ParameterName.Get) {}
    }

    class PropertyNameJsonConverter : NameToJsonConverter<PropertyName, IPropertyName>
    {
        public PropertyNameJsonConverter() : base(PropertyName.Get) { }
    }

    class TypeNameJsonConverter : NameToJsonConverter<TypeName, ITypeName>
    {
        public TypeNameJsonConverter() : base(TypeName.Get) {}
    }


}
