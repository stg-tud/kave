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
using KaVE.Model.SSTs.Declarations;
using KaVE.Utils;

namespace KaVE.Model.SSTs.Blocks
{
    public class ForEachLoop : Statement
    {
        public VariableDeclaration Decl { get; set; }
        public string LoopedIdentifier { get; set; }
        public readonly IList<Statement> Body = Lists.NewList<Statement>();

        private bool Equals(ForEachLoop other)
        {
            return Body.Equals(other.Body) && Equals(Decl, other.Decl) && Equals(LoopedIdentifier, other.LoopedIdentifier);
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
                hashCode = (hashCode * 397) ^ (Decl != null ? Decl.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LoopedIdentifier != null ? LoopedIdentifier.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format(
                "foreach ({0} in {1}) {{{2}}};",
                Decl.ToString().Replace(";", ""),
                LoopedIdentifier,
                string.Join(" ", Body.Select(s => s.ToString())));
        }
    }
}