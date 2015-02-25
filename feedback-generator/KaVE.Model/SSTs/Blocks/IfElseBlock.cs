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

using System.Collections.Generic;
using KaVE.Model.Collections;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Blocks
{
    public class IfElseBlock : IStatement
    {
        public IExpression Condition { get; set; }
        public readonly IList<IStatement> Then = Lists.NewList<IStatement>();
        public readonly IList<IStatement> Else = Lists.NewList<IStatement>();

        public bool Equals(IfElseBlock block)
        {
            return Equals(Condition, block.Condition) && Equals(Then, block.Then) && Equals(Else, block.Else);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Then.GetHashCode();
                hashCode = (hashCode*397) ^ Else.GetHashCode();
                hashCode = (hashCode*397) ^ (Condition != null ? Condition.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}