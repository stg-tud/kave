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
using KaVE.Commons.Model.Names.CSharp.Parser;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Names.CSharp
{
    class CsMethodName : IMethodName
    {
        protected TypeNamingParser.MethodContext ctx;

        public CsMethodName(TypeNamingParser.MethodContext ctx)
        {
            Asserts.Null(ctx.UNKNOWN(), "ctx.UNKNOWN() != null");
            this.ctx = ctx;
        }

        public string Signature
        {
            get
            {
                if (IsConstructor)
                {
                    return ctx.regularMethod().nonStaticCtor() != null
                        ? ".ctor" + ctx.regularMethod().methodSignature().GetText()
                        : ".cctor" + ctx.regularMethod().methodSignature().GetText();
                }
                if (ctx.regularMethod() != null)
                {
                    return ctx.regularMethod().customMethod().methodDefinition().id().GetText() + (ctx.regularMethod().customMethod().genericTypePart() != null ? ctx.regularMethod().customMethod().genericTypePart().GetText() : "") +
                           ctx.regularMethod().methodSignature().GetText();
                }
                return ctx.UNKNOWN().GetText();
            }
        }

        public IList<IParameterName> Parameters
        {
            get
            {
                IList<IParameterName> paras = new KaVEList<IParameterName>();
                if (HasParameters)
                {
                    foreach (var p in ctx.regularMethod().methodSignature().formalParam())
                    {
                        paras.Add(ParameterName.Get(p.GetText()));
                    }
                }
                return paras;
            }
        }

        public bool HasParameters
        {
            get { return ctx.regularMethod() != null && ctx.regularMethod().methodSignature() != null; }
        }

        public bool IsConstructor
        {
            get
            {
                return IsUnknown &&
                       (ctx.regularMethod().staticCctor() != null || ctx.regularMethod().nonStaticCtor() != null);
            }
        }

        public ITypeName ReturnType
        {
            get
            {
                if (ctx.regularMethod() != null && ctx.regularMethod().customMethod() != null)
                {
                    return new CsTypeName(ctx.regularMethod().customMethod().methodDefinition().type()[0]);
                }
                return CsNameUtil.GetTypeName("?");
            }
        }

        public bool IsExtensionMethod
        {
            get
            {
                if (HasParameters)
                {
                    foreach (var p in Parameters)
                    {
                        if (p.IsExtensionMethodParameter)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public ITypeName DeclaringType
        {
            get
            {
                if (IsConstructor)
                {
                    return new CsTypeName(ctx.regularMethod().staticCctor() != null ? ctx.regularMethod().staticCctor().type() : ctx.regularMethod().nonStaticCtor().type());
                }
                return new CsTypeName(ctx.regularMethod().customMethod().methodDefinition().type(1));
            }
        }

        public bool IsStatic
        {
            get
            {
                if (IsConstructor)
                {
                    return ctx.regularMethod().staticCctor() != null && ctx.regularMethod().staticCctor().staticModifier() != null;
                }
                return !IsUnknown && (ctx.regularMethod() != null && ctx.regularMethod().customMethod().staticModifier() != null);
            }
        }

        public string Name
        {
            get
            {
                if (IsConstructor)
                {
                    return ctx.regularMethod().staticCctor() != null ? ".cctor" : ".ctor";
                }
                return ctx.regularMethod().customMethod().methodDefinition().id().GetText();
            }
        }

        public string Identifier
        {
            get { return ctx.GetText(); }
        }

        public bool IsUnknown
        {
            get { return false; }
        }

        public bool IsHashed
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsGenericEntity
        {
            get
            {
                return !IsConstructor && !IsUnknown &&
                       ctx.regularMethod() != null && ctx.regularMethod().customMethod() != null &&
                        ctx.regularMethod().customMethod().genericTypePart() != null;
            }
        }

        public bool HasTypeParameters
        {
            get
            {
                return IsGenericEntity && ctx.regularMethod().customMethod().genericTypePart().genericParam() != null;
            }
        }

        public IList<ITypeName> TypeParameters
        {
            get
            {
                List<ITypeName> typePara = new KaVEList<ITypeName>();
                if (HasTypeParameters)
                {
                    foreach (var p in ctx.regularMethod().customMethod().genericTypePart().genericParam())
                    {
                        typePara.Add(new TypeParameterName(p.GetText()));
                    }
                }
                return typePara;
            }
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
