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
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.CodeElements
{
    public class MethodName : MemberName, IMethodName
    {
        private const string UnknownMethodIdentifier = UnknownMemberIdentifier + "()";

        public MethodName() : this(UnknownMethodIdentifier) {}

        public MethodName([NotNull] string identifier) : base(identifier)
        {
            if (IsConstructor)
            {
                Asserts.That(ReturnType.IsVoidType);
            }
        }

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

        private string _fullName;

        private string FullName
        {
            get { return _fullName ?? (_fullName = GetFullNameFromMethod(Identifier)); }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static string GetFullNameFromMethod(string id)
        {
            var openRT = id.IndexOf("[", StringComparison.Ordinal);
            var closeRT = id.FindCorrespondingCloseBracket(openRT);

            var openDT = id.FindNext(closeRT, '[');
            var closeDT = id.FindCorrespondingCloseBracket(openDT);

            var dot = id.FindNext(closeDT, '.');

            var nextGeneric = id.FindNext(dot, '`');
            if (nextGeneric == -1)
            {
                nextGeneric = id.Length;
            }
            var nextParam = id.FindNext(dot, '(');
            var isGeneric = nextGeneric < nextParam;

            int openParams;
            if (isGeneric)
            {
                var openGeneric = id.FindNext(dot, '[');
                var closeGeneric = id.FindCorrespondingCloseBracket(openGeneric);

                openParams = id.FindNext(closeGeneric, '(');
            }
            else
            {
                openParams = id.FindNext(dot, '(');
            }
            int afterDot = dot + 1;
            var fullName = id.Substring(afterDot, openParams - afterDot).Trim();
            return fullName;
        }

        public IKaVEList<ITypeParameterName> TypeParameters
        {
            get { return HasTypeParameters ? FullName.ParseTypeParameters() : Lists.NewList<ITypeParameterName>(); }
        }

        public bool HasTypeParameters
        {
            get { return FullName.Contains("[["); }
        }

        public bool IsGenericEntity
        {
            get { return HasTypeParameters; }
        }

        private IKaVEList<IParameterName> _parameters;

        public IList<IParameterName> Parameters
        {
            get { return _parameters ?? (_parameters = this.GetParameterNamesFromMethod()); }
        }

        public bool HasParameters
        {
            get { return Parameters.Count > 0; }
        }

        public bool IsConstructor
        {
            get { return Name.Equals(".ctor") || Name.Equals(".cctor"); }
        }

        public ITypeName ReturnType
        {
            // TODO NameUpdate: Technically, this is not correct! the value type of a method is a delegate
            get { return ValueType; }
        }

        public bool IsExtensionMethod
        {
            get { return IsStatic && Parameters.Count > 0 && Parameters[0].IsExtensionMethodParameter; }
        }
    }
}