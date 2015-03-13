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

using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp.Modularization
{
    public class NamespaceName : Name, INamespaceName
    {
        public const string GlobalNamespaceIdentifier = "";

        private static readonly WeakNameCache<NamespaceName> Registry =
            WeakNameCache<NamespaceName>.Get(id => new NamespaceName(id));

        public static readonly INamespaceName GlobalNamespace = Get(GlobalNamespaceIdentifier);

        public new static INamespaceName UnknownName
        {
            get { return Get(UnknownNameIdentifier); }
        }

        /// <summary>
        ///     Namespace names follow the scheme <code>'parent namespace name'.'namespace name'</code>. An exception is the global
        ///     namespace, which has the empty string as its identfier.
        ///     Examples of namespace names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>System</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>CodeCompletion.Model.Names.CSharp</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public new static NamespaceName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private NamespaceName(string identifier)
            : base(identifier) {}

        public INamespaceName ParentNamespace
        {
            get
            {
                if (IsGlobalNamespace)
                {
                    return null;
                }
                var lastSeperatorIndex = Identifier.LastIndexOf('.');
                return lastSeperatorIndex == -1
                    ? GlobalNamespace
                    : Get(Identifier.Substring(0, lastSeperatorIndex));
            }
        }

        public string Name
        {
            get
            {
                var lastSeperatorIndex = Identifier.LastIndexOf('.');
                return Identifier.Substring(lastSeperatorIndex + 1);
            }
        }

        public bool IsGlobalNamespace
        {
            get { return Identifier.Equals(GlobalNamespaceIdentifier); }
        }
    }
}