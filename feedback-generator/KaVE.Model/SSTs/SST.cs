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
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Statements;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs
{
    public class SST : ISSTNode
    {
        public ITypeName EnclosingType { get; set; }
        public ISet<FieldDeclaration> Fields { get; set; }
        public ISet<PropertyDeclaration> Properties { get; set; }
        public ISet<MethodDeclaration> Methods { get; set; }
        public ISet<EventDeclaration> Events { get; set; }
        public ISet<DelegateDeclaration> Delegates { get; set; }
        public CompletionTrigger TypeLevelTrigger { get; set; }

        public SST()
        {
            Fields = Sets.NewHashSet<FieldDeclaration>();
            Properties = Sets.NewHashSet<PropertyDeclaration>();
            Methods = Sets.NewHashSet<MethodDeclaration>();
            Events = Sets.NewHashSet<EventDeclaration>();
            Delegates = Sets.NewHashSet<DelegateDeclaration>();
        }

        public ISet<MethodDeclaration> EntryPoints
        {
            get { return Sets.NewHashSetFrom(Methods.AsEnumerable().Where(m => m.IsEntryPoint)); }
        }

        public ISet<MethodDeclaration> NonEntryPoints
        {
            get { return Sets.NewHashSetFrom(Methods.AsEnumerable().Where(m => !m.IsEntryPoint)); }
        }

        public void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            visitor.Visit(this, context);
        }


        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(SST other)
        {
            return Equals(EnclosingType, other.EnclosingType) && Equals(Fields, other.Fields) &&
                   Equals(Methods, other.Methods) && Equals(Properties, other.Properties) &&
                   Equals(Events, other.Events) && Equals(Delegates, other.Delegates) &&
                   Equals(TypeLevelTrigger, other.TypeLevelTrigger);
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
                hashCode = (hashCode*397) ^ (TypeLevelTrigger != null ? TypeLevelTrigger.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}