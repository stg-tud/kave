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
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.SolutionAnalysis.CompletionEventToMicroCommits;

namespace KaVE.RS.SolutionAnalysis.UserProfileExports
{
    public class UserProfileExportRunner
    {
        private readonly IIoHelper _io;
        private readonly UserProfileExportHelper _helper;

        public UserProfileExportRunner(IIoHelper io, UserProfileExportHelper helper)
        {
            _io = io;
            _helper = helper;
        }

        public void Export(string rootDir)
        {
            var exports = _io.FindExports();
            var ups = Lists.NewList<UserProfileEvent>();

            _helper.LogFoundExports(exports.Count);

            foreach (var exportZip in exports)
            {
                _helper.LogOpenExport(exportZip);

                var hasUserProfile = false;
                var count = 0;
                foreach (var up in ReadUserProfile(exportZip))
                {
                    Asserts.That(count++ <= 1);
                    hasUserProfile = true;
                    ups.Add(up);
                }

                _helper.LogResult(hasUserProfile);
            }

            _helper.LogUserProfiles(ups);
        }

        private IEnumerable<UserProfileEvent> ReadUserProfile(string exportZip)
        {
            return _io.ReadEvents(exportZip).OfType<UserProfileEvent>();
        }
    }
}