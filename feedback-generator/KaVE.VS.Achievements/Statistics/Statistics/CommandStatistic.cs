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
using KaVE.VS.Achievements.UI.StatisticUI;
using KaVE.VS.Achievements.Util.ToStringFormatting.StatisticFormatting;

namespace KaVE.VS.Achievements.Statistics.Statistics
{
    // data structure for Command Statistics
    public class CommandStatistic : Statistic<CommandStatistic>
    {
        public Dictionary<string, int> CommandTypeValues;

        public CommandStatistic()
        {
            CommandTypeValues = new Dictionary<string, int>();
        }

        private static bool IsNamedCommand(string commandType)
        {
            return RemovePrefixFromCommandId(commandType) != "";
        }

        private static string RemovePrefixFromCommandId(string commandId)
        {
            var index = commandId.LastIndexOf(':');
            return index == -1 ? commandId : commandId.Substring(index + 1);
        }

        /// <summary>
        ///     Gets a List of StatisticElements containing each statistic of this data structure
        /// </summary>
        public override List<StatisticElement> GetCollection()
        {
            return (from commandTypeValue in CommandTypeValues
                where IsNamedCommand(commandTypeValue.Key)
                select new StatisticElement
                {
                    Name = RemovePrefixFromCommandId(commandTypeValue.Key),
                    Value = commandTypeValue.Value.Format()
                }).ToList();
        }
    }
}