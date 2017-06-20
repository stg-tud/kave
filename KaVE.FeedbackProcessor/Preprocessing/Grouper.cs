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
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    public interface IGrouper
    {
        IKaVESet<IKaVESet<string>> GroupRelatedZips(IDictionary<string, IKaVESet<string>> zipToIds);
    }

    public class Grouper : IGrouper
    {
        private readonly IGrouperLogger _log;

        public Grouper(IGrouperLogger log)
        {
            _log = log;
        }

        public IKaVESet<IKaVESet<string>> GroupRelatedZips(IDictionary<string, IKaVESet<string>> zipToIds)
        {
            _log.Init();
            _log.Zips(zipToIds);
            var users = AssembleUsers(zipToIds);
            _log.Users(users);
            return Sets.NewHashSetFrom(users.Select(u => u.Files));
        }

        private static IKaVESet<User> AssembleUsers(IDictionary<string, IKaVESet<string>> zipToIds)
        {
            var users = Sets.NewHashSet<User>();

            var sortedKeysToPreserveOrdering = zipToIds.Keys.Reverse();
            foreach (var zip in sortedKeysToPreserveOrdering)
            {
                var ids = Sets.NewHashSetFrom(zipToIds[zip]); // clone
                var currentUser = new User
                {
                    Files = {zip},
                    Identifiers = ids
                };
                var matches = FindMatchingEntries(users, ids);

                if (matches.Any())
                {
                    foreach (var match in matches)
                    {
                        users.Remove(match);
                        currentUser.Files.AddAll(match.Files);
                        currentUser.Identifiers.AddAll(match.Identifiers);
                    }
                }

                users.Add(currentUser);
            }

            return users;
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