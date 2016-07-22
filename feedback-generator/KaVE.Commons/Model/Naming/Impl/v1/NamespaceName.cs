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

using System.Text;
using Antlr4.Runtime.Misc;
using KaVE.Commons.Model.Naming.Impl.v1.Parser;
using KaVE.Commons.Model.Naming.Types.Organization;

namespace KaVE.Commons.Model.Naming.Impl.v1
{
    public class NamespaceName : INamespaceName
    {
        [NotNull]
        private readonly TypeNamingParser.NamespaceContext _ctx;

        public NamespaceName(TypeNamingParser.NamespaceContext ctx)
        {
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

        public INamespaceName ParentNamespace
        {
            get
            {
                var s = GetTextFromIdWithoutLast(_ctx.id());
                return Names.Namespace(s);
            }
        }

        private string GetTextFromIdWithoutLast(TypeNamingParser.IdContext[] id)
        {
            var s = new StringBuilder();
            for (int i = 0; i < id.Length; i++)
            {
                if (i == id.Length - 1)
                {
                    break;
                }
                s.Append(id[i].GetText());
                s.Append(".");
            }
            return s.ToString();
        }

        public string Name
        {
            get { return _ctx.id(_ctx.id().Length - 1).GetText(); }
        }

        public bool IsGlobalNamespace
        {
            get { return Identifier.Equals(""); }
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