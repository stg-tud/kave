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
using KaVE.Commons.Model.Names.CSharp.Parser;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class CsAssemblyVersion : IName, IAssemblyVersion
    {
        private TypeNamingParser.AssemblyVersionContext ctx;

        public CsAssemblyVersion(TypeNamingParser.AssemblyVersionContext ctx)
        {
            this.ctx = ctx;
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
            get { return Identifier.Contains("=="); }
        }

        public int CompareTo(IAssemblyVersion other)
        {
            var otherVersion = other as AssemblyVersion;
            if (otherVersion == null || other.IsUnknown || IsUnknown)
            {
                return int.MinValue;
            }
            var majorDiff = Major - otherVersion.Major;
            if (majorDiff != 0)
            {
                return majorDiff;
            }
            var minorDiff = Minor - otherVersion.Minor;
            if (minorDiff != 0)
            {
                return minorDiff;
            }
            var buildDiff = Build - otherVersion.Build;
            if (buildDiff != 0)
            {
                return buildDiff;
            }
            return Revision - otherVersion.Revision;
        }

        public int Major
        {
            get { return Int32.Parse(ctx.num(0).GetText()); }
        }

        public int Minor { get { return Int32.Parse(ctx.num(1).GetText()); } }
        public int Build { get { return Int32.Parse(ctx.num(2).GetText()); } }
        public int Revision { get { return Int32.Parse(ctx.num(3).GetText()); } }

        public override bool Equals(object other)
        {
            var otherName = other as IName;
            return otherName != null && string.Equals(Identifier, otherName.Identifier);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }
}