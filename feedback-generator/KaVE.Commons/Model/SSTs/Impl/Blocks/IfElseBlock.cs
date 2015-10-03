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
 */

using System.Collections.Generic;
using System.Runtime.Serialization;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.SSTs.Impl.Blocks
{
    [DataContract]
    public class IfElseBlock : IIfElseBlock
    {
        [DataMember]
        public ISimpleExpression Condition { get; set; }

        [DataMember]
        public IKaVEList<IStatement> Then { get; set; }

        [DataMember]
        public IKaVEList<IStatement> Else { get; set; }

        public IEnumerable<ISSTNode> Children
        {
            get
            {
                var children = Lists.NewList<ISSTNode>(Condition);
                foreach (var stmt in Then)
                {
                    children.Add(stmt);
                }
                foreach (var stmt in Else)
                {
                    children.Add(stmt);
                }
                return children;
            }
        }

        public IfElseBlock()
        {
            Condition = new UnknownExpression();
            Then = Lists.NewList<IStatement>();
            Else = Lists.NewList<IStatement>();
        }

        private bool Equals(IfElseBlock block)
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
                var hashCode = 35 + Then.GetHashCode();
                hashCode = (hashCode*397) ^ Else.GetHashCode();
                hashCode = (hashCode*397) ^ Condition.GetHashCode();
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

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}