/*
 * Copyright 2014 Technische Universitšt Darmstadt
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
    public class TryBlock : Statement
    {
        public readonly IList<Statement> Body = Lists.NewList<Statement>();
        public readonly IList<CatchBlock> CatchBlocks = Lists.NewList<CatchBlock>();
        public readonly IList<Statement> Finally = Lists.NewList<Statement>();

        private bool Equals(TryBlock other)
        {
            return Body.Equals(other.Body) && CatchBlocks.Equals(other.CatchBlocks) && Finally.Equals(other.Finally);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Body.GetHashCode();
                hashCode = (hashCode*397) ^ CatchBlocks.GetHashCode();
                hashCode = (hashCode*397) ^ Finally.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "try {{{0}}} {1} finally {{{2}}};",
                string.Join(" ", Body),
                string.Join(" ", CatchBlocks),
                string.Join(" ", Finally));
        }
    }
}