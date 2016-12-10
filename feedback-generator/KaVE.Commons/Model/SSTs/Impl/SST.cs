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
using System.Linq;
using System.Runtime.Serialization;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.SSTs.Impl
{
    [DataContract]
    public class SST : ISST
    {
        [DataMember]
        public ITypeName EnclosingType { get; set; }

        private string _partialClassIdentifier;

        [DataMember]
        public string PartialClassIdentifier
        {
            get { return _partialClassIdentifier; }
            set
            {
                // prevent setting empty string
                if (value != null && string.IsNullOrEmpty(value))
                {
                    return;
                }
                _partialClassIdentifier = value;
            }
        }

        public bool IsPartialClass
        {
            get { return !string.IsNullOrEmpty(PartialClassIdentifier); }
        }

        [DataMember]
        public IKaVESet<IFieldDeclaration> Fields { get; set; }

        [DataMember]
        public IKaVESet<IPropertyDeclaration> Properties { get; set; }

        [DataMember]
        public IKaVESet<IMethodDeclaration> Methods { get; set; }

        [DataMember]
        public IKaVESet<IEventDeclaration> Events { get; set; }

        [DataMember]
        public IKaVESet<IDelegateDeclaration> Delegates { get; set; }

        public IKaVESet<IMethodDeclaration> EntryPoints
        {
            get { return Sets.NewHashSetFrom(Methods.AsEnumerable().Where(m => m.IsEntryPoint)); }
        }

        public IKaVESet<IMethodDeclaration> NonEntryPoints
        {
            get { return Sets.NewHashSetFrom(Methods.AsEnumerable().Where(m => !m.IsEntryPoint)); }
        }

        public IEnumerable<ISSTNode> Children
        {
            get { return Lists.NewList<ISSTNode>(); }
        }

        public SST()
        {
            EnclosingType = Names.UnknownType;
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
            var areBothNonPartial = string.IsNullOrEmpty(PartialClassIdentifier) &&
                                    string.IsNullOrEmpty(other.PartialClassIdentifier);
            var pciEq = areBothNonPartial || string.Equals(PartialClassIdentifier, other.PartialClassIdentifier);
            return EnclosingType.Equals(other.EnclosingType) &&
                   pciEq && Fields.Equals(other.Fields) &&
                   Properties.Equals(other.Properties) && Methods.Equals(other.Methods) && Events.Equals(other.Events) &&
                   Delegates.Equals(other.Delegates);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EnclosingType.GetHashCode();
                if (!string.IsNullOrEmpty(PartialClassIdentifier))
                {
                    hashCode = (hashCode*397) ^ PartialClassIdentifier.GetHashCode();
                }
                hashCode = (hashCode*397) ^ Fields.GetHashCode();
                hashCode = (hashCode*397) ^ Properties.GetHashCode();
                hashCode = (hashCode*397) ^ Methods.GetHashCode();
                hashCode = (hashCode*397) ^ Events.GetHashCode();
                hashCode = (hashCode*397) ^ Delegates.GetHashCode();
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