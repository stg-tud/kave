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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using KaVE.FeedbackProcessor.Import;
using KaVE.FeedbackProcessor.Properties;

namespace KaVE.FeedbackProcessor
{
    public class SanityCheckApp
    {
        public void Run()
        {
            var fileLoader = new FeedbackArchiveReader();

            Dictionary<Type, int> counts = new Dictionary<Type, int>();

            var archives = OpenFeedbackArchives();
            Console.Write("processing archives... ");
            foreach (var archive in archives)
            {
                Console.Write('*');
                foreach (var evt in fileLoader.ReadAllEvents(archive))
                {
                    var type = evt.GetType();

                    if (!counts.ContainsKey(type))
                    {
                        counts[type] = 1;
                    }
                    else
                    {
                        counts[type]++;
                    }
                }
            }
            Console.WriteLine("done");
            Console.WriteLine();

            Console.WriteLine("counts:");
            Console.WriteLine("=======");
            foreach (var type in counts.Keys)
            {
                Console.WriteLine("- {0}: {1}x", type, counts[type]);
            }
        }


        private IEnumerable<ZipFile> OpenFeedbackArchives()
        {
            return Directory.GetFiles(Configuration.ImportDirectory, "*.zip").Select(ZipFile.Read);
        }
    }
}