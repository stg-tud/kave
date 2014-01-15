using System;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class AssemblyVersion : Name, IAssemblyVersion
    {
        private static readonly WeakNameCache<AssemblyVersion> Registry = WeakNameCache<AssemblyVersion>.Get(id => new AssemblyVersion(id));

        /// <summary>
        /// Assembly version numbers have the format <code>'major'.'minor'.'build'.'revision'</code>.
        /// Examples of assembly versions are:
        /// <list type="bullet">
        ///     <item><description><code>1.2.3.4</code></description></item>
        ///     <item><description><code>4.0.0.0</code></description></item>
        /// </list>
        /// </summary>
        public new static AssemblyVersion Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private AssemblyVersion(string identifier)
            : base(identifier)
        {
        }

        private int[] Fragments { get { return Array.ConvertAll(Identifier.Split('.'), int.Parse); } }

        public int Major { get { return Fragments[0]; } }

        public int Minor { get { return Fragments[1]; } }

        public int Build { get { return Fragments[2]; } }

        public int Revision { get { return Fragments[3]; } }

        public int CompareTo(IAssemblyVersion other)
        {
            var otherVersion = other as AssemblyVersion;
            if (otherVersion == null) return int.MinValue;
            var majorDiff = Major - otherVersion.Major;
            if (majorDiff != 0) return majorDiff;
            var minorDiff = Minor - otherVersion.Minor;
            if (minorDiff != 0) return minorDiff;
            var buildDiff = Build - otherVersion.Build;
            if (buildDiff != 0) return buildDiff;
            return Revision - otherVersion.Revision;
        }

        public static bool operator <(AssemblyVersion v1, AssemblyVersion v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator >(AssemblyVersion v1, AssemblyVersion v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <=(AssemblyVersion v1, AssemblyVersion v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >=(AssemblyVersion v1, AssemblyVersion v2)
        {
            return v1.CompareTo(v2) >= 0;
        }
    }
}
