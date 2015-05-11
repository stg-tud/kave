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

using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class EnumTypeName : TypeName
    {
        private const string NamePrefix = "e:";

        internal static bool IsEnumTypeIdentifier(string identifier)
        {
            return identifier.StartsWith(NamePrefix);
        }

        [UsedImplicitly]
        internal new static ITypeName Get(string identifier)
        {
            return TypeName.Get(identifier);
        }

        internal EnumTypeName(string identifier) : base(identifier) {}

        public override string FullName
        {
            get { return base.FullName.Substring(NamePrefix.Length); }
        }

        public override bool IsEnumType
        {
            get { return true; }
        }
    }
}