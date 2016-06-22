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

namespace KaVE.Commons.Model.Names.CSharp
{
    public class CsAssemblyName : IName, IAssemblyName
    {
        private TypeNamingParser.AssemblyContext ctx;

        public CsAssemblyName(TypeNamingParser.AssemblyContext ctx) 
        {
            this.ctx = ctx;
        }

        public IAssemblyVersion AssemblyVersion
        {
            get
            {
                if (ctx.regularAssembly().assemblyVersion() != null)
                {
                    return new CsAssemblyVersion(ctx.regularAssembly().assemblyVersion());
                }
                return new UnknownName();
            }
        }

        public string Name
        {
            get { return ctx.regularAssembly().id(0).GetText(); }
        }

        public string Identifier
        {
            get { return ctx.GetText(); }
        }

        public bool IsUnknown
        {
            get { return ctx.UNKNOWN() != null; }
        }

        public bool IsHashed
        {
            get
            {
                return Identifier.Contains("==");
            }
        }
    }
}