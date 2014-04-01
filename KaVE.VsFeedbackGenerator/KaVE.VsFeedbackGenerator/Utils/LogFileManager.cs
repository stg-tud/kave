using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public abstract class LogFileManager<TMessage> : ILogFileManager<TMessage>
    {
        protected LogFileManager([NotNull] string baseLocation,
            IStreamTransformer transformer)
        {
            Transformer = transformer;
            BaseLocation = baseLocation;
        }

        public IStreamTransformer Transformer { get; private set; }

        public string BaseLocation { get; private set; }

        public string DefaultExtension
        {
            get { return ".log" + Transformer.Extention; }
        }

        public IEnumerable<string> GetLogFileNames()
        {
            return Directory.Exists(BaseLocation)
                ? Directory.GetFiles(BaseLocation, "*" + DefaultExtension)
                : new string[0];
        }

        private static void EnsureParentDirectoryExists(string fileName)
        {
            var parentDirectory = Path.GetDirectoryName(fileName);
            Asserts.NotNull(parentDirectory, "could not determine parent directory from path '{0}'", fileName);
            Directory.CreateDirectory(parentDirectory);
        }

        public ILogWriter<TMessage> NewLogWriter(string logFileName)
        {
            EnsureParentDirectoryExists(logFileName);
            Asserts.That(logFileName.EndsWith(Transformer.Extention));
            Stream logStream = new FileStream(logFileName, FileMode.Append, FileAccess.Write);
            return NewLogWriter(Transformer.TransformStreamForWrite(logStream));
        }

        protected abstract ILogWriter<TMessage> NewLogWriter(Stream logStream);

        public ILogReader<TMessage> NewLogReader(string logFileName)
        {
            Asserts.That(File.Exists(logFileName), "log file '{0}' doesn't exist", logFileName);
            Asserts.That(logFileName.EndsWith(Transformer.Extention));
            Stream logStream = new FileStream(logFileName, FileMode.Open);
            return NewLogReader(Transformer.TransformStreamForRead(logStream));
        }

        protected abstract ILogReader<TMessage> NewLogReader(Stream logStream);

        public void DeleteLogs(params string[] logFileNames)
        {
            foreach (var logFileName in logFileNames)
            {
                File.Delete(logFileName);
            }
        }

        public void DeleteLogsOlderThan(DateTime time)
        {
            DeleteLogs(GetLogFileNames().Where(log => File.GetLastWriteTime(log) < time).ToArray());
        }

        public string GetLogFileName(string filename, string extension = null)
        {
            return Path.Combine(BaseLocation, filename + (extension ?? DefaultExtension));
        }

        public override string ToString()
        {
            return "LogFileManager[" + BaseLocation + "," + DefaultExtension + "]";
        }
    }
}