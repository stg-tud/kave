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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using KaVE.Commons.Model.Naming.Types.Organization;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public interface IContextStatisticsLogger : IStatisticsLogger
    {
        void StartUp(IContextFilter cf);
        void Results(IContextStatistics contextStatistics, IDictionary<IAssemblyName, int> asmCounter);
    }

    public class ContextStatisticsLogger : StatisticsLoggerBase, IContextStatisticsLogger
    {
        public void StartUp(IContextFilter cf)
        {
            Log("Starting up with {0}", cf);
        }

        [SuppressMessage("ReSharper", "LocalizableElement")]
        public void Results(IContextStatistics stats, IDictionary<IAssemblyName, int> counts)
        {
            lock (Lock)
            {
                Log("done!");

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("UniqueTypeDecl.Count: {0}", stats.UniqueTypeDecl.Count);
                Console.WriteLine(
                    "UniqueMethodDeclsOverrideOrImplementAsm.Count: {0}",
                    stats.UniqueMethodDeclsOverrideOrImplementAsm.Count);
                Console.WriteLine("UniqueAssemblies.Count: {0}", stats.UniqueAssemblies.Count);
                Console.WriteLine("UniqueAsmMethods.Count: {0}", stats.UniqueAsmMethods.Count);
                Console.WriteLine("UniqueAsmFields.Count: {0}", stats.UniqueAsmFields.Count);
                Console.WriteLine("UniqueAsmProperties.Count: {0}", stats.UniqueAsmProperties.Count);
                Console.WriteLine();
                Console.WriteLine("(clearing sets now ...)");
                Console.WriteLine();

                stats.UniqueTypeDecl.Clear();
                stats.UniqueMethodDeclsOverrideOrImplementAsm.Clear();
                stats.UniqueAssemblies.Clear();
                stats.UniqueAsmMethods.Clear();
                stats.UniqueAsmFields.Clear();
                stats.UniqueAsmProperties.Clear();

                // getter-only is not printed.
                Console.WriteLine("NumTypeDeclTotal = {0}", stats.NumTypeDeclTotal);
                Console.WriteLine(
                    "NumInvocationExprTotal = {0}",
                    stats.NumUnknownInvocations + stats.NumValidInvocations);
                Console.WriteLine(stats.ToString());

                Console.WriteLine("\n## Histogram ##");
                for (var i = 1; i < 20; i++)
                {
                    LogAsms(counts, i);
                }
                for (var i = 20; i < 60; i += 2)
                {
                    LogAsms(counts, i);
                }
                for (var i = 60; i <= 150; i += 10)
                {
                    LogAsms(counts, i);
                }

                Console.WriteLine("\n## Assembly Occurrences ##");
                foreach (var asm in counts.OrderBy(p => p.Value).Reverse())
                {
                    Console.WriteLine("count({0}) = {1}", asm.Key, asm.Value);
                }
            }
        }

        private void LogAsms(IDictionary<IAssemblyName, int> counts, int minNum)
        {
            var count = counts.Keys.Count(k => counts[k] >= minNum);
            // ReSharper disable once LocalizableElement
            Console.WriteLine("min({0}) = {1}", minNum, count);
        }
    }
}