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
using KaVE.Commons.Utils.Collections;

namespace KaVE.RS.SolutionAnalysis.SortByUser
{
    public interface ISortByUserLogger
    {
        void StartScanning();
        void FoundUsers(IKaVESet<User> users);
        void Reassembling();
        void UserResult(User user);
    }

    public class SortByUserLogger : ISortByUserLogger
    {
        public void FoundUsers(IKaVESet<User> users)
        {
            Console.WriteLine(@"=== Found users ===");
            foreach (var user in users)
            {
                Console.WriteLine(
                    @"{0}, {1} sessions: {2}",
                    user.Identifiers.Count,
                    string.Join(", ", user.Files));
            }
        }

        public void UserResult(User user)
        {
            Console.WriteLine();
            Console.WriteLine(
                @" {0} ...",
                string.Join(", ", user.Files));
        }

        public void Reassembling()
        {
            Console.WriteLine();
            Console.WriteLine(@"=== Reassembling archives ===");
        }

        public void StartScanning()
        {
            Console.WriteLine(@"=== Scanning archives ===");
        }
    }
}