using System;
using System.Linq;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class AssemblyName : Name, IAssemblyName
    {
        private static readonly WeakNameCache<AssemblyName> Registry = WeakNameCache<AssemblyName>.Get(id => new AssemblyName(id));

        /// <summary>
        /// Assembly names follow the scheme <code>'assembly name'[, 'assembly version']</code>.
        /// Example assembly names are:
        /// <list type="bullet">
        ///     <item><description><code>CodeCompletion.Model.Names, 1.0.0.0</code></description></item>
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
                var fragments = GetFragments();
                return fragments.Length <= 1 ? null : CSharp.AssemblyVersion.Get(fragments[1]);
            }
        }

        public string Name { get { return GetFragments()[0]; } }

        private String[] GetFragments()
        {
                return Identifier.Split(',').Select(f => f.Trim()).ToArray();
        }
    }
}
