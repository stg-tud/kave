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
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.CodeElements
{
    public class LambdaName : BaseName, ILambdaName
    {
        public LambdaName() : this(UnknownNameIdentifier) {}
        public LambdaName([NotNull] string identifier) : base(identifier) {}

        private IKaVEList<IParameterName> _parameters;

        public IKaVEList<IParameterName> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    if (IsUnknown)
                    {
                        _parameters = Lists.NewList<IParameterName>();
                    }
                    else
                    {
                        var endOfParameters = Identifier.LastIndexOf(')');
                        var startOfParameters = Identifier.FindCorrespondingOpenBracket(endOfParameters);
                        _parameters = Identifier.GetParameterNamesFromSignature(startOfParameters, endOfParameters);
                    }
                }
                return _parameters;
            }
        }

        public bool HasParameters
        {
            get { return Parameters.Count > 0; }
        }

        public ITypeName ReturnType
        {
            get
            {
                if (IsUnknown)
                {
                    return new TypeName();
                }
                var openR = Identifier.IndexOf('[');
                var closeR = Identifier.FindCorrespondingCloseBracket(openR);
                openR++; // skip bracket
                return TypeUtils.CreateTypeName(Identifier.Substring(openR, closeR - openR));
            }
        }

        public override bool IsUnknown
        {
            get { return UnknownNameIdentifier.Equals(Identifier); }
        }
    }
}