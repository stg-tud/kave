/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using KaVE.Commons.Model.Names;
using KaVE.JetBrains.Annotations;
using Newtonsoft.Json;

namespace KaVE.Commons.Utils.Json
{
    public static class JsonSerialization
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
            if (assemblyName.StartsWith("KaVE.Commons.Model"))
            {
                return typeof (IName).Assembly;
            }
            if (assemblyName == "System")
            {
                return typeof (ISet<>).Assembly;
            }
            return null;
        }

        public static readonly Encoding Encoding = new UTF8Encoding(false);

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
        public static string ToCompactJson([CanBeNull] this object instance)
        {
            var settings = CreateSerializationSettings();
            settings.Formatting = Formatting.None;
            var json = JsonConvert.SerializeObject(instance, settings);
            // TODO discuss this replacement (seb)
            json = new SSTTypeNameShortener().RemoveDetails(json);
            return json;
        }

        /// <summary>
        ///     Converts an object to a formatted Json string.
        /// </summary>
        /// <remarks>
        ///     The method uses the same serialization settings as <see cref="ToCompactJson" />, except for the formatting.
        /// </remarks>
        [NotNull]
        public static string ToFormattedJson([CanBeNull] this object instance)
        {
            var settings = CreateSerializationSettings();
            settings.Formatting = Formatting.Indented;
            return SerializeWithCustomIndentationDepth(instance, settings);
        }

        /// <summary>
        ///     Parses an instance of <typeparamref name="T" /> from a Json string.
        /// </summary>
        /// <remarks>
        ///     Uses the same serialization settings as <see cref="ToCompactJson" />.
        /// </remarks>
        public static T ParseJsonTo<T>([NotNull] this string json)
        {
            var settings = CreateSerializationSettings();
            // TODO get rid of this special case handling
            // BEGIN legacy-data hanlding
            if (json.StartsWith("{\"$type\":\"KaVE.Model.Events.CompletionEvent.CompletionEvent, KaVE.Model\""))
            {
                json = json.Replace("KaVE.Model.Events.CompletionEvent.", "KaVE.Model.Events.CompletionEvents.");
            }
            json = json.Replace("KaVE.Model.", "KaVE.Commons.Model.").Replace("KaVE.Model", "KaVE.Commons");
            json = json.Replace(
                "KaVE.Commons.Model.SSTs.Impl.Declarations.VariableDeclaration",
                "KaVE.Commons.Model.SSTs.Impl.Statements.VariableDeclaration");
            // TODO discuss this replacement (seb)
            json = new SSTTypeNameShortener().AddDetails(json);
            settings.Converters.Add(new ProposalCollectionConverter());
            // END legacy-data handling
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        ///     Converts an object to a pretty Json string that is tuned for human readability.
        /// </summary>
        /// <remarks>
        ///     Pretty-print serialization cannot generally be deserialized, because information is lost during serialization.
        /// </remarks>
        [NotNull]
        public static string ToPrettyPrintJson([CanBeNull] this object instance)
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