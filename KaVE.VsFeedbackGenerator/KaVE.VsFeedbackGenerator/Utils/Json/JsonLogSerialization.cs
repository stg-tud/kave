using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            Binder = new MyTypeNameBinderForDebugging()
        };

        internal static readonly Encoding Encoding = new UTF8Encoding(false);
    }

    public class MyTypeNameBinderForDebugging : DefaultSerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (typeName.Contains("[["))
            {
                // TODO make this a clean solution!

                // resolution of only the type parameter succeeds
                //var typeNameStart = typeName.IndexOf("[[", System.StringComparison.Ordinal) + 2;
                //var typeNameEnd = typeName.IndexOf("]]", System.StringComparison.Ordinal);
                //var parameterTypeName = typeName.Substring(typeNameStart, typeNameEnd - typeNameStart);
                //var endOfTypeName = parameterTypeName.IndexOf(',');
                //var parameterType = base.BindToType(parameterTypeName.Substring(endOfTypeName + 2), parameterTypeName.Substring(0, endOfTypeName));

                // resolution of the type thru the assembly of the base type fails
                //Assembly assembly1 = Assembly.LoadWithPartialName(assemblyName);
                //Type type = assembly1.GetType(typeName);

                // resolution of the type thru the general type-resolution mechanism succeeds
                return Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
            }

            return base.BindToType(assemblyName, typeName);
        }
    }
}