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
    public class MemberName : IFieldName, IPropertyName, IEventName
    {
        private readonly TypeNamingParser.MemberNameContext _ctx;

        public MemberName(TypeNamingParser.MemberNameContext ctx)
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
            get { return _ctx.UNKNOWN() != null; }
        }

        public bool IsHashed
        {
            get { return Identifier.Contains("=="); }
        }

        public ITypeName DeclaringType
        {
            get
            {
                if (_ctx.simpleMemberName() != null)
                {
                    return GetDeclaringType(_ctx.simpleMemberName().methodDefinition());
                }
                if (_ctx.propertyName() != null)
                {
                    return GetDeclaringType(_ctx.propertyName().methodDefinition());
                }
                return UnknownName.Get(typeof(ITypeName));
            }
        }

        private ITypeName GetDeclaringType(TypeNamingParser.MethodDefinitionContext signature)
        {
            return new TypeName(signature.type(1));
        }

        public bool IsStatic
        {
            get
            {
                if (_ctx.simpleMemberName() != null)
                {
                    return _ctx.simpleMemberName().staticModifier() != null;
                }
                if (_ctx.propertyName() != null)
                {
                    return _ctx.propertyName().staticModifier() != null;
                }
                return false;
            }
        }

        public string Name
        {
            get
            {
                if (_ctx.simpleMemberName() != null)
                {
                    return GetName(_ctx.simpleMemberName().methodDefinition());
                }
                if (_ctx.propertyName() != null)
                {
                    return GetName(_ctx.propertyName().methodDefinition());
                }
                return UnknownName.Get(typeof(IMemberName)).FullName;
            }
        }

        public string FullName
        {
            get { throw new NotImplementedException(); }
        }

        private string GetName(TypeNamingParser.MethodDefinitionContext signature)
        {
            return signature.id().GetText();
        }

        public bool HasSetter
        {
            get
            {
                if (_ctx.propertyName() != null)
                {
                    return _ctx.propertyName().propertyModifier().GetText().Contains("set");
                }
                return false;
            }
        }

        public bool HasGetter
        {
            get
            {
                if (_ctx.propertyName() != null)
                {
                    return _ctx.propertyName().propertyModifier().GetText().Contains("get");
                }
                return false;
            }
        }

        public ITypeName ValueType
        {
            get
            {
                if (_ctx.simpleMemberName() != null)
                {
                    return GetValueType(_ctx.simpleMemberName().methodDefinition());
                }
                if (_ctx.propertyName() != null)
                {
                    return GetValueType(_ctx.propertyName().methodDefinition());
                }
                return UnknownName.Get(typeof(ITypeName));
            }
        }

        private ITypeName GetValueType(TypeNamingParser.MethodDefinitionContext signature)
        {
            return new TypeName(signature.type(0));
        }

        public ITypeName HandlerType
        {
            get { return ValueType; }
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

        public bool IsIndexer
        {
            get { return false; }
        }

        public bool HasParameters
        {
            get { return false; }
        }

        public IKaVEList<IParameterName> Parameters
        {
            get { return Lists.NewList<IParameterName>(); }
        }
    }
}