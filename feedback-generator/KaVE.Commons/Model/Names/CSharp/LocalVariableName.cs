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

using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class LocalVariableName : Name
    {
        private static readonly WeakNameCache<LocalVariableName> Registry =
            WeakNameCache<LocalVariableName>.Get(id => new LocalVariableName(id));

        public new static LocalVariableName UnknownName
        {
            get { return Get("[?] ???"); }
        }

        public override bool IsUnknown
        {
            get { return Equals(this, UnknownName); }
        }

        /// <summary>
        ///     Local variable names have the form '[value-type-identifier] variable-name'.
        /// </summary>
        public new static LocalVariableName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private LocalVariableName(string identifier) : base(identifier) {}

        public string Name
        {
            get
            {
                var indexOfName = Identifier.LastIndexOf(']') + 2;
                return Identifier.Substring(indexOfName);
            }
        }

        public ITypeName ValueType
        {
            get
            {
                var lengthOfTypeIdentifier = Identifier.LastIndexOf(']') - 1;
                return TypeName.Get(Identifier.Substring(1, lengthOfTypeIdentifier));
            }
        }
    }
}