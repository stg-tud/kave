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
using KaVE.Model.Names;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl.Expressions.Assignable
{
    public class InvocationExpression : IInvocationExpression
    {
        public IVariableReference Reference { get; set; }
        public IMethodName MethodName { get; set; }
        public IList<ISimpleExpression> Parameters { get; set; }

        public InvocationExpression()
        {
            Parameters = Lists.NewList<ISimpleExpression>();
        }

        private bool Equals(IInvocationExpression other)
        {
            return string.Equals(Reference, other.Reference) && Equals(MethodName, other.MethodName) &&
                   Parameters.Equals(other.Parameters);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 11 + (Reference != null ? Reference.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Parameters.GetHashCode();
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