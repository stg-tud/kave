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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    public class IdReader
    {
        private readonly IPreprocessingIo _io;

        public IdReader(IPreprocessingIo io)
        {
            _io = io;
        }

        public IKaVESet<string> Read(string zip)
        {
            var ids = Sets.NewHashSet<string>();

            foreach (var e in GetEventsFromArchive(zip))
            {
                var upe = e as UserProfileEvent;
                if (upe != null)
                {
                    var pid = upe.ProfileId.Trim();
                    if (!"".Equals(pid)) //TODO isNullOrEmpty
                    {
                        ids.Add("pid:" + pid);
                    }
                }

                if (e.IDESessionUUID != null)
                {
                    var sid = e.IDESessionUUID.Trim();
                    if (!"".Equals(sid)) //TODO isNullOrEmpty
                    {
                        ids.Add("sid:" + sid);
                    }
                }
            }
            return ids;
        }

        private IEnumerable<IDEEvent> GetEventsFromArchive(string zip)
        {
            var fullPath = _io.GetFullPath_Raw(zip);
            var ra = new ReadingArchive(fullPath);
            while (ra.HasNext())
            {
                yield return ra.GetNext<IDEEvent>();
            }
        }
    }
}