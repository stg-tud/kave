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
using KaVE.Utils;

namespace KaVE.Model.SSTs.Blocks
{
    public class CaseBlock
    {
        public string Label { get; set; }
        public readonly IList<Statement> Body = Lists.NewList<Statement>();

        protected bool Equals(CaseBlock other)
        {
            return Equals(Body, other.Body) && string.Equals(Label, other.Label);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 30 + ((Body != null ? Body.GetHashCode() : 0)*397) ^ (Label != null ? Label.GetHashCode() : 0);
            }
        }
    }
}