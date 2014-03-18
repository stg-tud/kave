using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public abstract class SessionExport<T>
    {
        internal string ExportToTemporaryFile(IList<T> events, Func<string, ILogWriter<T>> writer)
        {
            var tempFileName = Path.GetTempFileName();
            using (var logWriter = writer(tempFileName))
            {
                foreach (var e in events)
                {
                    logWriter.Write(e);
                }
            }
            return tempFileName;
        }

        public abstract ExportResult<IList<T>> Export(IList<T> events, Func<string, ILogWriter<T>> writerFactory);
    }

    internal class FileExport<T> : SessionExport<T>
    {
        private readonly Func<string> _saveFile;

        public FileExport([NotNull] Func<string> saveFile)
        {
            _saveFile = saveFile;
        }

        public override ExportResult<IList<T>> Export(IList<T> events, Func<string, ILogWriter<T>> writerFactory)
        {
            var specifiedLocation = _saveFile();
            if (specifiedLocation.IsNullOrEmpty())
            {
                return ExportResult<IList<T>>.Fail(events);
            }
            var tempFile = ExportToTemporaryFile(events, writerFactory);
            File.Copy(tempFile, specifiedLocation, true);
            return ExportResult<IList<T>>.Success(events);
        }
    }

    internal class HttpExport<T> : SessionExport<T>
    {
        private readonly string _hostAddress;

        public HttpExport([NotNull] string hostAddress)
        {
            _hostAddress = hostAddress;
        }

        public override ExportResult<IList<T>> Export(IList<T> events, Func<string, ILogWriter<T>> writerFactory)
        {
            var tempFile = ExportToTemporaryFile(events, writerFactory);
            // TODO: specify Filename/Extention (TransferFile accepts a parameter filename, but how should it look like? .log? .log.gz?)
            var response = HttpPostFileTransfer.TransferFile<ExportResult<object>>(_hostAddress, tempFile);
            if (response == null)
            {
                return ExportResult<IList<T>>.Fail(events);
            }
            return ExportResult<IList<T>>.CloneWithData(response, events);
        }
    }

    internal class TempExport<T> : SessionExport<T>
    {
        public override ExportResult<IList<T>> Export(IList<T> events, Func<string, ILogWriter<T>> writerFactory)
        {
            ExportToTemporaryFile(events, writerFactory);
            return ExportResult<IList<T>>.Success(events);
        }
    }
}