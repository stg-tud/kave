using System;
using System.IO;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ISessionPublisher
    {
        ExportResult<object> Publish(string tmpFilename);
    }

    public class FilePublisher : ISessionPublisher
    {
        private readonly Func<string> _requestFileLocation;

        public FilePublisher([NotNull] Func<string> requestFileLocation)
        {
            _requestFileLocation = requestFileLocation;
        }

        public ExportResult<object> Publish(string tmpFilename)
        {
            var targetLocation = _requestFileLocation();
            if (targetLocation.IsNullOrEmpty())
            {
                return ExportResult<object>.Fail();
            }
            File.Copy(tmpFilename, targetLocation, true);
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

        public ExportResult<object> Publish(string tmpFilename)
        {
            try
            {
                var response = HttpPostFileTransfer.TransferFile<ExportResult<object>>(_hostAddress, tmpFilename);
                if (response != null)
                {
                    return response;
                }
            }
                // ReSharper disable once EmptyGeneralCatchClause
            catch {}

            return ExportResult<object>.Fail();
        }
    }
}