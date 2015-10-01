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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class MethodName : MemberName, IMethodName
    {
        private static readonly WeakNameCache<MethodName> Registry =
            WeakNameCache<MethodName>.Get(id => new MethodName(id));

        public new static IMethodName UnknownName
        {
            get { return Get("[?] [?].???()"); }
        }

        public override bool IsUnknown
        {
            get { return Equals(this, UnknownName); }
        }

        /// <summary>
        ///     Method type names follow the scheme
        ///     <code>'modifiers' ['return type name'] ['declaring type name'].'method name'('parameter names')</code>.
        ///     Examples of valid method names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>[System.Void, mscore, 4.0.0.0] [DeclaringType, AssemblyName, 1.2.3.4].MethodName()</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>static [System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].StaticMethod()</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>[System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].AMethod(opt [System.Int32, mscore, 4.0.0.0] length)</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>[System.String, mscore, 4.0.0.0] [System.String, mscore, 4.0.0.0]..ctor()</code>
        ///                 (Constructor)
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        [NotNull]
        public new static MethodName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private static readonly Regex SignatureSyntax =
            new Regex("\\]\\.((([^([]+)(?:`[0-9]+\\[[^(]+\\]){0,1})\\(.*\\))$");

        private MethodName(string identifier)
            : base(identifier) {}

        public override string Name
        {
            get { return SignatureSyntax.Match(Identifier).Groups[3].Value; }
        }

        private string FullName
        {
            get { return SignatureSyntax.Match(Identifier).Groups[2].Value; }
        }

        public IList<ITypeName> TypeParameters
        {
            get { return HasTypeParameters ? FullName.ParseTypeParameters() : new List<ITypeName>(); }
        }

        public bool HasTypeParameters
        {
            get { return FullName.Contains("[["); }
        }

        public bool IsGenericEntity
        {
            get { return HasTypeParameters; }
        }

        public IList<IParameterName> Parameters
        {
            get { return Identifier.GetParameterNames(); }
        }

        public bool HasParameters
        {
            get { return Identifier.HasParameters(); }
        }

        public bool IsConstructor
        {
            get { return Name.Equals(".ctor") || Name.Equals(".cctor"); }
        }

        public ITypeName ReturnType
        {
            get { return ValueType; }
        }

        public string Signature
        {
            get { return SignatureSyntax.Match(Identifier).Groups[1].Value; }
        }
    }
}