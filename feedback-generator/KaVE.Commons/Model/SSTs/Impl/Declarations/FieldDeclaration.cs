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
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.SSTs.Impl.Declarations
{
    [DataContract]
    public class FieldDeclaration : IFieldDeclaration
    {
        [DataMember]
        public IFieldName Name {
            get { return this.CreateFieldName(); }
            set
            {
                ValueType = new TypeReference {TypeName = value.ValueType};
                FieldName = new SimpleName { Name = value.Name };
                DeclaringType = new TypeReference { TypeName = value.DeclaringType };
                IsStatic = value.IsStatic;
            } 
        }

        public ITypeReference ValueType { get; set; }
        public ISimpleName FieldName { get; set; }
        public ITypeReference DeclaringType { get; set; }
        public bool IsStatic { get; set; }

        public IEnumerable<ISSTNode> Children
        {
            get { return Lists.NewList<ISSTNode>(); }
        }

        public FieldDeclaration()
        {
            Name = Names.CSharp.FieldName.UnknownName;
        }

        private bool Equals(FieldDeclaration other)
        {
            return Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return unchecked(21 + Name.GetHashCode());
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