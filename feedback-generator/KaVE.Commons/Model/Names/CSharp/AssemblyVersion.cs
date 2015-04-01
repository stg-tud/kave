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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class AssemblyVersion : Name, IAssemblyVersion
    {
        private static readonly WeakNameCache<AssemblyVersion> Registry =
            WeakNameCache<AssemblyVersion>.Get(id => new AssemblyVersion(id));

        public new static IAssemblyVersion UnknownName
        {
            get { return Get(UnknownNameIdentifier); }
        }

        /// <summary>
        ///     Assembly version numbers have the format <code>'major'.'minor'.'build'.'revision'</code>.
        ///     Examples of assembly versions are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>4.0.0.0</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public new static AssemblyVersion Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private AssemblyVersion(string identifier)
            : base(identifier) {}

        private int[] Fragments
        {
            get
            {
                var identifier = (this == UnknownName) ? "-1.-1.-1.-1" : Identifier;
                return Array.ConvertAll(identifier.Split('.'), int.Parse);
            }
        }

        public int Major
        {
            get { return Fragments[0]; }
        }

        public int Minor
        {
            get { return Fragments[1]; }
        }

        public int Build
        {
            get { return Fragments[2]; }
        }

        public int Revision
        {
            get { return Fragments[3]; }
        }

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
    }
}