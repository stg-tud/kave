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

namespace KaVE.Model.Names.CSharp
{
    /// <summary>
    ///     Aliases are defined by using statements, like "using alias = Some.Reference;". A special case is the alias
    ///     "global" that represents the global namespace by convention.
    /// </summary>
    public class AliasName : Name
    {
        private static readonly WeakNameCache<AliasName> Registry = WeakNameCache<AliasName>.Get(
            id => new AliasName(id));

        public new static AliasName UnknownName
        {
            get { return Get(UnknownNameIdentifier); }
        }

        /// <summary>
        ///     Alias names are valid C# identifiers that are not keywords, plus the special alias 'global'.
        /// </summary>
        public new static AliasName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private AliasName(string identifier) : base(identifier) {}
    }
}