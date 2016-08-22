using System;
using System.IO;
using KaVE.Commons.Utils;
using KaVE.FeedbackProcessor.Preprocessing.Filters;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    internal class PreprocessingRunner
    {
        private readonly string _dirIn;
        private readonly string _dirLogs;
        private readonly string _dirMerged;
        private readonly string _dirOut;

        private readonly PreprocessingIo _io;

        public PreprocessingRunner(string dirIn, string dirTmp, string dirOut)
        {
            _dirIn = dirIn;
            _dirLogs = dirTmp + @"Logs\";
            _dirMerged = dirTmp + @"Merged\";
            _dirOut = dirOut;

            _io = new PreprocessingIo(_dirIn, _dirMerged, _dirOut);
        }

        public void Run()
        {
            new MultiThreadedRunner(
                _io,
                new MultiThreadedRunnerLogger(new ConsoleLogger(new DateUtils())),
                3,
                CreateIdReader,
                new Grouper(new GrouperLogger(CreateWorkerLogger(0))),
                CreateGroupMerger,
                CreateCleaner).Run();
        }

        private IdReader CreateIdReader(int taskId)
        {
            return new IdReader(new IdReaderLogger(CreateWorkerLogger(taskId)));
        }

        private GroupMerger CreateGroupMerger(int taskId)
        {
            throw new NotImplementedException();
        }

        private Cleaner CreateCleaner(int taskId)
        {
            return new Cleaner(null, new CleanerLogger(CreateWorkerLogger(taskId)))
            {
                Filters =
                {
                    //new VersionFilter(1000),
                    new NoSessionIdFilter(),
                    new NoTimeFilter(),
                    new InvalidCompletionEventFilter()
                }
            };
        }

        private IPrepocessingLogger CreateWorkerLogger(int taskId)
        {
            var logFile = Path.Combine(_dirLogs + @"Preprocessing-worker{0}.log".FormatEx(taskId));
            return new AppendingFileLogger(logFile, new DateUtils());
        }
    }
}