﻿/*
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
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable
{
    [DataContract]
    public class CastExpression : ICastExpression
    {
        [DataMember]
        public ITypeName TargetType { get; set; }

        [DataMember]
        public IVariableReference VariableReference { get; set; }

        public IEnumerable<ISSTNode> Children
        {
            get { return Lists.NewList<ISSTNode>(); }
        }

        public CastExpression()
        {
            TargetType = TypeName.UnknownName;
            VariableReference = new VariableReference();
        }

        protected bool Equals(CastExpression other)
        {
            return TargetType.Equals(other.TargetType) && VariableReference.Equals(other.VariableReference);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return obj.GetType() == GetType() && Equals((CastExpression) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TargetType.GetHashCode()*397) ^ VariableReference.GetHashCode();
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