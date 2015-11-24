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

using System;
using System.Linq;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Collections;

namespace KaVE.RS.SolutionAnalysis.StatisticsForPapers
{
    internal class StatisticsForPaperRunner
    {
        private readonly IStatisticsIo _io;
        private readonly IStatisticsPrinter _printer;
        private IKaVEList<IUserProfileEvent> _upes;
        private IKaVESet<string> _keys;
        private IKaVESet<string> _assignableSubmissions;

        public StatisticsForPaperRunner(IStatisticsIo io, IStatisticsPrinter printer)
        {
            _io = io;
            _printer = printer;
        }

        public void Run()
        {
            _upes = Lists.NewList<IUserProfileEvent>();
            var users = Sets.NewHashSet<string>();
            _keys = Sets.NewHashSet<string>();
            _assignableSubmissions = Sets.NewHashSet<string>();

            var zips = _io.FindCcZips().ToList();

            var cur = 1;
            var total = zips.Count;
            foreach (var zipName in zips)
            {
                _printer.StartZip(zipName, cur++, total);

                var userKey = GetUserKey(zipName);
                users.Add(userKey);
                _printer.FoundUserKey(userKey);

                var zipKeys = GetKeysFrom(zipName);
                _printer.FoundKeysInZip(zipKeys);
                foreach (var key in zipKeys)
                {
                    var combKey = string.Format("{0}\t{1}", key, userKey);
                    _keys.Add(combKey);
                }
            }

            _printer.FoundUsers(users);
            _printer.FoundKeys(_keys);
            _printer.FoundUpes(_upes);
            _printer.FoundAssignableZips(_assignableSubmissions);
        }

        private string GetUserKey(string zipName)
        {
            var upe = _io.TryGetUserProfile(zipName);
            if (upe == null)
            {
                return "AUTO_" + zipName;
            }
            _upes.Add(upe);
            _assignableSubmissions.Add(zipName);
            return upe.ProfileId;
        }

        private IKaVESet<string> GetKeysFrom(string zipName)
        {
            IKaVESet<string> keys = Sets.NewHashSet<string>();

            foreach (var cce in _io.ReadCce(zipName))
            {
                var date = cce.TriggeredAt ?? DateTime.MinValue;
                var dateStr = string.Format("{0:0000}{1:00}{2:00}", date.Year, date.Month, date.Day);
                keys.Add(dateStr);
            }

            return keys;
        }
    }
}