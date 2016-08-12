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

using KaVE.Commons.Utils.Exceptions;
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
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            Validate(identifier != null, "identifier must not be null");
            Identifier = identifier;
        }

        protected static void Validate(bool condition, string msg = "invalid identifier")
        {
            if (!condition)
            {
                throw new ValidationException(msg, null);
            }
        }

        public override bool Equals(object other)
        {
            var otherName = other as IName;
            return otherName != null && Equals(otherName);
        }

        private bool Equals(IName other)
        {
            return GetType() == other.GetType() && string.Equals(Identifier, other.Identifier);
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