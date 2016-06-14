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

using System.Linq;
using System.Text;
using KaVE.Commons.Model.Names.CSharp.Parser;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class CsNamespaceName : IName, INamespaceName
    {
        private TypeNamingParser.NamespaceContext ctx;

        public CsNamespaceName(TypeNamingParser.NamespaceContext ctx)
        {
            this.ctx = ctx;
        }
        public string Identifier
        {
            get { return ctx != null ? ctx.GetText() : "?"; }
        }

        public bool IsUnknown
        {
            get { return ctx == null; }
        }

        public bool IsHashed
        {
            get
            {
                return Identifier.Contains("==");
            }
        }

        public INamespaceName ParentNamespace
        {
            get
            {
                if (!IsUnknown)
                {
                    var s = GetTextFromIdWithoutLast(ctx.id());
                    return CsNameUtil.ParseNamespaceName(s);
                }
                return null;
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
            get
            {
                return ctx != null ? ctx.id(ctx.id().Length-1).GetText() : "?";
            }
        }

        public bool IsGlobalNamespace
        {
            get { return Identifier.Equals(""); }
        }
    }
}