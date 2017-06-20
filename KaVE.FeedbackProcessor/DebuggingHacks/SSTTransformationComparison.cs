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
using System.Diagnostics.CodeAnalysis;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.DebuggingHacks
{
    internal class SSTTransformationComparison
    {
        private readonly IPreprocessingIo _ioA;
        private readonly IPreprocessingIo _ioB;

        public SSTTransformationComparison(IPreprocessingIo ioA, IPreprocessingIo ioB)
        {
            _ioB = ioB;
            _ioA = ioA;
        }

        [SuppressMessage("ReSharper", "LocalizableElement")]
        public void Run()
        {
            Console.WriteLine("Comparing two folder of contexts:");
            Console.WriteLine("A: {0}", _ioA.GetFullPath_In(""));
            Console.WriteLine("B: {0}", _ioB.GetFullPath_In(""));

            Console.WriteLine("Reading A ...");
            var zipsA = _ioA.FindRelativeZipPaths();
            Console.WriteLine("Found {0} zips.", zipsA.Count);


            Console.WriteLine("Reading B ...");
            var zipsB = _ioB.FindRelativeZipPaths();
            Console.WriteLine("Found {0} zips.", zipsB.Count);

            var notInB = Sets.NewHashSet<string>();
            foreach (var zip in zipsA)
            {
                if (!zipsB.Contains(zip))
                {
                    notInB.Add(zip);
                }
            }

            var notInA = Sets.NewHashSet<string>();
            foreach (var zip in zipsB)
            {
                if (!zipsA.Contains(zip))
                {
                    notInA.Add(zip);
                }
            }

            Console.WriteLine("== {0} zips in common ==", zipsA.Count - notInB.Count);

            Console.WriteLine("== {0} zips not in A ==", notInA.Count);
            foreach (var zip in notInA)
            {
                Console.WriteLine("* {0}", zip);
            }

            Console.WriteLine("== {0} zips not in B ==", notInB.Count);
            foreach (var zip in notInB)
            {
                Console.WriteLine("* {0}", zip);
            }


            var gainedSize = Sets.NewHashSet<string>();
            Console.WriteLine("== reduced size ==");
            foreach (var zip in zipsA)
            {
                if (!notInB.Contains(zip))
                {
                    var sizeA = _ioA.GetSize_In(zip);
                    var sizeB = _ioB.GetSize_In(zip);
                    var delta = (sizeB / (double) sizeA - 1) * 100;
                    var deltaAbs = Math.Abs(delta);
                    if (deltaAbs >= 0.1) // 0.001 or 0.1%
                    {
                        var sign = delta > 0 ? '+' : '-';
                        var str = "* {0}  ({1}{2:0.0}%)".FormatEx(zip, sign, deltaAbs);

                        if (delta < 0)
                        {
                            Console.WriteLine(str);
                        }

                        if (delta > 0)
                        {
                            gainedSize.Add(str);
                        }
                    }
                }
            }

            Console.WriteLine("== gained size ==");
            foreach (var str in gainedSize)
            {
                Console.WriteLine(str);
            }
        }
    }
}