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

using System;
using System.Collections.Generic;
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Declarations
{
    public class MethodDeclaration : MemberDeclaration
    {
        public IMethodName Name { get; set; }
        public readonly IList<Statement> Body = Lists.NewList<Statement>();
        public Boolean IsEntryPoint { get; set; }

        public override void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            visitor.Visit(this, context);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(MethodDeclaration other)
        {
            return Equals(Body, other.Body) && Equals(Name, other.Name) && IsEntryPoint.Equals(other.IsEntryPoint);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Body != null ? Body.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ IsEntryPoint.GetHashCode();
                return hashCode;
            }
        }
    }
}