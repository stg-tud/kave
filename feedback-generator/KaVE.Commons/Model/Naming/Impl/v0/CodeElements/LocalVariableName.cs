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

using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.CodeElements
{
    public class LocalVariableName : BaseName, ILocalVariableName
    {
        private const string UnknownLocalVariableName = "[?] ???";

        public LocalVariableName() : this(UnknownLocalVariableName) {}
        public LocalVariableName([NotNull] string identifier) : base(identifier) {}

        public string Name
        {
            get
            {
                var indexOfName = Identifier.LastIndexOf(']') + 1;
                return Identifier.Substring(indexOfName).Trim();
            }
        }

        public ITypeName ValueType
        {
            get
            {
                var lengthOfTypeIdentifier = Identifier.LastIndexOf(']') - 1;
                return TypeUtils.CreateTypeName(Identifier.Substring(1, lengthOfTypeIdentifier));
            }
        }

        public override bool IsUnknown
        {
            get { return UnknownLocalVariableName.Equals(Identifier); }
        }
    }
}