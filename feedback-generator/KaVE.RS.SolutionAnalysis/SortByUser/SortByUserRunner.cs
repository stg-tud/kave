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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.Json;

namespace KaVE.RS.SolutionAnalysis.SortByUser
{
    public class SortByUserRunner
    {
        private readonly string _sourceDir;
        private string _targetDir;

        public SortByUserRunner(string sourceDir, string targetDir)
        {
            _sourceDir = sourceDir;
            _targetDir = targetDir;
        }

        public void Run()
        {
            Console.WriteLine(@"=== Scanning archives ===");
            var archivesWithIdentifiers = ScanArchivesForIdentifiers(GetArchives(_sourceDir));

            var users = AssembleUsers(archivesWithIdentifiers).ToList();

            Console.WriteLine(@"=== Found users ===");
            foreach (var user in users)
            {
                Console.WriteLine(
                    @"{0}, {1} sessions: {2}",
                    user.Identifiers.UserProfileId,
                    user.Identifiers.SessionsIDs.Count,
                    string.Join(", ", user.Files.Select(f => f.Replace(_sourceDir, ""))));
            }

            Console.WriteLine();
            Console.WriteLine(@"=== Reassembling archives ===");
            foreach (var user in users)
            {
                Console.WriteLine();
                Console.WriteLine(
                    @"{0}: {1} ...",
                    user.Identifiers.UserProfileId,
                    string.Join(", ", user.Files.Select(f => f.Replace(_sourceDir, ""))));

                var allEvents = user.Files.SelectMany(GetEventsFromArchive).OrderBy(e => e.TriggeredAt);
                WriteEvents(allEvents, Path.Combine(_targetDir, user.Identifiers.GetUniqueName()));
            }
        }

        private void WriteEvents(IOrderedEnumerable<IDEEvent> events, string targetFile)
        {
            using (var fileStream = File.OpenWrite(targetFile))
            using (var zipFile = new ZipFile())
            {
                zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;
                var i = 0;
                foreach (var e in events)
                {
                    var fileName = (i++) + "-" + e.GetType().Name + ".json";
                    var json = e.ToFormattedJson();
                    zipFile.AddEntry(fileName, json);
                    Console.Write("\r{0} events ...   ", i);
                }
                zipFile.Save(fileStream);
            }
        }

        public static IEnumerable<User> AssembleUsers(Dictionary<string, UserIdentifiers> archivesWithIdentifiers)
        {
            var users = new List<User>();

            foreach (var archive in archivesWithIdentifiers)
            {
                var currentUser = new User {Files = {archive.Key}, Identifiers = archive.Value};
                var matches = FindMatchingEntries(users, archive.Value).ToList();

                if (matches.Any())
                {
                    foreach (var match in matches)
                    {
                        users.Remove(match);
                    }

                    matches.Add(currentUser);
                    var mergedUser = MergeUsers(matches);
                    users.Add(mergedUser);
                }
                else
                {
                    users.Add(currentUser);
                }
            }

            return users;
        }

        private static User MergeUsers(IEnumerable<User> usersToMerge)
        {
            var newUser = new User();

            foreach (var oldUser in usersToMerge)
            {
                newUser.Identifiers.UserProfileId = oldUser.Identifiers.UserProfileId;

                foreach (var sessionId in oldUser.Identifiers.SessionsIDs)
                {
                    newUser.Identifiers.SessionsIDs.Add(sessionId);
                }

                foreach (var file in oldUser.Files)
                {
                    newUser.Files.Add(file);
                }
            }

            return newUser;
        }

        private static IEnumerable<User> FindMatchingEntries(IEnumerable<User> users,
            UserIdentifiers currentArchiveIdentifiers)
        {
            return users.Where(
                user =>
                    (!string.IsNullOrEmpty(user.Identifiers.UserProfileId) &&
                     user.Identifiers.UserProfileId == currentArchiveIdentifiers.UserProfileId)
                    || user.Identifiers.SessionsIDs.Overlaps(currentArchiveIdentifiers.SessionsIDs));
        }

        private Dictionary<string, UserIdentifiers> ScanArchivesForIdentifiers(IEnumerable<string> getArchives)
        {
            var identifiers = new Dictionary<string, UserIdentifiers>();

            foreach (var archive in getArchives)
            {
                identifiers.Add(archive, new UserIdentifiers());

                foreach (var e in GetEventsFromArchive(archive))
                {
                    var upe = e as UserProfileEvent;
                    if (upe != null)
                    {
                        identifiers[archive].UserProfileId = upe.ProfileId;
                    }

                    identifiers[archive].SessionsIDs.Add(e.IDESessionUUID);
                }

                Console.WriteLine(archive);
                Console.WriteLine(
                    @"Profile: {0}; Sessions: {1}",
                    identifiers[archive].UserProfileId,
                    string.Join(", ", identifiers[archive].SessionsIDs));
                Console.WriteLine();
            }

            return identifiers;
        }

        private IEnumerable<string> GetArchives(string sourceDir)
        {
            return Directory.EnumerateFiles(sourceDir, "*.zip", SearchOption.AllDirectories);
        }

        private IEnumerable<IDEEvent> GetEventsFromArchive(string file)
        {
            var ra = new ReadingArchive(file);

            while (ra.HasNext())
            {
                yield return ra.GetNext<IDEEvent>();
            }
        }
    }
}