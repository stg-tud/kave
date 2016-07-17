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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;

namespace KaVE.Commons.Model.Naming.Impl.v0.CodeElements
{
    public class MethodName : MemberName, IMethodName
    {
        private const string UnknownMethodIdentifier = UnknownMemberIdentifier + "()";

        public MethodName() : base(UnknownMethodIdentifier) {}

        public MethodName(string identifier) : base(identifier) {}

        public override bool IsUnknown
        {
            get { return Equals(Identifier, UnknownMethodIdentifier); }
        }


        private static readonly Regex SignatureSyntax =
            new Regex("\\]\\.((([^([]+)(?:`[0-9]+\\[[^(]+\\]){0,1})\\(.*\\))$");

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
            get
            {
                try
                {
                    return Identifier.GetParameterNames();
                }
                catch (Exception e)
                {
                    // TODO improve handling in NameUtils
                    Console.WriteLine("Error getting params for '{0}', falling back to none", Identifier);
                    return new List<IParameterName>();
                }
            }
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

        public bool IsExtensionMethod
        {
            get { return IsStatic && Parameters.Count > 0 && Parameters[0].IsExtensionMethodParameter; }
        }
    }
}