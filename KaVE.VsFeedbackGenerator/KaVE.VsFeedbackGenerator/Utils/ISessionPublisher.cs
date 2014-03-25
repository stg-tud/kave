using System;
using System.IO;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ISessionPublisher
    {
        ExportResult<object> Publish(string filename);
    }

    public class FilePublisher : ISessionPublisher
    {
        private readonly Func<string> _saveFile;

        public FilePublisher([NotNull] Func<string> saveFile)
        {
            _saveFile = saveFile;
        }

        public ExportResult<object> Publish(string filename)
        {
            var specifiedLocation = _saveFile();
            if (specifiedLocation.IsNullOrEmpty())
            {
                return ExportResult<object>.Fail();
            }
            File.Copy(filename, specifiedLocation, true);
            return ExportResult<object>.Success();
        }
    }

    public class HttpPublisher : ISessionPublisher
    {
        private readonly string _hostAddress;

        public HttpPublisher([NotNull] string hostAddress)
        {
            _hostAddress = hostAddress;
        }

        public ExportResult<object> Publish(string filename)
        {
            var response = HttpPostFileTransfer.TransferFile<ExportResult<object>>(_hostAddress, filename);
            if (response == null)
            {
                return ExportResult<object>.Fail();
            }
            return response;
        }
    }
}