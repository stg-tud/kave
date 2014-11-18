/*
 * Copyright 2014 Technische Universitšt Darmstadt
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

using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Collections;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Blocks
{
    public class WhileLoop : Statement
    {
        public Expression Condition { get; set; }
        public readonly IList<Statement> Body = Lists.NewList<Statement>();

        private bool Equals(WhileLoop other)
        {
            return Body.Equals(other.Body) && Equals(Condition, other.Condition);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Body.GetHashCode()*397) ^ (Condition != null ? Condition.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("while ({0}) {{{1}}};", Condition, string.Join(" ", Body.Select(s => s.ToString())));
        }
    }
}