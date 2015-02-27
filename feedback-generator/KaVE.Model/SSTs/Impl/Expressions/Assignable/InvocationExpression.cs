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
using KaVE.JetBrains.Annotations;
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;
using KaVE.Utils.Assertion;

namespace KaVE.Model.SSTs.Impl.Expressions.Assignable
{
    public class InvocationExpression : IInvocationExpression
    {
        public IVariableReference Reference { get; set; }
        public IMethodName Name { get; set; }

        [NotNull]
        public IList<ISimpleExpression> Parameters { get; set; }

        public InvocationExpression()
        {
            Parameters = Lists.NewList<ISimpleExpression>();
        }

        private bool Equals(InvocationExpression other)
        {
            return string.Equals(Reference, other.Reference) && Equals(Name, other.Name) &&
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
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Parameters != null ? HashCodeUtils.For(398, Parameters) : 0);
                return hashCode;
            }
        }

        public void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            throw new System.NotImplementedException();
        }

        public static InvocationExpression New(IMethodName name, params ISimpleExpression[] parameters)
        {
            Asserts.That(name.IsStatic || name.IsConstructor);
            return new InvocationExpression
            {
                Name = name,
                Parameters = Lists.NewListFrom(parameters),
            };
        }

        public static InvocationExpression New(string id, IMethodName name, params ISimpleExpression[] parameters)
        {
            Asserts.Not(name.IsStatic || name.IsConstructor);
            return new InvocationExpression
            {
                Reference = new VariableReference {Identifier = id},
                Name = name,
                Parameters = Lists.NewListFrom(parameters),
            };
        }
    }
}