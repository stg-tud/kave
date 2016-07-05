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

using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v0.IDEComponents
{
    public class CommandBarControlName : Name, IIDEComponentName
    {
        public const char HierarchySeperator = '|';

        private static readonly WeakNameCache<CommandBarControlName> Registry =
            WeakNameCache<CommandBarControlName>.Get(id => new CommandBarControlName(id));

        public new static CommandBarControlName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private CommandBarControlName(string identifier)
            : base(identifier) {}

        public CommandBarControlName Parent
        {
            get
            {
                var endOfParentIdentifier = Identifier.LastIndexOf(HierarchySeperator);
                return endOfParentIdentifier < 0 ? null : Get(Identifier.Substring(0, endOfParentIdentifier));
            }
        }

        public string Name
        {
            get
            {
                var startOfName = Identifier.LastIndexOf(HierarchySeperator) + 1;
                return Identifier.Substring(startOfName);
            }
        }
    }
}