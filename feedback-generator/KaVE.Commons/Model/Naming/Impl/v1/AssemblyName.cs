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
using KaVE.Commons.Model.Naming.Impl.v1.Parser;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Model.Naming.Impl.v1
{
    public class AssemblyName : IAssemblyName
    {
        private readonly TypeNamingParser.AssemblyContext _ctx;

        public AssemblyName(TypeNamingParser.AssemblyContext ctx)
        {
            Asserts.Null(ctx.UNKNOWN(), "ctx.UNKNOWN() != null");
            _ctx = ctx;
        }

        public IAssemblyVersion Version
        {
            get
            {
                if (_ctx.regularAssembly().assemblyVersion() != null)
                {
                    return new AssemblyVersion(_ctx.regularAssembly().assemblyVersion());
                }
                return UnknownName.Get(typeof(IAssemblyVersion));
            }
        }

        public string Name
        {
            get { return _ctx.regularAssembly().assemblyName().GetText(); }
        }

        public bool IsLocalProject
        {
            // TODO NameUpdate: Implement
            get { throw new NotImplementedException(); }
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