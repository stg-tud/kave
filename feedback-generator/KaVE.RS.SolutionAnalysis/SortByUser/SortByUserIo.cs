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
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.RS.SolutionAnalysis.SortByUser
{
    public interface ISortByUserIo
    {
        IDictionary<string, IKaVESet<string>> ScanArchivesForIdentifiers();
        void MergeArchives(IKaVESet<string> files);
    }

    public class SortByUserIo : ISortByUserIo
    {
        private readonly string _dirIn;
        private readonly string _dirOut;

        public SortByUserIo(string dirIn, string dirOut)
        {
            _dirIn = dirIn;
            _dirOut = dirOut;
        }

        public IDictionary<string, IKaVESet<string>> ScanArchivesForIdentifiers()
        {
            var allIds = new Dictionary<string, IKaVESet<string>>();

            foreach (var fileName in GetArchives())
            {
                var ids = Sets.NewHashSet<string>();

                foreach (var e in GetEventsFromArchive(fileName))
                {
                    var upe = e as UserProfileEvent;
                    if (upe != null)
                    {
                        var pid = upe.ProfileId.Trim();
                        if (!"".Equals(pid))
                        {
                            ids.Add("pid:" + pid);
                        }
                    }

                    if (e.IDESessionUUID != null)
                    {
                        var sid = e.IDESessionUUID.Trim();
                        if (!"".Equals(sid))
                        {
                            ids.Add("sid:" + sid);
                        }
                    }
                }

                allIds[fileName] = ids;
            }

            return allIds;
        }

        private IEnumerable<string> GetArchives()
        {
            return
                Directory.EnumerateFiles(_dirIn, "*.zip", SearchOption.AllDirectories)
                         .Select(f => f.Replace(_dirIn + @"\", ""));
        }

        public void MergeArchives(IKaVESet<string> files)
        {
            if (files.Count > 0)
            {
                var allEvents = files.SelectMany(GetEventsFromArchive);
                WriteEventsForNewUser(files.First(), allEvents);
            }
        }

        private IEnumerable<IDEEvent> GetEventsFromArchive(string file)
        {
            var fullPath = Path.Combine(_dirIn, file);
            var ra = new ReadingArchive(fullPath);
            while (ra.HasNext())
            {
                yield return ra.GetNext<IDEEvent>();
            }
        }

        private void WriteEventsForNewUser(string fileName, IEnumerable<IDEEvent> events)
        {
            var fullName = Path.Combine(_dirOut, fileName);
            var dir = Path.GetDirectoryName(fullName);
            if (dir != null)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            using (var wa = new WritingArchive(fullName))
            {
                foreach (var e in events)
                {
                    wa.Add(e);
                }
            }
        }
    }
}