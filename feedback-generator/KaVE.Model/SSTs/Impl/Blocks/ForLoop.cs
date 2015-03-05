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
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl.Blocks
{
    public class ForLoop : IForLoop
    {
        public IList<IStatement> Init { get; set; }
        public ILoopHeaderExpression Condition { get; set; }
        public IList<IStatement> Step { get; set; }
        public IList<IStatement> Body { get; set; }

        public ForLoop()
        {
            Init = Lists.NewList<IStatement>();
            Step = Lists.NewList<IStatement>();
            Body = Lists.NewList<IStatement>();
        }

        private bool Equals(ForLoop other)
        {
            return Body.Equals(other.Body) && Step.Equals(other.Step) && Init.Equals(other.Init) &&
                   Equals(Condition, other.Condition);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 34 + Init.GetHashCode();
                hashCode = (hashCode*397) ^ Step.GetHashCode();
                hashCode = (hashCode*397) ^ Body.GetHashCode();
                hashCode = (hashCode*397) ^ (Condition != null ? Condition.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            visitor.Visit(this, context);
        }

        public TReturn Accept<TContext, TReturn>(ISSTNodeVisitor<TContext, TReturn> visitor, TContext context)
        {
            return visitor.Visit(this, context);
        }
    }
}