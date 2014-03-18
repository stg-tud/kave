using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public class LogFileManager<TMessage> : ILogFileManager<TMessage>
    {
        public LogFileManager([NotNull] string location,
            IFormatWriter<TMessage> mainWriter,
            params IFormatReader<TMessage>[] otherFormats)
        {
            var all = new List<IFormatReader<TMessage>>
            {
                mainWriter
            };
            all.AddRange(otherFormats);
            Reader = all;
            Writer = all.OfType<IFormatWriter<TMessage>>();
            BaseLocation = location;
        }

        public IEnumerable<IFormatWriter<TMessage>> Writer { get; private set; }
        public IEnumerable<IFormatReader<TMessage>> Reader { get; private set; }

        public string BaseLocation { get; private set; }

        public string DefaultExtention
        {
            get { return Writer.First().Extention; }
        }

        public IEnumerable<string> GetLogFileNames()
        {
            return Directory.Exists(BaseLocation)
                ? Reader.SelectMany(r => Directory.GetFiles(BaseLocation, "*" + r.Extention))
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
            var writer = Writer.First(w => logFileName.EndsWith(w.Extention));
            return writer.NewWriter(logFileName);
        }

        public ILogReader<TMessage> NewLogReader(string logFileName)
        {
            Asserts.That(File.Exists(logFileName), "log file '{0}' doesn't exist", logFileName);
            var reader = Reader.First(w => logFileName.EndsWith(w.Extention));
            return reader.NewReader(logFileName);
        }

        public void DeleteLogsOlderThan(DateTime time)
        {
            foreach (var file in GetLogFileNames().Where(log => File.GetLastWriteTime(log) < time))
            {
                File.Delete(file);
            }
        }

        public string GetLogFileName(string filename, string extension = null)
        {
            return Path.Combine(BaseLocation, filename + (extension ?? DefaultExtention));
        }
    }
}