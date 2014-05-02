using System;
using System.Linq;
using System.Reflection;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    internal class NameToJsonConverter : JsonConverter
    {
        private const string TypePropertyName = "type";
        private const string IdentifierPropertyName = "id";
        private const string OldIdentifierPropertyName = "identifier";
        private const string NameQualifierPrefix = "KaVE.Model.Names.";

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(TypePropertyName);
            writer.WriteValue(AliasFor(value.GetType()));
            writer.WritePropertyName(IdentifierPropertyName);
            writer.WriteValue(((IName) value).Identifier);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jName = JObject.Load(reader);
            var factoryMethod = GetFactoryMethod(jName);
            var identifier = GetIdentifier(jName);
            return factoryMethod.Invoke(null, new object[] {identifier});
        }

        private static string GetIdentifier(JObject jName)
        {
            var idToken = jName.GetValue(IdentifierPropertyName) ?? jName.GetValue(OldIdentifierPropertyName);
            return idToken.ToString();
        }

        private static MethodInfo GetFactoryMethod(JObject jName)
        {
            var typeAlias = jName.GetValue(TypePropertyName).ToString();
            var type = TypeFrom(typeAlias);
            var factoryMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(
                m =>
                {
                    var parameterInfos = m.GetParameters();
                    return parameterInfos.Count() == 1 && parameterInfos[0].ParameterType == typeof (string);
                }).First();
            return factoryMethod;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (IName).IsAssignableFrom(objectType);
        }

        private static string AliasFor(Type nameType)
        {
            return nameType.FullName.Substring(NameQualifierPrefix.Length);
        }

        private static Type TypeFrom(string alias)
        {
            var assemblyName = typeof (IName).Assembly.FullName;
            var assemblyQualifiedTypeName = NameQualifierPrefix + alias + ", " + assemblyName;
            var type = Type.GetType(assemblyQualifiedTypeName);
            Asserts.NotNull(type, "Could not load required type " + assemblyQualifiedTypeName);
            return type;
        }
    }
}