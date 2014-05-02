using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    internal static class JsonSerialization
    {
        private const int IndentationDepth = 4;

        static JsonSerialization()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        /// <summary>
        ///     During deserialization of log files, loading the model assembly fails, when model types are used as type
        ///     parameters of type from other assemblies, e.g., in case of lists of model objects.
        /// </summary>
        private static Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = args.Name;
            if (assemblyName.StartsWith("KaVE.Model"))
            {
                return typeof (IName).Assembly;
            }
            return null;
        }

        internal static readonly Encoding Encoding = new UTF8Encoding(false);

        private static readonly JsonSerializerSettings PrettyPrintSettings = new JsonSerializerSettings
        {
            Converters =
            {
                new NameToIdentifierConverter(),
                new EnumToStringConverter()
            },
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None
        };

        private static JsonSerializerSettings CreateSerializationSettings()
        {
            return new JsonSerializerSettings
            {
                Converters =
                {
                    new NameToJsonConverter()
                },
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            };
        }

        /// <summary>
        ///     Converts an object to a Json string without any unnecessary whitespaces or newlines.
        /// </summary>
        [NotNull]
        internal static string ToCompactJson([CanBeNull] this object instance)
        {
            var settings = CreateSerializationSettings();
            settings.Formatting = Formatting.None;
            return JsonConvert.SerializeObject(instance, settings);
        }

        /// <summary>
        ///     Converts an object to a formatted Json string.
        /// </summary>
        /// <remarks>
        ///     The method uses the same serialization settings as <see cref="ToCompactJson" />, except for the formatting.
        /// </remarks>
        [NotNull]
        internal static string ToFormattedJson([CanBeNull] this object instance)
        {
            var settings = CreateSerializationSettings();
            settings.Formatting = Formatting.Indented;
            return SerializeWithCustomIndentationDepth(instance, settings);
        }

        /// <summary>
        /// Parses an instance of <typeparamref name="T"/> from a Json string.
        /// </summary>
        /// <remarks>
        /// Uses the same serialization settings as <see cref="ToCompactJson"/>.
        /// </remarks>
        [CanBeNull]
        internal static T ParseJsonTo<T>([NotNull] this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, CreateSerializationSettings());
        }

        /// <summary>
        /// Converts an object to a pretty Json string that is tuned for human readability.
        /// </summary>
        /// <remarks>
        /// Pretty-print serialization cannot generally be deserialized, because information is lost during serialization.
        /// </remarks>
        [NotNull]
        internal static string ToPrettyPrintJson([CanBeNull] this object instance)
        {
            return SerializeWithCustomIndentationDepth(instance, PrettyPrintSettings);
        }

        private static string SerializeWithCustomIndentationDepth(object instance,
            JsonSerializerSettings jsonSerializerSettings)
        {
            var stringWriter = new StringWriter();
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Indentation = IndentationDepth;
                var serializer = JsonSerializer.Create(jsonSerializerSettings);
                serializer.Serialize(jsonWriter, instance);
                return stringWriter.ToString();
            }
        }
    }
}