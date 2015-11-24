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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.RS.SolutionAnalysis.StatisticsForPapers
{
    public interface IStatisticsIo
    {
        IEnumerable<string> FindCcZips();
        IUserProfileEvent TryGetUserProfile(string zipName);
        IEnumerable<CompletionEvent> ReadCce(string zipName);
    }

    public class StatisticsIo : IStatisticsIo
    {
        private readonly string _dirCcEvents;
        private readonly string _dirAllEvents;

        public StatisticsIo(string dirCCEvents, string dirAllEvents)
        {
            _dirCcEvents = dirCCEvents;
            _dirAllEvents = dirAllEvents;
        }

        public IEnumerable<string> FindCcZips()
        {
            var findCcZips = Directory.EnumerateFiles(_dirCcEvents, "*.zip", SearchOption.AllDirectories);
            var shortened = findCcZips.Select(z => z.Substring(_dirCcEvents.Length));
            return shortened;
        }

        public IUserProfileEvent TryGetUserProfile(string zipName)
        {
            var fullPath = Path.Combine(_dirAllEvents, zipName);
            var ra = new ReadingArchive(fullPath);
            while (ra.HasNext())
            {
                var e = ra.GetNext<IDEEvent>() as IUserProfileEvent;
                if (e != null)
                {
                    return e;
                }
            }
            return null;
        }

        public IEnumerable<CompletionEvent> ReadCce(string zipName)
        {
            var fullPath = Path.Combine(_dirCcEvents, zipName);
            var ra = new ReadingArchive(fullPath);
            while (ra.HasNext())
            {
                var e = ra.GetNext<IDEEvent>() as CompletionEvent;
                if (e != null)
                {
                    yield return e;
                }
            }
        }
    }
}