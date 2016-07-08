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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class AssemblyName : Name, IAssemblyName
    {
        private static readonly WeakNameCache<AssemblyName> Registry =
            WeakNameCache<AssemblyName>.Get(id => new AssemblyName(id));

        private readonly Regex _isValidVersionRegex = new Regex("^\\d\\.\\d\\.\\d\\.\\d$");

        public new static IAssemblyName UnknownName
        {
            get { return Get(UnknownNameIdentifier); }
        }

        public override bool IsUnknown
        {
            get { return Equals(this, UnknownName); }
        }

        /// <summary>
        ///     Assembly names follow the scheme <code>'assembly name'[, 'assembly version']</code>.
        ///     Example assembly names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>CodeCompletion.Model.Names, 1.0.0.0</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>CodeCompletion.Model.Names</code>
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Only the assembly name and version information are mandatory. Note, however, that object identity is only guarateed
        ///     for exact identifier matches.
        /// </summary>
        public new static AssemblyName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private AssemblyName(string identifier) : base(identifier) {}

        public IAssemblyVersion Version
        {
            get
            {
                var fragments = GetFragments();
                return fragments.Length <= 1
                    ? AssemblyVersion.UnknownName
                    : AssemblyVersion.Get(fragments[1]);
            }
        }

        public string Name
        {
            get { return GetFragments()[0]; }
        }

        public bool IsLocalProject
        {
            get { return Version.Equals(Names.UnknownAssembly.Version); }
        }

        private string[] GetFragments()
        {
            var split = Identifier.LastIndexOf(",", StringComparison.Ordinal);
            if (split == -1)
            {
                return new[] {Identifier};
            }
            var name = Identifier.Substring(0, split).Trim();
            var version = Identifier.Substring(split + 1).Trim();


            return _isValidVersionRegex.IsMatch(version)
                ? new[] {name, version}
                : new[] {Identifier};
        }
    }
}