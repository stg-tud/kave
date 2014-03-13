using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public abstract class SessionExport<T>
    {
        protected string ExportToTemporaryFile(IList<T> events, Func<string, ILogWriter<T>> writer)
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
        public override ExportResult<IList<T>> Export(IList<T> events, Func<string, ILogWriter<T>> writerFactory)
        {
            var tempFile = ExportToTemporaryFile(events, writerFactory);
            var saveFileDialog = new SaveFileDialog
            {
                Filter = Properties.SessionManager.SaveFileDialogFilter
            };
            if (saveFileDialog.ShowDialog().Equals(DialogResult.Cancel))
            {
                return ExportResult<IList<T>>.Fail(events, "Dialog aborted");
            }
            var specifiedLocation = saveFileDialog.FileName; // check?
            File.Copy(tempFile, specifiedLocation, true);
            return ExportResult<IList<T>>.Success(events);
        }
    }

    internal class HttpExport<T> : SessionExport<T>
    {
        public const string HostAddress = "http://kave.st.informatik.tu-darmstadt.de:667/upload";
        public override ExportResult<IList<T>> Export(IList<T> events, Func<string, ILogWriter<T>> writerFactory)
        {
            var tempFile = ExportToTemporaryFile(events, writerFactory);
            // TODO: specify Filename/Extension (TransferFile accepts a parameter filename, but how should it look like? .log? .log.gz?)
            var response = HttpPostFileTransfer.TransferFile<ExportResult<object>>(HostAddress, tempFile);
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