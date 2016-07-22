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
using KaVE.Commons.Model.Naming.Impl.v1.Parser;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v1
{
    public class LambdaName : ILambdaName
    {
        private readonly TypeNamingParser.LambdaNameContext _ctx;

        public LambdaName(TypeNamingParser.LambdaNameContext ctx)
        {
            Asserts.Null(ctx.UNKNOWN(), "ctx.UNKNOWN() != null");
            _ctx = ctx;
        }

        public string Identifier
        {
            get { return _ctx.GetText(); }
        }

        public bool IsUnknown
        {
            get { return false; }
        }

        public bool IsHashed
        {
            get { return Identifier.Contains("=="); }
        }

        public IKaVEList<IParameterName> Parameters
        {
            get
            {
                var parameters = Lists.NewList<IParameterName>();
                if (_ctx.realLambdaName().methodSignature() != null)
                {
                    foreach (var p in _ctx.realLambdaName().methodSignature().formalParam())
                    {
                        parameters.Add(Names.Parameter(p.GetText()));
                    }
                }
                return parameters;
            }
        }

        public bool HasParameters
        {
            get
            {
                return _ctx.realLambdaName() != null && _ctx.realLambdaName().methodSignature() != null &&
                       _ctx.realLambdaName().methodSignature().formalParam().Length != 0;
            }
        }

        public ITypeName ReturnType
        {
            get { return new TypeName(_ctx.realLambdaName().type()); }
        }

        public override bool Equals(object other)
        {
            var otherName = other as IName;
            return otherName != null && Equals(otherName);
        }

        private bool Equals(IName other)
        {
            return string.Equals(Identifier, other.Identifier);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }
}