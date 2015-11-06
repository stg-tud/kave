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
    public class MethodDeclaration : IMethodDeclaration
    {
        private IMethodName _name;

        [DataMember]
        public IMethodName Name
        {
            get { return _name; }
            set
            {
                _name = value;

                MethodName = new SimpleName { Name = _name.Name };
                ReturnType = new TypeReference { TypeName = Name.ReturnType };
                DeclaringType = new TypeReference { TypeName = Name.DeclaringType };
                Parameters = CreateParameterDeclarationList(Name.Parameters);
                IsStatic = Name.IsStatic;
            }
        }

        [DataMember]
        public bool IsEntryPoint { get; set; }

        [DataMember]
        public IKaVEList<IStatement> Body { get; set; }

        public IEnumerable<ISSTNode> Children
        {
            get { return Lists.NewList<ISSTNode>(); }
        }

        public ITypeReference ReturnType { get;  set; }

        public ISimpleName MethodName { get;  set; }

        public ITypeReference DeclaringType { get;  set; }

        public IEnumerable<IParameterDeclaration> Parameters { get;  set; }

        public bool IsStatic { get;  set; }

        public MethodDeclaration()
        {
            Name = Names.CSharp.MethodName.UnknownName;
            Body = Lists.NewList<IStatement>();
        }

        private static IEnumerable<IParameterDeclaration> CreateParameterDeclarationList(IEnumerable<IParameterName> parameterNames)
        {
            var parameters = Lists.NewList<ParameterDeclaration>();
            foreach (var parameterName in parameterNames)
            {
                parameters.Add(
                    new ParameterDeclaration
                    {
                        Name = new SimpleName {Name = parameterName.Name},
                        Type = new TypeReference {TypeName = parameterName.ValueType},
                        IsOptional = parameterName.IsOptional,
                        IsOutput = parameterName.IsOutput,
                        IsParameterArray = parameterName.IsParameterArray,
                        IsPassedByReference = parameterName.IsPassedByReference,
                    });
            }
            return parameters;
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
                var hashCode = Body.GetHashCode();
                hashCode = (hashCode*397) ^ Name.GetHashCode();
                hashCode = (hashCode*397) ^ IsEntryPoint.GetHashCode();
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