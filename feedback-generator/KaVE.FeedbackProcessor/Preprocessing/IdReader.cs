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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.Json;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    public class IdReader
    {
        private readonly IdReaderLogger _log;

        public IdReader(IdReaderLogger log)
        {
            _log = log;
        }

        public IKaVESet<string> Read([NotNull] string zip)
        {
            Asserts.NotNull(zip);
            Asserts.That(File.Exists(zip));
            Asserts.That(zip.EndsWith(".zip"));

            var cacheFile = zip.Substring(0, zip.Length - 4) + ".ids";
            if (File.Exists(cacheFile))
            {
                var json = File.ReadAllText(cacheFile);
                return json.ParseJsonTo<KaVEHashSet<string>>();
            }

            var ids = Sets.NewHashSet<string>();

            foreach (var e in GetEventsFromArchive(zip))
            {
                var upe = e as UserProfileEvent;
                if (upe != null && !string.IsNullOrEmpty(upe.ProfileId))
                {
                    var pid = upe.ProfileId.Trim();
                    if (!string.IsNullOrEmpty(pid))
                    {
                        ids.Add("pid:" + pid);
                    }
                }

                if (e.IDESessionUUID != null && !string.IsNullOrEmpty(e.IDESessionUUID))
                {
                    var sid = e.IDESessionUUID.Trim();
                    if (!string.IsNullOrEmpty(sid))
                    {
                        ids.Add("sid:" + sid);
                    }
                }
            }
            var jsonOut = ids.ToFormattedJson();
            File.WriteAllText(cacheFile, jsonOut);
            return ids;
        }

        private static IEnumerable<IDEEvent> GetEventsFromArchive(string zip)
        {
            var ra = new ReadingArchive(zip);
            while (ra.HasNext())
            {
                yield return ra.GetNext<IDEEvent>();
            }
        }
    }
}