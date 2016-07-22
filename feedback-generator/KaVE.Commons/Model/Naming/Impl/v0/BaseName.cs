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

using System.Text.RegularExpressions;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0
{
    public abstract class BaseName : IName
    {
        protected const string UnknownNameIdentifier = "???";

        public string Identifier { get; private set; }

        public abstract bool IsUnknown { get; }

        public bool IsHashed
        {
            // The hash of a substring of the name is the Base64-encoded MD5 hash of the original substring.
            // Base64 works by encoding groups of 3 bytes. If the length of the input string is not divisible
            // by three, Base64 will add padding bytes and add one '=' to the result string for each padded byte.
            // Since MD5 produces strings of 16 bytes, there will always be two '=' at the end of a hashed substring.
            // Because '==' can not be part of a valid unhashed name, we can then assume that a name containing '=='
            // has been hashed by the anonymizer.
            get { return Identifier.Contains("=="); }
        }

        protected BaseName([NotNull] string identifier)
        {
            Asserts.NotNull(identifier);
            Identifier = FixMissingGenericTicks(identifier);
        }

        private static readonly Regex MissingTicks = new Regex("\\+([a-zA-Z0-9]+)\\[\\[");

        private static string FixMissingGenericTicks(string id)
        {
            var match = MissingTicks.Match(id);
            if (!match.Success)
            {
                return id;
            }

            var type = string.Format("+{0}[[", match.Groups[1]);
            var newType = string.Format("+{0}`0[[", match.Groups[1]);
            var newId = id.Replace(type, newType);
            return FixMissingGenericTicks(newId);
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
            return Identifier != null ? Identifier.GetHashCode() : 0;
        }

        // TODO NameUpdate: remove operators (check for usages before)!
        public static bool operator ==(BaseName n1, IName n2)
        {
            return Equals(n1, n2);
        }

        public static bool operator !=(BaseName n1, IName n2)
        {
            return !(n1 == n2);
        }

        public override string ToString()
        {
            return Identifier;
        }
    }
}