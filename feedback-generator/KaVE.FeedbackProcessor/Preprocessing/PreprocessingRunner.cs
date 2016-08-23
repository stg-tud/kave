using System.IO;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Preprocessing.Filters;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    internal class PreprocessingRunner
    {
        private readonly int _numWorkers;
        private readonly string _dirLogs;

        private readonly PreprocessingIo _io;

        public PreprocessingRunner(string dirIn, string dirTmp, string dirOut, int numWorkers = 1)
        {
            _numWorkers = numWorkers;
            Asserts.That(Directory.Exists(dirIn));
            Asserts.That(Directory.Exists(dirOut));

            Asserts.That(Directory.Exists(dirTmp));
            _dirLogs = dirTmp + @"Preprocessing-Logs\";
            var dirMerged = dirTmp + @"Preprocessing-Merged\";

            Asserts.Not(Directory.Exists(_dirLogs));
            Directory.CreateDirectory(_dirLogs);
            Asserts.Not(Directory.Exists(dirMerged));
            Directory.CreateDirectory(dirMerged);

            _io = new PreprocessingIo(dirIn, dirMerged, dirOut);
        }

        public void Run()
        {
            new MultiThreadedPreprocessing(
                _io,
                new MultiThreadedPreprocessingLogger(CreateWorkerLogger(-1)),
                _numWorkers,
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
            return new GroupMerger(_io, new GroupMergerLogger(CreateWorkerLogger(taskId)));
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
            var relFile = taskId == -1 ? "main.log" : @"worker{0}.log".FormatEx(taskId);
            var logFile = Path.Combine(_dirLogs + relFile);
            return new AppendingFileLogger(logFile, new DateUtils());
        }
    }
}