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
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.Naming.Impl.v0.CodeElements
{
    public abstract class MemberName : BaseName, IMemberName
    {
        protected const string UnknownMemberIdentifier = "[?] [?].???";
        public const string StaticModifier = "static";

        protected MemberName(string identifier) : base(identifier) {}

        protected string Modifiers
        {
            get { return Identifier.Substring(0, Identifier.IndexOf('[')); }
        }

        public bool IsStatic
        {
            get { return Modifiers.Contains(StaticModifier); }
        }

        public virtual string Name
        {
            get
            {
                var nameWithBraces = Identifier.Substring(Identifier.LastIndexOf('.') + 1);
                var endIdx = nameWithBraces.IndexOf('(');
                return endIdx != -1
                    ? nameWithBraces.Substring(0, endIdx)
                    : nameWithBraces;
            }
        }

        private string _fullName;

        public string FullName
        {
            get { return _fullName ?? (_fullName = GetFullName()); }
        }

        private string GetFullName()
        {
            var id = Identifier;
            var openR = id.IndexOf("[", StringComparison.Ordinal);
            var closeR = id.FindCorrespondingCloseBracket(openR);

            var openD = id.FindNext(closeR, '[');
            var closeD = id.FindCorrespondingCloseBracket(openD);

            var dot = id.FindNext(closeD, '.');
            var openP = id.FindNext(dot, '(', '`');
            if (openP != -1 && id[openP] == '`')
            {
                var openGen = id.FindNext(openP, '[');
                var closeGen = id.FindCorrespondingCloseBracket(openGen);
                openP = id.FindNext(closeGen, '(');
            }

            var start = dot + 1;
            var end = openP == -1 ? id.Length : openP;
            return id.Substring(start, end - start).Trim();
        }

        private ITypeName _valueType;

        public ITypeName ValueType
        {
            get
            {
                if (_valueType != null)
                {
                    return _valueType;
                }

                var openValType = Identifier.FindNext(0, '[');
                var closeValType = Identifier.FindCorrespondingCloseBracket(openValType);
                // ignore open bracket
                openValType++;
                var declTypeIdentifier = Identifier.Substring(openValType, closeValType - openValType);
                return _valueType = TypeUtils.CreateTypeName(declTypeIdentifier);
            }
        }

        private ITypeName _declaringType;

        public ITypeName DeclaringType
        {
            get
            {
                if (_declaringType != null)
                {
                    return _declaringType;
                }

                var openValType = Identifier.FindNext(0, '[');
                var closeValType = Identifier.FindCorrespondingCloseBracket(openValType);
                var openDeclType = Identifier.FindNext(closeValType, '[');
                var closeDeclType = Identifier.FindCorrespondingCloseBracket(openDeclType);
                // ignore open bracket
                openDeclType++;
                var declTypeIdentifier = Identifier.Substring(openDeclType, closeDeclType - openDeclType);
                return _declaringType = TypeUtils.CreateTypeName(declTypeIdentifier);
            }
        }
    }
}