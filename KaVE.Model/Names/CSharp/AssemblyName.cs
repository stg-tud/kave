using System;
using System.Linq;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class AssemblyName : Name, IAssemblyName
    {
        private static readonly WeakNameCache<AssemblyName> Registry = WeakNameCache<AssemblyName>.Get(id => new AssemblyName(id));

        /// <summary>
        /// Assembly names follow the scheme <code>'assembly name'[, Version='assembly version']</code>.
        /// Example assembly names are:
        /// <list type="bullet">
        ///     <item><description><code>CodeCompletion.Model.Names, Version=1.0.0.0</code></description></item>
        ///     <item><description><code>CodeCompletion.Model.Names</code></description></item>
        /// </list>
        /// Only the assembly name and version information are mandatory. Note, however, that object identity is only guarateed for exact identifier matches.
        /// </summary>
        public new static AssemblyName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private AssemblyName(string identifier) : base(identifier) { }

        public IAssemblyVersion AssemblyVersion
        {
            get
            {
                if (Fragments.Length <= 1)
                {
                    return null;
                }
                var version = Fragments[1].Split('=')[1];
                return CSharp.AssemblyVersion.Get(version);
            }
        }

        public string Name { get { return Fragments[0]; } }

        private String[] Fragments
        {
            get
            {
                return Identifier.Split(',').Select(f => f.Trim()).ToArray();
            }
        }
    }
}
