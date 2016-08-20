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

using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.Preprocessing.Model
{
    public class User
    {
        public IKaVESet<string> Identifiers { get; set; }
        public IKaVESet<string> Files { get; private set; }

        public User()
        {
            Identifiers = Sets.NewHashSet<string>();
            Files = Sets.NewHashSet<string>();
        }

        private bool Equals(User other)
        {
            return Equals(Identifiers, other.Identifiers) && Files.Equals(other.Files);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifiers != null ? Identifiers.GetHashCode() : 0)*397) ^
                       (Files != null ? Files.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}