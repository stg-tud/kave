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

using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Intervals;
using KaVE.FeedbackProcessor.Intervals.Exporter;

namespace KaVE.FeedbackProcessor
{
    public class WatchdogExportRunner
    {
        private readonly ILogger _logger = new ConsoleLogger();

        public void RunWatchdogDebugging(string dirIn, string dirWd, string dirSvg)
        {
            var svge = new SvgExport();

            foreach (var zip in FindZips(dirIn))
            {
                _logger.Info(@"Opening {0} ...", zip);
                var intervals = new IntervalTransformer(_logger).TransformFile(zip).ToList();
                _logger.Info(@"Found {0} intervals.", intervals.Count);


                var userId = intervals[0].UserId;
                var svgFile = dirSvg + userId + ".svg";
                var eventsFile = dirSvg + userId + ".txt";
                var wdFilesFolder = dirWd + userId;

                _logger.Info(@"Dumping Event Stream ... ({0})", eventsFile);
                WriteEventStream(zip, eventsFile);

                _logger.Info(@"Converting to WD format ... ({0})", wdFilesFolder);
                WatchdogExporter.Convert(intervals).WriteToFiles(wdFilesFolder);

                _logger.Info(@"Now exporting .svg files for debugging ...");
                svge.Run(Lists.NewListFrom(intervals), svgFile);
            }
        }

        private static string[] FindZips(string dir)
        {
            var zips = Directory.GetFiles(dir, "*.zip", SearchOption.AllDirectories);
            return zips;
        }

        private static void WriteEventStream(string zip, string file)
        {
            var ra = new ReadingArchive(zip);
            var events = ra.GetAll<IDEEvent>();
            EventStreamExport.Write(events, file);
        }
    }
}