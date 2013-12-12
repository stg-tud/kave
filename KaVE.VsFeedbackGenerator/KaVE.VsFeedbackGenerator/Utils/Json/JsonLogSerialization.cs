using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using KaVE.Model.Names;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    static class JsonLogSerialization
    {
        static JsonLogSerialization()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        /// <summary>
        /// During deserialization of log files, loading the model assembly fails, when model types are used as type
        /// parameters of type from other assemblies, e.g., in case of lists of model objects.
        /// </summary>
        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name;
            if (assemblyName.StartsWith("KaVE.Model"))
            {
                return typeof(IName).Assembly;
            }
            return null;
        }

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