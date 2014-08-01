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
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class MethodName : MemberName, IMethodName
    {
        private static readonly WeakNameCache<MethodName> Registry =
            WeakNameCache<MethodName>.Get(id => new MethodName(id));

        public static new IMethodName UnknownName {get { return Get("[?] [?].???()"); }}

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
        public new static MethodName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private MethodName(string identifier)
            : base(identifier) {}

        public override string Name
        {
            get
            {
                var fullName = FullName;
                var startOfTypeParameters = fullName.LastIndexOf('`');
                return startOfTypeParameters > -1 ? fullName.Substring(0, startOfTypeParameters) : FullName;
            }
        }

        private string FullName
        {
            get
            {
                var endIndexOfMethodName = Identifier.IndexOf('(');
                var startIndexOfMethodName =
                    Identifier.LastIndexOf("].", endIndexOfMethodName, StringComparison.Ordinal) + 2;
                var lengthOfMethodName = endIndexOfMethodName - startIndexOfMethodName;
                return Identifier.Substring(startIndexOfMethodName, lengthOfMethodName);
            }
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
            get
            {
                var parameters = new List<IParameterName>();
                if (HasParameters)
                {
                    var startOfParameterIdentifier = Identifier.IndexOf('(') + 1;
                    var endOfParameterList = Identifier.IndexOf(')');
                    do
                    {
                        var endOfParameterType = Identifier.IndexOf(
                            "] ",
                            startOfParameterIdentifier,
                            StringComparison.Ordinal);
                        var endOfParameterIdentifier = Identifier.IndexOf(',', endOfParameterType);
                        if (endOfParameterIdentifier < 0)
                        {
                            endOfParameterIdentifier = endOfParameterList;
                        }
                        var lengthOfParameterIdentifier = endOfParameterIdentifier - startOfParameterIdentifier;
                        var identifier =
                            Identifier.Substring(startOfParameterIdentifier, lengthOfParameterIdentifier).Trim();
                        parameters.Add(ParameterName.Get(identifier));
                        startOfParameterIdentifier = endOfParameterIdentifier + 1;
                    } while (startOfParameterIdentifier < endOfParameterList);
                }
                return parameters;
            }
        }

        public bool HasParameters
        {
            get { return !Identifier.Contains("()"); }
        }

        public bool IsConstructor
        {
            get { return Name.Equals(".ctor"); }
        }

        public ITypeName ReturnType
        {
            get { return ValueType; }
        }

        public string Signature
        {
            get
            {
                var startOfSignature = Identifier.IndexOf("].", StringComparison.Ordinal) + 2;
                return Identifier.Substring(startOfSignature);
            }
        }
    }
}