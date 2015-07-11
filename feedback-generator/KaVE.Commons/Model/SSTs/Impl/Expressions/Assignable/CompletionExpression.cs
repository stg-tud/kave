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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable
{
    public class CompletionExpression : ICompletionExpression
    {
        public ITypeName TypeReference { get; set; }
        public IVariableReference VariableReference { get; set; }
        public string Token { get; set; }

        public CompletionExpression()
        {
            Token = "";
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(ICompletionExpression other)
        {
            var isEqTypeRef = Equals(TypeReference, other.TypeReference);
            var isEqObjRef = Equals(VariableReference, other.VariableReference);
            var isEqToken = string.Equals(Token, other.Token);
            return isEqTypeRef && isEqObjRef && isEqToken;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hcTypeRef = TypeReference != null ? TypeReference.GetHashCode() : 0;
                var hcObjRef = VariableReference != null ? VariableReference.GetHashCode() : 0;
                var hcToken = Token.GetHashCode();
                return unchecked (3 + hcToken*397 + hcTypeRef*23846 + hcObjRef);
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