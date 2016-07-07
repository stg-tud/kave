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
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v0.CodeElements
{
    public class ParameterName : Name, IParameterName
    {
        public const string PassByReferenceModifier = "ref";
        public const string OutputModifier = "out";
        public const string VarArgsModifier = "params";
        public const string OptionalModifier = "opt";
        public const string ExtensionMethodModifier = "this";

        private static readonly WeakNameCache<ParameterName> Registry =
            WeakNameCache<ParameterName>.Get(id => new ParameterName(id));

        public new static IParameterName UnknownName
        {
            get { return Get("[?] ???"); }
        }

        public override bool IsUnknown
        {
            get { return Equals(this, UnknownName); }
        }

        /// <summary>
        ///     Parameter names follow the scheme <code>'modifiers' ['parameter type name'] 'parameter name'</code>.
        ///     Examples of parameter names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>[System.Int32, mscore, 4.0.0.0] size</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>out [System.Int32, mscore, 4.0.0.0] outputParameter</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>params [System.Int32, mscore, 4.0.0.0] varArgsParamter</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>ref [System.Int32, mscore, 4.0.0.0] referenceParameter</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>opt [System.Int32, mscore, 4.0.0.0] optionalParameter</code> (i.e., parameter with
        ///                 default value)
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public new static ParameterName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private ParameterName(string identifier)
            : base(identifier) {}

        public ITypeName ValueType
        {
            get
            {
                var startOfValueTypeIdentifier = Identifier.IndexOf('[') + 1;
                var endOfValueTypeIdentifier = Identifier.LastIndexOf(']');
                var lengthOfValueTypeIdentifier = endOfValueTypeIdentifier - startOfValueTypeIdentifier;
                return Names.Type(Identifier.Substring(startOfValueTypeIdentifier, lengthOfValueTypeIdentifier));
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
    }
}