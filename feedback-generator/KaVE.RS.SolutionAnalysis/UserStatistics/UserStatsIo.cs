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
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.RS.SolutionAnalysis.UserStatistics
{
    class UserStatsIo
    {
        private readonly string _dirEvents;
        private readonly string _dirStats;

        public UserStatsIo(string dirEvents, string dirStats)
        {
            if (!dirEvents.EndsWith(@"\"))
            {
                dirEvents += @"\";
            }
            if (!dirStats.EndsWith(@"\"))
            {
                dirStats += @"\";
            }
            _dirEvents = dirEvents;
            _dirStats = dirStats;
        }

        public IEnumerable<string> FindZips()
        {
            var findCcZips = Directory.EnumerateFiles(_dirEvents, "*.zip", SearchOption.AllDirectories);
            var shortened = findCcZips.Select(z => z.Substring(_dirEvents.Length));
            return shortened;
        }

        public IEnumerable<IDEEvent> Read(string zip)
        {
            var fullPath = Path.Combine(_dirEvents, zip);
            var ra = new ReadingArchive(fullPath);
            while (ra.HasNext())
            {
                var e = ra.GetNext<IDEEvent>();
                if (e != null)
                {
                    yield return e;
                }
            }
        }

        public void WriteStats(ISet<UserStats> stats)
        {
            //new Directory
            using (var fs = File.Create(Path.Combine(_dirStats, "UserStats.json")))
            {
              //  JsonUtils.
            }
        }

        public ISet<UserStats> ReadStats()
        {
            throw new System.NotImplementedException();
        }
    }
}