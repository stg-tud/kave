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
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.CodeElements
{
    public class ParameterName : BaseName, IParameterName
    {
        private const string UnknownParameterNameIdentifier = "[?] ???";

        public const string PassByReferenceModifier = "ref";
        public const string OutputModifier = "out";
        public const string VarArgsModifier = "params";
        public const string OptionalModifier = "opt";
        public const string ExtensionMethodModifier = "this";

        public ParameterName() : this(UnknownParameterNameIdentifier) {}

        public ParameterName([NotNull] string identifier) : base(identifier)
        {
            if (IsParameterArray)
            {
                Asserts.That(ValueType.IsArray);
            }
        }

        public ITypeName ValueType
        {
            get
            {
                var startOfValueTypeIdentifier = Identifier.IndexOf('[') + 1;
                var endOfValueTypeIdentifier = Identifier.LastIndexOf(']');
                var lengthOfValueTypeIdentifier = endOfValueTypeIdentifier - startOfValueTypeIdentifier;
                return
                    TypeUtils.CreateTypeName(
                        Identifier.Substring(startOfValueTypeIdentifier, lengthOfValueTypeIdentifier));
            }
        }

        public string Name
        {
            get { return Identifier.Substring(Identifier.LastIndexOf(' ') + 1); }
        }

        public bool IsPassedByReference
        {
            get { return ValueType.IsReferenceType || Modifiers.Contains(PassByReferenceModifier); }
        }

        private string Modifiers
        {
            get { return Identifier.Substring(0, Identifier.IndexOf('[')); }
        }

        public bool IsOutput
        {
            get { return Modifiers.Contains(OutputModifier); }
        }

        public bool IsParameterArray
        {
            get { return Modifiers.Contains(VarArgsModifier); }
        }

        public bool IsOptional
        {
            get { return Modifiers.Contains(OptionalModifier); }
        }

        public bool IsExtensionMethodParameter
        {
            get { return Modifiers.Contains(ExtensionMethodModifier); }
        }

        public override bool IsUnknown
        {
            get { return UnknownParameterNameIdentifier.Equals(Identifier); }
        }
    }
}