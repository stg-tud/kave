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

using KaVE.Model.Names;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Statements;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl
{
    public class Completion : ICompletionExpression, ICompletionStatement
    {
        public ITypeName TypeReference { get; set; }
        public IVariableReference ObjectReference { get; set; }
        public string Token { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(Completion other)
        {
            var isEqTypeRef = Equals(TypeReference, other.TypeReference);
            var isEqObjRef = Equals(ObjectReference, other.ObjectReference);
            var isEqToken = string.Equals(Token, other.Token);
            return isEqTypeRef && isEqObjRef && isEqToken;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hcTypeRef = TypeReference != null ? TypeReference.GetHashCode() : 0;
                var hcObjRef = ObjectReference != null ? ObjectReference.GetHashCode() : 0;
                var hcToken = Token != null ? Token.GetHashCode() : 0;
                return unchecked (3 + hcToken*397 + hcTypeRef*23846 + hcObjRef);
            }
        }

        public void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            visitor.Visit(this, context);
        }
    }
}