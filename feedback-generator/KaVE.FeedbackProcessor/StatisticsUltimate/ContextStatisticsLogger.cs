/*
 * Copyright 2017 Sebastian Proksch
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

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public interface IContextStatisticsLogger : IStatisticsLogger
    {
        void Results(IContextStatistics contextStatistics);
    }

    public class ContextStatisticsLogger : StatisticsLoggerBase
    {
        public void Results(IContextStatistics stats)
        {
            lock (Lock)
            {
                Log("done!");

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("UniqueAssemblies.Count: {0}", stats.UniqueAssemblies.Count);
                Console.WriteLine("UniqueAsmMethods.Count: {0}", stats.UniqueAsmMethods.Count);
                Console.WriteLine("UniqueAsmFields.Count: {0}", stats.UniqueAsmFields.Count);
                Console.WriteLine("UniqueAsmProperties.Count: {0}", stats.UniqueAsmProperties.Count);
                Console.WriteLine();
                Console.WriteLine("(clearing sets now ...)");
                Console.WriteLine();

                stats.UniqueAssemblies.Clear();
                stats.UniqueAsmMethods.Clear();
                stats.UniqueAsmFields.Clear();
                stats.UniqueAsmProperties.Clear();

                Console.WriteLine(stats.ToString());
            }
        }
    }
}