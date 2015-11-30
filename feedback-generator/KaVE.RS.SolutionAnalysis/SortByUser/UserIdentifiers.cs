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

using System.Collections.Generic;

namespace KaVE.RS.SolutionAnalysis.SortByUser
{
    public class UserIdentifiers
    {
        public ISet<string> SessionsIDs { get; private set; }
        public string UserProfileId { get; set; }

        public UserIdentifiers()
        {
            SessionsIDs = new HashSet<string>();
        }

        protected bool Equals(UserIdentifiers other)
        {
            return SessionsIDs.SetEquals(other.SessionsIDs) && string.Equals(UserProfileId, other.UserProfileId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((UserIdentifiers) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((SessionsIDs != null ? SessionsIDs.GetHashCode() : 0)*397) ^
                       (UserProfileId != null ? UserProfileId.GetHashCode() : 0);
            }
        }

        public string GetUniqueName()
        {
            return !string.IsNullOrEmpty(UserProfileId)
                ? UserProfileId
                : string.Format("NoUserProfileID-{0}", GetHashCode());
        }
    }
}