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

namespace KaVE.Model.SSTs.Expressions
{
    public class IfElseExpression : Expression
    {
        public Expression Condition { get; set; }
        // use LambdaExpression here in case of more complex subexpressions
        public Expression IfExpression { get; set; }
        public Expression ElseExpression { get; set; }

        private bool Equals(IfElseExpression other)
        {
            return Equals(Condition, other.Condition) && Equals(IfExpression, other.IfExpression) &&
                   Equals(ElseExpression, other.ElseExpression);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Condition != null ? Condition.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IfExpression != null ? IfExpression.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ElseExpression != null ? ElseExpression.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}