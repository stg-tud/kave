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
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Collections;

namespace KaVE.RS.SolutionAnalysis.StatisticsForPapers
{
    public interface IStatisticsPrinter
    {
        void StartZip(string zipName, int cur, int total);
        void FoundUserKey(string userKey);
        void FoundKeysInZip(IKaVESet<string> zipKeys);
        void FoundKeys(IKaVESet<string> keys);
        void FoundUpes(IKaVEList<IUserProfileEvent> upes);
        void FoundUsers(IKaVESet<string> keys);
        void FoundAssignableZips(IKaVESet<string> zips);
    }

    public class StatisticsPrinter : IStatisticsPrinter
    {
        public void StartZip(string zipName, int cur, int total)
        {
            Console.WriteLine(@"#### {2}/{3}: {0} ({1}) ####", zipName, DateTime.Now, cur, total);
        }

        public void FoundUserKey(string userKey)
        {
            Console.WriteLine(@"user: {0}", userKey);
        }

        public void FoundKeysInZip(IKaVESet<string> zipKeys)
        {
            Console.WriteLine(@"keys: [");
            foreach (var key in zipKeys)
            {
                Console.WriteLine(@"  {0}", key);
            }
            Console.WriteLine(@"]");
            Console.WriteLine();
        }

        public void FoundKeys(IKaVESet<string> keys)
        {
            Console.WriteLine(@"#### keys:");
            foreach (var key in keys)
            {
                Console.WriteLine(key);
            }
            Console.WriteLine();
        }

        public void FoundUpes(IKaVEList<IUserProfileEvent> upes)
        {
            var total = upes.Count;
            var i = 1;
            Console.WriteLine(@"#### upes:");
            foreach (var key in upes)
            {
                Console.WriteLine(@"{0}/{1}: {2}", i++, total, key);
            }
            Console.WriteLine();
        }

        public void FoundUsers(IKaVESet<string> users)
        {
            Console.WriteLine(@"#### users:");
            foreach (var user in users)
            {
                Console.WriteLine(user);
            }
            Console.WriteLine();
        }

        public void FoundAssignableZips(IKaVESet<string> zips)
        {
            Console.WriteLine(@"#### found {0} assignable submissions: ####", zips.Count);
            foreach (var zip in zips)
            {
                Console.WriteLine(zip);
            }
            Console.WriteLine();
        }
    }
}