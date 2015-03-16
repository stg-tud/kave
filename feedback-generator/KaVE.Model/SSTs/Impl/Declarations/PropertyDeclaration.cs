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

using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl.Declarations
{
    public class PropertyDeclaration : IPropertyDeclaration
    {
        public IPropertyName Name { get; set; }
        public IKaVEList<IStatement> Get { get; set; }
        public IKaVEList<IStatement> Set { get; set; }

        public PropertyDeclaration()
        {
            Name = PropertyName.UnknownName;
            Get = Lists.NewList<IStatement>();
            Set = Lists.NewList<IStatement>();
        }

        private bool Equals(PropertyDeclaration other)
        {
            return Equals(Get, other.Get) && Equals(Set, other.Set) && Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Get.GetHashCode();
                hashCode = (hashCode*397) ^ Set.GetHashCode();
                hashCode = (hashCode*397) ^ Name.GetHashCode();
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