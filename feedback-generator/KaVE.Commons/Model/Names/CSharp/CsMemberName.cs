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

using KaVE.Commons.Model.Names.CSharp.Parser;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class CsMemberName : IMemberName, IName, IFieldName, IPropertyName, IEventName
    {
        private readonly TypeNamingParser.MemberNameContext _ctx;

        public CsMemberName(TypeNamingParser.MemberNameContext ctx)
        {
            this._ctx = ctx;
        }

        public string Identifier
        {
            get { return _ctx.GetText(); }
        }

        public bool IsUnknown
        {
            get { return _ctx.UNKNOWN() != null; }
        }

        public bool IsHashed { get { return Identifier.Contains("=="); } }

        public ITypeName DeclaringType
        {
            get
            {
                if (_ctx.simpleMemberName() != null)
                {
                    return GetDeclaringType(_ctx.simpleMemberName().signature());
                }
                else if (_ctx.propertyName() != null)
                {
                    return GetDeclaringType(_ctx.propertyName().signature());
                }
                return new UnknownName();
            }
        }

        private ITypeName GetDeclaringType(TypeNamingParser.SignatureContext signature)
        {
            return new CsTypeName(signature.type(1));
        }

        public bool IsStatic
        {
            get
            {
                if (_ctx.simpleMemberName() != null)
                {
                    return _ctx.simpleMemberName().staticModifier() != null;
                }
                else if (_ctx.propertyName() != null)
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
                    return GetName(_ctx.simpleMemberName().signature());
                }
                else if (_ctx.propertyName() != null)
                {
                    return GetName(_ctx.propertyName().signature());
                }
                return new UnknownName().Identifier;
            }
        }

        private string GetName(TypeNamingParser.SignatureContext signature)
        {
            return signature.id().GetText();
        }

        public bool HasSetter
        {
            get
            {
                if (_ctx.propertyName() != null)
                {
                    return _ctx.propertyName().propertyModifier().GetText() == "set";
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
                    return _ctx.propertyName().propertyModifier().GetText() == "get";
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
                    return GetValueType(_ctx.simpleMemberName().signature());
                }
                else if (_ctx.propertyName() != null)
                {
                    return GetValueType(_ctx.propertyName().signature());
                }
                return new UnknownName();
            }
        }

        private ITypeName GetValueType(TypeNamingParser.SignatureContext signature)
        {
            return new CsTypeName(signature.type(0));
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
    }
}