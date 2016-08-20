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

namespace KaVE.FeedbackProcessor.SortByUser
{
    public class SortByUserRunner
    {
        private readonly ISortByUserIo _io;
        private readonly ISortByUserLogger _log;

        public SortByUserRunner(ISortByUserIo io, ISortByUserLogger log)
        {
            _io = io;
            _log = log;
        }

        public void Run()
        {
            _log.StartScanning();
            var archivesWithIdentifiers = _io.ScanArchivesForIdentifiers();

            _log.StartUserIdentification();

            var users = AssembleUsers(archivesWithIdentifiers);

            _log.FoundUsers(users);

            _log.StartMerging();

            var numBefore = 0;
            var numAfter = 0;
            foreach (var user in users)
            {
                _log.StartProcessingUser(user);
                numAfter++;
                numBefore += user.Files.Count;

                _io.MergeArchives(user.Files);
            }

            _log.FinalStats(numBefore, numAfter);
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