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

using System.Runtime.Serialization;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.SSTs.Impl.Blocks
{
    [DataContract]
    public class CaseBlock : ICaseBlock
    {
        [DataMember]
        public ISimpleExpression Label { get; set; }

        [DataMember]
        public IKaVEList<IStatement> Body { get; set; }

        public CaseBlock()
        {
            Label = new UnknownExpression();
            Body = Lists.NewList<IStatement>();
        }

        private bool Equals(CaseBlock other)
        {
            return Equals(Body, other.Body) && Equals(Label, other.Label);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 30 + (Body.GetHashCode()*397) ^ Label.GetHashCode();
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}