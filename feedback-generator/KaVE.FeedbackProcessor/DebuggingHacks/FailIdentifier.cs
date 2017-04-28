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
using System.IO;
using System.Linq;
using System.Text;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Json;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.DebuggingHacks
{
    public class FailIdentifier
    {
        [SuppressMessage("ReSharper", "LocalizableElement")]
        public void Run()
        {
            var io = new PreprocessingIo(@"F:\Contexts\", @"F:\Tmp\", @"F:\Tmp\");
            var zips = io.FindRelativeZipPaths();
            var zipSlns = Sets.NewHashSetFrom(zips.Select(z => @"GH\" + z.Replace("-contexts.zip", "")));

            var jsonAll = File.ReadAllText(@"F:\R\index.json");
            var allSln = jsonAll.ParseJsonTo<HashSet<string>>();

            var json = File.ReadAllText(@"F:\R\ended-incl-fails.json");
            var endedInclFails = json.ParseJsonTo<HashSet<string>>();

            Console.WriteLine("== Found zips for the following {0} slns ==", zipSlns.Count);

            var sb = new StringBuilder();
            sb.Append("[");
            var first = true;
            foreach (var sln in allSln)
            {
                if (zipSlns.Contains(sln))
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }
                    first = false;
                    sb.Append('"').Append(sln).Append('"');
                }
            }
            sb.Append("]");
            Console.WriteLine(sb.ToString());
        }
    }
}