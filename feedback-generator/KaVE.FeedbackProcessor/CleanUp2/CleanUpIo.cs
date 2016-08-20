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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.FeedbackProcessor.CleanUp2
{
    public interface ICleanUpIo
    {
        IList<string> GetZips();
        IEnumerable<IDEEvent> ReadZip(string zip);
        void WriteZip(IEnumerable<IDEEvent> events, string zip);
    }

    public class CleanUpIo : ICleanUpIo
    {
        private readonly string _dirIn;
        private readonly string _dirOut;

        public CleanUpIo(string dirIn, string dirOut)
        {
            if (!dirIn.EndsWith(@"\"))
            {
                dirIn += @"\";
            }
            if (!dirOut.EndsWith(@"\"))
            {
                dirOut += @"\";
            }
            _dirIn = dirIn;
            _dirOut = dirOut;

            Console.WriteLine(@"working directories:");
            Console.WriteLine(@"- in:  {0}", dirIn);
            Console.WriteLine(@"- out: {0}", dirOut);
        }

        public IList<string> GetZips()
        {
            return
                Directory.EnumerateFiles(_dirIn, "*.zip", SearchOption.AllDirectories)
                         .Select(f => f.Replace(_dirIn, "")).ToList();
        }

        public IEnumerable<IDEEvent> ReadZip(string zip)
        {
            var fullName = Path.Combine(_dirIn, zip);
            var ra = new ReadingArchive(fullName);
            return ra.GetAll<IDEEvent>();
        }

        public void WriteZip(IEnumerable<IDEEvent> events, string zip)
        {
            var fullName = Path.Combine(_dirOut, zip);

            CreateParentIfNecessary(fullName);

            using (var wa = new WritingArchive(fullName))
            {
                wa.AddAll(events);
            }
        }

        private static void CreateParentIfNecessary(string fullName)
        {
            var parent = Path.GetDirectoryName(fullName);
            Asserts.NotNull(parent);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
        }
    }
}