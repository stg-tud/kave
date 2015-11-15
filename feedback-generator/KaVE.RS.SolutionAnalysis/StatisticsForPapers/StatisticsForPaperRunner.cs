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

        public StatisticsForPaperRunner(IStatisticsIo io, IStatisticsPrinter printer)
        {
            _io = io;
            _printer = printer;
        }

        public void Run()
        {
            _upes = Lists.NewList<IUserProfileEvent>();
            _keys = Sets.NewHashSet<string>();

            foreach (var zipName in _io.FindCcZips())
            {
                _printer.StartZip(zipName);

                var userKey = GetUserKey(zipName);
                _printer.FoundUserKey(userKey);

                var zipKeys = GetKeysFrom(zipName);
                _printer.FoundKeysInZip(zipKeys);
                foreach (var key in zipKeys)
                {
                    var combKey = string.Format("{0}\t{1}", key, userKey);
                    _keys.Add(combKey);
                }
            }

            _printer.FoundKeys(_keys);
            _printer.FoundUpes(_upes);
        }

        private string GetUserKey(string zipName)
        {
            var upe = _io.TryGetUserProfile(zipName);
            if (upe == null)
            {
                return "AUTO_" + zipName;
            }
            _upes.Add(upe);
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