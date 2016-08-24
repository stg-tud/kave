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
    public interface IIdReader
    {
        IKaVESet<string> Read([NotNull] string zip);
    }

    public class IdReader : IIdReader
    {
        private readonly IIdReaderLogger _log;

        public IdReader(IIdReaderLogger log)
        {
            _log = log;
        }

        public IKaVESet<string> Read(string zip)
        {
            Asserts.NotNull(zip);
            Asserts.That(File.Exists(zip));
            Asserts.That(zip.EndsWith(".zip"));

            _log.Processing(zip);

            IKaVESet<string> ids;
            if (IsCached(zip))
            {
                _log.CacheHit();

                ids = ReadCache(zip);
            }
            else
            {
                _log.CacheMiss();

                ids = ReadZip(zip);
                WriteCache(ids, zip);
            }
            _log.FoundIds(ids);
            return ids;
        }

        private static bool IsCached(string zip)
        {
            var cacheFile = GetCacheFile(zip);
            return File.Exists(cacheFile);
        }

        private static string GetCacheFile(string zip)
        {
            return zip.Substring(0, zip.Length - 4) + ".ids";
        }

        private static void WriteCache(IKaVESet<string> ids, string zip)
        {
            var json = ids.ToFormattedJson();
            var cacheFile = GetCacheFile(zip);
            File.WriteAllText(cacheFile, json);
        }

        private static IKaVESet<string> ReadCache(string zip)
        {
            var cacheFile = GetCacheFile(zip);
            var json = File.ReadAllText(cacheFile);
            var ids = json.ParseJsonTo<KaVEHashSet<string>>();
            return ids;
        }

        private static IKaVESet<string> ReadZip(string zip)
        {
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
            return ids;
        }

        private static IEnumerable<IDEEvent> GetEventsFromArchive(string zip)
        {
            using (var ra = new ReadingArchive(zip))
            {
                while (ra.HasNext())
                {
                    yield return ra.GetNext<IDEEvent>();
                }
            }
        }
    }
}