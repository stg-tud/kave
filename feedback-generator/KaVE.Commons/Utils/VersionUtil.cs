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
 */

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using KaVE.Commons.Model;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils
{
    public class VersionUtil
    {
        private Assembly GetAssembly()
        {
            return typeof(VersionUtil).Assembly;
        }

        public virtual Version GetCurrentVersion()
        {
            return GetAssembly().GetName().Version;
        }

        public virtual string GetCurrentInformalVersion()
        {
            try
            {
                var attributeType = typeof(AssemblyInformationalVersionAttribute);
                var versions = GetAssembly().GetCustomAttributes(attributeType, true);
                // ReSharper disable once PossibleNullReferenceException
                return (versions[0] as AssemblyInformationalVersionAttribute).InformationalVersion;
            }
            catch
            {
                return "0.0-" + Variant.Unknown;
            }
        }

        public virtual Variant GetCurrentVariant()
        {
            try
            {
                var informalVersion = GetCurrentInformalVersion();
                var variantStr = informalVersion.Substring(informalVersion.LastIndexOf('-') + 1);
                return (Variant) Enum.Parse(typeof(Variant), variantStr);
            }
            catch
            {
                return Variant.Unknown;
            }
        }

        private static readonly Regex VersionExpr = new Regex("^0\\.(\\d+)-(\\w+)$");

        [NotNull]
        public static IKaVEVersion Parse([NotNull] string versionStr)
        {
            Asserts.Not(string.IsNullOrEmpty(versionStr));

            var res = VersionExpr.Match(versionStr);
            Asserts.That(res.Success);

            var version = int.Parse(res.Groups[1].ToString());
            var variant = (Variant) Enum.Parse(typeof(Variant), res.Groups[2].ToString());

            return new KaVEVersion
            {
                KaVEVersionNumber = version,
                Variant = variant
            };
        }
    }
}