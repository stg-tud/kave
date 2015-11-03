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
using KaVE.RS.SolutionAnalysis.CompletionEventToMicroCommits;

namespace KaVE.RS.SolutionAnalysis.UserProfileExports
{
    public class UserProfileExportRunner
    {
        private readonly IIoHelper _io;
        private readonly UserProfileExportHelper _helper;
        private IKaVEList<UserProfileEvent> _profiles;
        private ISet<string> _days;

        public UserProfileExportRunner(IIoHelper io, UserProfileExportHelper helper)
        {
            _io = io;
            _helper = helper;
        }

        public void Export(string rootDir)
        {
            var exports = _io.FindExports();

            _profiles = Lists.NewList<UserProfileEvent>();
            _days = new HashSet<string>();

            _helper.LogFoundExports(exports.Count);

            var fileCounter = 0;
            foreach (var exportZip in exports)
            {
                _helper.LogOpenExport(exportZip);

                var hasUserProfile = false;
                foreach (var up in _io.ReadEvents(exportZip))
                {
                    RegisterKey(fileCounter, up);

                    var userProfileEvent = up as UserProfileEvent;
                    if (userProfileEvent != null)
                    {
                        hasUserProfile = true;
                        _profiles.Add(userProfileEvent);
                    }
                }

                _helper.LogResult(hasUserProfile);
                fileCounter++;
            }

            _helper.LogNumberDays(fileCounter, _days.Count);
            _helper.LogUserProfiles(_profiles);
        }

        private void RegisterKey(int fileCounter, IDEEvent up)
        {
            if (up.TriggeredAt.HasValue)
            {
                var day = up.TriggeredAt.Value.ToString("ddMMyy");
                var idx = string.Format("{0}-{1}", fileCounter, day);
                _days.Add(idx);
            }
        }
    }
}