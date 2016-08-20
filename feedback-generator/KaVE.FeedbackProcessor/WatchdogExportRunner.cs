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