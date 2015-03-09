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
using System.Linq;
using System.Runtime.Serialization;
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl
{
    [DataContract]
    public class SST : ISST
    {
        [DataMember]
        public ITypeName EnclosingType { get; set; }

        [DataMember]
        public ISet<IFieldDeclaration> Fields { get; set; }

        [DataMember]
        public ISet<IPropertyDeclaration> Properties { get; set; }

        [DataMember]
        public ISet<IMethodDeclaration> Methods { get; set; }

        [DataMember]
        public ISet<IEventDeclaration> Events { get; set; }

        [DataMember]
        public ISet<IDelegateDeclaration> Delegates { get; set; }

        public ISet<IMethodDeclaration> EntryPoints
        {
            get { return Sets.NewHashSetFrom(Methods.AsEnumerable().Where(m => m.IsEntryPoint)); }
        }

        public ISet<IMethodDeclaration> NonEntryPoints
        {
            get { return Sets.NewHashSetFrom(Methods.AsEnumerable().Where(m => !m.IsEntryPoint)); }
        }

        public SST()
        {
            EnclosingType = TypeName.UnknownName;
            Fields = Sets.NewHashSet<IFieldDeclaration>();
            Properties = Sets.NewHashSet<IPropertyDeclaration>();
            Methods = Sets.NewHashSet<IMethodDeclaration>();
            Events = Sets.NewHashSet<IEventDeclaration>();
            Delegates = Sets.NewHashSet<IDelegateDeclaration>();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(SST other)
        {
            return Equals(EnclosingType, other.EnclosingType) && Equals(Fields, other.Fields) &&
                   Equals(Methods, other.Methods) && Equals(Properties, other.Properties) &&
                   Equals(Events, other.Events) && Equals(Delegates, other.Delegates);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (EnclosingType != null ? EnclosingType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Fields != null ? Fields.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Methods != null ? Methods.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Properties != null ? Properties.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Events != null ? Events.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Delegates != null ? Delegates.GetHashCode() : 0);
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