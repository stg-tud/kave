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
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.WatchdogExports.Exporter;

namespace KaVE.FeedbackProcessor.WatchdogExports
{
    public class WatchdogExportRunner
    {
        private const int FirstVersionToInclude = 1010;

        private readonly string _dirIn;
        private readonly string _dirWd;
        private readonly string _dirSvg;
        private readonly ILogger _logger = new ConsoleLogger();
        private readonly EventStreamExport _export;
        private readonly EventFixer _eventFixer;

        public WatchdogExportRunner(string dirIn, string dirWd, string dirSvg)
        {
            Console.WriteLine(
                "Initializing WatchdogExportRunner (first version to include: 0.{0})",
                FirstVersionToInclude);
            _dirIn = dirIn;
            _dirWd = dirWd;
            _dirSvg = dirSvg;
            _eventFixer = new EventFixer();
            _export = new EventStreamExport(dirSvg, _eventFixer);
        }

        private IntervalTransformer InitTransformer()
        {
            return new IntervalTransformer(_logger, _eventFixer, FirstVersionToInclude);
        }

        public void RunTransformation()
        {
            var transformer = InitTransformer();

            Console.Write(@"Finding Zips... ");
            var zips = FindZips(_dirIn);

            Console.WriteLine(@"found {0}.", zips.Length);

            var total = zips.Length;
            var cur = 0;

            var intervals = zips.SelectMany(
                zip =>
                {
                    Console.WriteLine();
                    Console.WriteLine(new string('#', 60));
                    var proc = 100*cur/(double) total;
                    var size = new FileInfo(zip).Length;
                    var sizeInMb = size/(1024*1024);
                    Console.WriteLine(
                        @"Transforming .zip {0}/{1} ({2:0.0}% done): {3} [{4:0.0}MB]...",
                        ++cur,
                        total,
                        proc,
                        zip,
                        sizeInMb);
                    var intervalsForFile = transformer.TransformFile(zip).ToList();
                    Console.WriteLine(@"--> Extracted {0} intervals.", intervalsForFile.Count);
                    return intervalsForFile;
                }).ToList();

            Console.WriteLine();
            Console.WriteLine(new string('#', 60));
            Console.WriteLine(@"Found {0} intervals, converting to WatchDog Data...", intervals.Count);
            var data = WatchdogExporter.Convert(intervals);
            Console.WriteLine(@"Wrtiting...");
            data.WriteToFiles(_dirWd);
        }

        public void RunDebugging()
        {
            var svge = new SvgExport();

            foreach (var zip in FindZips(_dirIn))
            {
                _logger.Info(@"Opening {0} ...", zip);
                var intervals = InitTransformer().TransformFile(zip).ToList();
                _logger.Info(@"Found {0} intervals.", intervals.Count);


                var userId = zip.Substring(_dirIn.Length, zip.Length - _dirIn.Length - 4); // strip baseDir and extension
                var svgFile = _dirSvg + userId + ".svg";
                var relEventsFile = userId + ".txt";
                var wdFilesFolder = _dirWd + userId;

                _logger.Info(@"Dumping Event Stream ... ({0})", relEventsFile);
                WriteEventStream(zip, relEventsFile);

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

        private void WriteEventStream(string zip, string relFile)
        {
            using (var ra = new ReadingArchive(zip))
            {
                var events = ra.GetAll<IDEEvent>();
                _export.Write(events, relFile);
            }
        }
    }
}