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

namespace KaVE.FeedbackProcessor.Preprocessing.Logging
{
    public interface IIdReaderLogger
    {
        void Processing(string zip);
        void CacheHit();
        void CacheMiss();
        void FoundIds(IEnumerable<string> ids);
    }

    public class IdReaderLogger : IIdReaderLogger
    {
        private readonly IPrepocessingLogger _log;

        public IdReaderLogger(IPrepocessingLogger log)
        {
            _log = log;

            _log.Log();
            _log.Log(new string('#', 60));
            _log.Log("# reading ids");
            _log.Log(new string('#', 60));
        }

        public void Processing(string zip)
        {
            _log.Log();
            _log.Log("#### {0}", zip);
        }

        public void CacheHit()
        {
            _log.Log("cache hit, reading cache...");
        }

        public void CacheMiss()
        {
            _log.Log("reading zip...");
        }

        public void FoundIds(IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                _log.Log("- {0}", id);
            }
        }
    }
}