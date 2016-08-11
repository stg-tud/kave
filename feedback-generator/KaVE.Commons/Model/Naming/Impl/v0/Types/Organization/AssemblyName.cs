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
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types.Organization
{
    public class AssemblyName : BaseName, IAssemblyName
    {
        public AssemblyName() : this(UnknownNameIdentifier) {}

        public AssemblyName(string identifier) : base(identifier)
        {
            Version = new AssemblyVersion();

            if (!UnknownNameIdentifier.Equals(identifier) && !identifier.Contains(","))
            {
                IsLocalProject = true;
            }

            foreach (var c in new[] {"(", ")", "[", "]", "{", "}", ",", ";", ":", " "})
            {
                if (Name.Contains(c))
                {
                    throw new ValidationException("identifier must not contain the char '{0}'".FormatEx(c), null);
                }
            }

            var fragments = GetFragments();
            Version = fragments.Length <= 1
                ? new AssemblyVersion()
                : new AssemblyVersion(fragments[1]);
        }

        public IAssemblyVersion Version { get; private set; }

        public string Name
        {
            get { return GetFragments()[0]; }
        }

        public bool IsLocalProject { get; private set; }

        private string[] GetFragments()
        {
            var split = Identifier.LastIndexOf(",", StringComparison.Ordinal);
            if (split == -1)
            {
                return new[] {Identifier};
            }
            var name = Identifier.Substring(0, split).Trim();
            var version = Identifier.Substring(split + 1).Trim();


            return new[] {name, version};
        }

        public override bool IsUnknown
        {
            get { return UnknownNameIdentifier.Equals(Identifier); }
        }
    }
}