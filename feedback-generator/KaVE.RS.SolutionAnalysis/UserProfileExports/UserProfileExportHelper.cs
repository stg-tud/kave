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
using KaVE.Commons.Utils.Json;

namespace KaVE.RS.SolutionAnalysis.UserProfileExports
{
    public class UserProfileExportHelper
    {
        private int _current;
        private int _count;

        public virtual void LogFoundExports(int count)
        {
            _count = count;
        }

        public virtual void LogOpenExport(string exportZip)
        {
            Console.Write(@"{3} | {0}/{1}: {2}... ", ++_current, _count, exportZip, DateTime.Now);
        }

        public virtual void LogResult(bool hasProfile)
        {
            Console.WriteLine(hasProfile ? "has profile" : "-");
        }

        public virtual void LogUserProfiles(IKaVEList<UserProfileEvent> profiles)
        {
            foreach (var profile in profiles)
            {
                Console.WriteLine(@"----------------------------");
                Console.WriteLine(profile.ToFormattedJson());
            }
        }

        public virtual void LogNumberDays(int fileCounter, int count)
        {
            Console.Write(@"found data in {0} files, covering {1} workdays", fileCounter, count);
        }
    }
}