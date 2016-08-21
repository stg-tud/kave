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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    public class GroupMerger
    {
        private readonly IPreprocessingIo _io;

        public GroupMerger(IPreprocessingIo io)
        {
            _io = io;
        }

        public void Merge(IKaVESet<string> relZips)
        {
            if (relZips.Count > 0)
            {
                var zipOut = _io.GetFullPath_Merged(relZips.First());
                _io.EnsureParentExists(zipOut);

                using (var wa = new WritingArchive(zipOut))
                {
                    foreach (var e in ReadArchives(relZips))
                    {
                        wa.Add(e);
                    }
                }
            }
        }

        private IEnumerable<IDEEvent> ReadArchives(IEnumerable<string> relZips)
        {
            foreach (var relZip in relZips)
            {
                var zip = _io.GetFullPath_Raw(relZip);
                var ra = new ReadingArchive(zip);
                while (ra.HasNext())
                {
                    yield return ra.GetNext<IDEEvent>();
                }
            }
        }
    }
}