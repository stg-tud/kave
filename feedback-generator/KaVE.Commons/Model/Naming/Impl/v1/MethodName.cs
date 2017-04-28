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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v1.Parser;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Naming.Impl.v1
{
    public class MethodName : IMethodName
    {
        protected TypeNamingParser.MethodContext Ctx;

        public MethodName(TypeNamingParser.MethodContext ctx)
        {
            Asserts.Null(ctx.UNKNOWN(), "ctx.UNKNOWN() != null");
            Ctx = ctx;
        }

        public string Signature
        {
            get
            {
                if (IsConstructor)
                {
                    return Ctx.regularMethod().nonStaticCtor() != null
                        ? ".ctor" + Ctx.regularMethod().methodSignature().GetText()
                        : ".cctor" + Ctx.regularMethod().methodSignature().GetText();
                }
                if (Ctx.regularMethod() != null)
                {
                    return Ctx.regularMethod().customMethod().methodDefinition().id().GetText() +
                           (Ctx.regularMethod().customMethod().genericTypePart() != null
                               ? Ctx.regularMethod().customMethod().genericTypePart().GetText()
                               : "") +
                           Ctx.regularMethod().methodSignature().GetText();
                }
                return Ctx.UNKNOWN().GetText();
            }
        }

        public IKaVEList<IParameterName> Parameters
        {
            get
            {
                IKaVEList<IParameterName> paras = new KaVEList<IParameterName>();
                if (HasParameters)
                {
                    foreach (var p in Ctx.regularMethod().methodSignature().formalParam())
                    {
                        // TODO the getText call seems to break the context concept here
                        paras.Add(Names.Parameter(p.GetText()));
                    }
                }
                return paras;
            }
        }

        public bool HasParameters
        {
            get { return Ctx.regularMethod() != null && Ctx.regularMethod().methodSignature() != null; }
        }

        public bool IsConstructor
        {
            get
            {
                return IsUnknown &&
                       (Ctx.regularMethod().staticCctor() != null || Ctx.regularMethod().nonStaticCtor() != null);
            }
        }

        public bool IsInit
        {
            get { return false; }
        }

        public ITypeName ReturnType
        {
            get
            {
                if (Ctx.regularMethod() != null && Ctx.regularMethod().customMethod() != null)
                {
                    if (Ctx.regularMethod().customMethod().methodDefinition().type()[0].UNKNOWN() == null)
                    {
                        return new TypeName(Ctx.regularMethod().customMethod().methodDefinition().type()[0]);
                    }
                }
                return UnknownName.Get(typeof(ITypeName));
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
                if (IsConstructor &&
                    ((Ctx.regularMethod().staticCctor() != null &&
                      Ctx.regularMethod().staticCctor().type().UNKNOWN() == null) ||
                     (Ctx.regularMethod().nonStaticCtor() != null &&
                      Ctx.regularMethod().nonStaticCtor().type().UNKNOWN() == null)))
                {
                    return
                        new TypeName(
                            Ctx.regularMethod().staticCctor() != null
                                ? Ctx.regularMethod().staticCctor().type()
                                : Ctx.regularMethod().nonStaticCtor().type());
                }
                if (Ctx.regularMethod().customMethod().methodDefinition().type(1).UNKNOWN() == null)
                {
                    return new TypeName(Ctx.regularMethod().customMethod().methodDefinition().type(1));
                }
                return UnknownName.Get(typeof(ITypeName));
            }
        }

        public ITypeName ValueType
        {
            get { return ReturnType; }
        }

        public bool IsStatic
        {
            get
            {
                if (IsConstructor)
                {
                    return Ctx.regularMethod().staticCctor() != null &&
                           Ctx.regularMethod().staticCctor().staticModifier() != null;
                }
                return !IsUnknown &&
                       (Ctx.regularMethod() != null && Ctx.regularMethod().customMethod().staticModifier() != null);
            }
        }

        public string Name
        {
            get
            {
                if (IsConstructor)
                {
                    return Ctx.regularMethod().staticCctor() != null ? ".cctor" : ".ctor";
                }
                return Ctx.regularMethod().customMethod().methodDefinition().id().GetText();
            }
        }

        public string FullName
        {
            get { throw new NotImplementedException(); }
        }

        public string Identifier
        {
            get { return Ctx.GetText(); }
        }

        public bool IsUnknown
        {
            get { return false; }
        }

        public bool IsHashed
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasTypeParameters
        {
            get
            {
                var isGenericEntity = !IsConstructor && !IsUnknown &&
                                      Ctx.regularMethod() != null && Ctx.regularMethod().customMethod() != null &&
                                      Ctx.regularMethod().customMethod().genericTypePart() != null;
                return isGenericEntity && Ctx.regularMethod().customMethod().genericTypePart().genericParam() != null;
            }
        }

        public IKaVEList<ITypeParameterName> TypeParameters
        {
            get
            {
                var typePara = Lists.NewList<ITypeParameterName>();
                if (HasTypeParameters)
                {
                    foreach (var p in Ctx.regularMethod().customMethod().genericTypePart().genericParam())
                    {
                        typePara.Add(Names.Type(p.typeParameter().GetText()).AsTypeParameterName);
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

        public bool IsDelegateInvocation { get; private set; }
    }
}