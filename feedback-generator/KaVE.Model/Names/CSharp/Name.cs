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

using System.Runtime.Serialization;
using KaVE.Utils.Collections;

namespace KaVE.Model.Names.CSharp
{
    [DataContract]
    public class Name : IName
    {
        protected const string UnknownNameIdentifier = "???";

        private static readonly WeakReferenceDictionary<string, Name> NameRegistry =
            new WeakReferenceDictionary<string, Name>();

        public static IName UnknownName
        {
            get { return Get(UnknownNameIdentifier); }
        }

        public virtual bool IsUnknown
        {
            get { return Equals(this, UnknownName); }
        }

        public static Name Get(string identifier)
        {
            if (!NameRegistry.ContainsKey(identifier))
            {
                NameRegistry.Add(identifier, new Name(identifier));
            }
            return NameRegistry[identifier];
        }

        [DataMember(Order = 1)]
        public string Identifier { get; private set; }

        protected Name(string identifier)
        {
            Identifier = identifier;
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

        public static bool operator ==(Name n1, IName n2)
        {
            return Equals(n1, n2);
        }

        public static bool operator !=(Name n1, IName n2)
        {
            return !(n1 == n2);
        }

        public override string ToString()
        {
            return Identifier;
        }
    }
}