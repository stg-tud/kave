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

using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types.Organization
{
    public class NamespaceName : BaseName, INamespaceName
    {
        public NamespaceName() : this(UnknownNameIdentifier) {}
        public NamespaceName([NotNull] string identifier) : base(identifier) {}

        public INamespaceName ParentNamespace
        {
            get
            {
                if (IsGlobalNamespace)
                {
                    return null;
                }
                if (IsUnknown)
                {
                    return new NamespaceName();
                }
                var lastSeperatorIndex = Identifier.LastIndexOf('.');
                return lastSeperatorIndex == -1
                    ? new NamespaceName("")
                    : new NamespaceName(Identifier.Substring(0, lastSeperatorIndex));
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
            get { return Identifier.Equals(""); }
        }

        public override bool IsUnknown
        {
            get { return UnknownNameIdentifier.Equals(Identifier); }
        }
    }
}