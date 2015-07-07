using System;
using System.Reflection;
using KaVE.Commons.Model.SSTs;

namespace KaVE.Commons.Utils
{
    public class VersionUtil
    {
        private Assembly GetAssembly()
        {
            return typeof (ISST).Assembly;
        }

        public virtual Version GetCurrentVersion()
        {
            return GetAssembly().GetName().Version;
        }

        public virtual string GetCurrentInformalVersion()
        {
            try
            {
                var attributeType = typeof (AssemblyInformationalVersionAttribute);
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
                return (Variant) Enum.Parse(typeof (Variant), variantStr);
            }
            catch
            {
                return Variant.Unknown;
            }
        }

        public enum Variant
        {
            Unknown,
            Development,
            Default,
            Datev
        }
    }
}