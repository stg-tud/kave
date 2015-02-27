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
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl.Expressions.Assignable
{
    public class LambdaExpression : ILambdaExpression
    {
        public IList<IVariableDeclaration> Parameters { get; set; }
        public IList<IStatement> Body { get; set; }

        public LambdaExpression()
        {
            Parameters = Lists.NewList<IVariableDeclaration>();
            Body = Lists.NewList<IStatement>();
        }

        private bool Equals(LambdaExpression other)
        {
            return Equals(Body, other.Body);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return (Body != null ? Body.GetHashCode() : 0);
        }

        public void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}