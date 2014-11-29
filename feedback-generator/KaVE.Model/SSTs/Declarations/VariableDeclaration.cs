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
 *    - Sebastian Proksch
 */

using KaVE.Model.Names;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Declarations
{
    public class VariableDeclaration : Statement
    {
        public string Identifier { get; set; }
        public ITypeName Type { get; set; }

        public VariableDeclaration()
        {
            // TODO get rid of this constructor too as soon as we got rid of the other one
        }

        // TODO get rid of this constructor
        public VariableDeclaration(string identifier, ITypeName type)
        {
            Identifier = identifier;
            Type = type;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(VariableDeclaration other)
        {
            return string.Equals(Identifier, other.Identifier) && Equals(Type, other.Type);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 20 + ((Identifier != null ? Identifier.GetHashCode() : 0)*397) ^
                       (Type != null ? Type.GetHashCode() : 0);
            }
        }
    }
}