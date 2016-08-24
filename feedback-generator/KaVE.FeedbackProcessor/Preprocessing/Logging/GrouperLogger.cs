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
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing.Logging
{
    public interface IGrouperLogger
    {
        void Init();
        void Zips(IDictionary<string, IKaVESet<string>> zipToIds);
        void Users(IKaVESet<User> users);
    }

    public class GrouperLogger : IGrouperLogger
    {
        private readonly IPrepocessingLogger _log;

        public GrouperLogger(IPrepocessingLogger log)
        {
            _log = log;
        }

        public void Init()
        {
            _log.Log();
            _log.Log(new string('#', 60));
            _log.Log("# identifying users");
            _log.Log(new string('#', 60));
        }

        public void Zips(IDictionary<string, IKaVESet<string>> zipToIds)
        {
            _log.Log();
            _log.Log("{0} zips as input:", zipToIds.Keys.Count);

            foreach (var zip in zipToIds.Keys)
            {
                _log.Log();
                _log.Log("#### zip: {0}", zip);
                _log.Log("ids: ");
                foreach (var id in zipToIds[zip])
                {
                    _log.Append("{0}, ", id);
                }
            }
        }

        public void Users(IKaVESet<User> users)
        {
            _log.Log();
            _log.Log(new string('-', 60));
            _log.Log();
            _log.Log("identified {0} users:", users.Count);

            var i = 0;
            foreach (var u in users)
            {
                _log.Log();
                _log.Log("#### user {0}", i++);

                _log.Log();
                _log.Log("Files:");
                _log.Log();
                foreach (var file in u.Files)
                {
                    _log.Append("{0}, ", file);
                }

                _log.Log();
                _log.Log("Identifier:");
                _log.Log();
                foreach (var id in u.Identifiers)
                {
                    _log.Append("{0}, ", id);
                }
            }
        }
    }
}