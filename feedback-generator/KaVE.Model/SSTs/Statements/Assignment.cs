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

using KaVE.Utils;

namespace KaVE.Model.SSTs.Statements
{
    public class Assignment : Statement
    {
        public string Identifier { get; set; }
        public Expression Value { get; set; }

        public Assignment() {}

        public Assignment(string identifier, Expression expr)
        {
            Identifier = identifier;
            Value = expr;
        }

        protected bool Equals(Assignment other)
        {
            return string.Equals(Identifier, other.Identifier) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifier != null ? Identifier.GetHashCode() : 0)*397) ^
                       (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1};", Identifier, Value);
        }
    }
}