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
    public class ComposedExpression : Expression
    {
        public string[] Variables { get; set; }

        private bool Equals(ComposedExpression other)
        {
            if (Variables == other.Variables)
            {
                return true;
            }

            if (Variables == null || other.Variables == null)
            {
                return false;
            }

            return Variables.DeepEquals(other.Variables);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return 5 + (Variables != null ? HashCodeUtils.For(5, Variables) : 0);
        }
    }
}