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
using System.Linq;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    public class Grouper
    {
        public IKaVESet<IKaVESet<string>> GroupRelatedZips(IDictionary<string, IKaVESet<string>> zipToIds)
        {
            var users = AssembleUsers(zipToIds);
            return Sets.NewHashSetFrom(users.Select(u => u.Files));
        }

        private static IKaVESet<User> AssembleUsers(IDictionary<string, IKaVESet<string>> archivesWithIdentifiers)
        {
            var users = Sets.NewHashSet<User>();

            foreach (var archive in archivesWithIdentifiers)
            {
                var currentUser = new User {Files = {archive.Key}, Identifiers = archive.Value};
                var matches = FindMatchingEntries(users, archive.Value);

                if (matches.Any())
                {
                    foreach (var match in matches)
                    {
                        users.Remove(match);
                    }
                    matches.Add(currentUser);
                    currentUser = MergeUsers(matches);
                }

                users.Add(currentUser);
            }

            return users;
        }

        private static User MergeUsers(IEnumerable<User> usersToMerge)
        {
            var newUser = new User();

            foreach (var oldUser in usersToMerge)
            {
                newUser.Files.AddAll(oldUser.Files);
                newUser.Identifiers.AddAll(oldUser.Identifiers);
            }

            return newUser;
        }

        private static ISet<User> FindMatchingEntries(IEnumerable<User> users,
            IKaVESet<string> currentArchiveIdentifiers)
        {
            var matches = users.Where(
                user =>
                    (user.Identifiers.Overlaps(currentArchiveIdentifiers)));
            return Sets.NewHashSetFrom(matches);
        }
    }
}