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
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl.Blocks
{
    public class IfElseBlock : IIfElseBlock
    {
        public IExpression Condition { get; set; }
        public IList<IStatement> Then { get; set; }
        public IList<IStatement> Else { get; set; }

        public IfElseBlock()
        {
            Then = Lists.NewList<IStatement>();
            Else = Lists.NewList<IStatement>();
        }

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

        public void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}