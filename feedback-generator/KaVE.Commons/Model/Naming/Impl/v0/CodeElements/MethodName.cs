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

using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;
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
                // TODO NameUpdate: write fix and reenable check (+test, +repair).
                //Asserts.That(ReturnType.IsVoidType);
                //if (".cctor".Equals(Name))
                //{
                //Asserts.That(IsStatic);
                //}
                // else: Asserts.Not(IsStatic);
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

        private IKaVEList<ITypeParameterName> _typeParameters;

        public IKaVEList<ITypeParameterName> TypeParameters
        {
            get
            {
                if (_typeParameters == null)
                {
                    if (FullName.Contains("`"))
                    {
                        var start = FullName.IndexOf('[');
                        var end = FullName.LastIndexOf(']');
                        _typeParameters = FullName.ParseTypeParameterList(start, end);
                    }
                    else
                    {
                        _typeParameters = Lists.NewList<ITypeParameterName>();
                    }
                }

                return _typeParameters;
            }
        }

        public bool HasTypeParameters
        {
            get { return TypeParameters.Count > 0; }
        }

        private IKaVEList<IParameterName> _parameters;

        public IKaVEList<IParameterName> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    var endOfParameters = Identifier.LastIndexOf(')');
                    var startOfParameters = Identifier.FindCorrespondingOpenBracket(endOfParameters);
                    _parameters = Identifier.GetParameterNamesFromSignature(startOfParameters, endOfParameters);
                }

                return _parameters;
            }
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