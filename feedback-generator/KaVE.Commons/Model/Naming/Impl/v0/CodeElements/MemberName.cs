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
using System.Diagnostics.CodeAnalysis;
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
            get { return _fullName ?? (_fullName = GetFullName(Identifier)); }
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        private static string GetFullName(string id)
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

        public ITypeName ValueType
        {
            get
            {
                var openValType = Identifier.FindNext(0, '[');
                var closeValType = Identifier.FindCorrespondingCloseBracket(openValType);
                // ignore open bracket
                openValType++;
                var declTypeIdentifier = Identifier.Substring(openValType, closeValType - openValType);
                return TypeUtils.CreateTypeName(declTypeIdentifier);
            }
        }

        public ITypeName DeclaringType
        {
            get
            {
                var openValType = Identifier.FindNext(0, '[');
                var closeValType = Identifier.FindCorrespondingCloseBracket(openValType);
                var openDeclType = Identifier.FindNext(closeValType, '[');
                var closeDeclType = Identifier.FindCorrespondingCloseBracket(openDeclType);
                // ignore open bracket
                openDeclType++;
                var declTypeIdentifier = Identifier.Substring(openDeclType, closeDeclType - openDeclType);
                return TypeUtils.CreateTypeName(declTypeIdentifier);
            }
        }
    }
}