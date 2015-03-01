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
using KaVE.Model.Collections;
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Visitor;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Impl.Blocks
{
    public class SwitchBlock : ISwitchBlock
    {
        public string Identifier { get; set; }
        public IList<CaseBlock> Sections { get; set; }
        public IList<IStatement> DefaultSection { get; set; }

        public SwitchBlock()
        {
            Sections = Lists.NewList<CaseBlock>();
            DefaultSection = Lists.NewList<IStatement>();
        }

        protected bool Equals(SwitchBlock other)
        {
            return Equals(Sections, other.Sections) && Equals(DefaultSection, other.DefaultSection) &&
                   string.Equals(Identifier, other.Identifier);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Sections != null ? Sections.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (DefaultSection != null ? DefaultSection.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Identifier != null ? Identifier.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void Accept<TContext>(ISSTNodeVisitor<TContext> visitor, TContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}