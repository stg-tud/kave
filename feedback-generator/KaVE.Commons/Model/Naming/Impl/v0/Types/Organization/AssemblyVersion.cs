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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types.Organization
{
    public class AssemblyVersion : BaseName, IAssemblyVersion
    {
        private readonly Regex _isValidVersionRegex = new Regex("^-?\\d+\\.-?\\d+\\.-?\\d+\\.-?\\d+$");

        public AssemblyVersion() : this(UnknownNameIdentifier) {}

        public AssemblyVersion([NotNull] string identifier) : base(identifier)
        {
            if (!UnknownNameIdentifier.Equals(identifier))
            {
                if (!_isValidVersionRegex.IsMatch(identifier))
                {
                    throw new ValidationException("invalid assembly version '{0}'".FormatEx(identifier), null);
                }
            }
        }

        private int[] _fragments;

        private int[] Fragments
        {
            get { return _fragments ?? (_fragments = Array.ConvertAll(Identifier.Split('.'), int.Parse)); }
        }

        public int Major
        {
            get { return IsUnknown ? -1 : Fragments[0]; }
        }

        public int Minor
        {
            get { return IsUnknown ? -1 : Fragments[1]; }
        }

        public int Build
        {
            get { return IsUnknown ? -1 : Fragments[2]; }
        }

        public int Revision
        {
            get { return IsUnknown ? -1 : Fragments[3]; }
        }

        // TODO NameUpdate: What about the custom ordering operators for AsmVersions?
        public int CompareTo(IAssemblyVersion other)
        {
            var otherVersion = other as AssemblyVersion;
            if (otherVersion == null)
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

        public static bool operator <(AssemblyVersion v1, AssemblyVersion v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator >(AssemblyVersion v1, AssemblyVersion v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <=(AssemblyVersion v1, AssemblyVersion v2)
        {
            return v1.CompareTo(v2) <= 0;
        }

        public static bool operator >=(AssemblyVersion v1, AssemblyVersion v2)
        {
            return v1.CompareTo(v2) >= 0;
        }

        public override bool IsUnknown
        {
            get { return UnknownNameIdentifier.Equals(Identifier); }
        }
    }
}