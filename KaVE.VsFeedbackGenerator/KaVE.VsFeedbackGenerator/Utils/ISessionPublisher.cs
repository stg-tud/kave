using System;
using System.IO;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface ISessionPublisher
    {
        void Publish(string srcFilename);
    }

    public class FilePublisher : ISessionPublisher
    {
        private readonly Func<string> _requestFileLocation;
        private readonly IIoHelper _ioHelper;

        public FilePublisher([NotNull] Func<string> requestFileLocation)
        {
            _requestFileLocation = requestFileLocation;
            _ioHelper = Registry.GetComponent<IIoHelper>();
        }

        public void Publish(string srcFilename)
        {
            var targetLocation = _requestFileLocation();
            Asserts.That(File.Exists(srcFilename), "Quelldatei existiert nicht");
            Asserts.NotNull(targetLocation, "Kein Ziel angegeben");
            Asserts.That(!targetLocation.IsEmpty(), "Invalides Ziel angegeben");

            try
            {
                _ioHelper.CopyFile(srcFilename, targetLocation);
            }
            catch (Exception e)
            {
                Asserts.Fail("File Export failed: {0}", e.Message);
            }
        }
    }

    public class HttpPublisher : ISessionPublisher
    {
        private readonly string _hostAddress;
        private readonly IIoHelper _ioHelper;

        public HttpPublisher([NotNull] string hostAddress)
        {
            _hostAddress = hostAddress;
            _ioHelper = Registry.GetComponent<IIoHelper>();
        }

        public void Publish(string srcFilename)
        {
            Asserts.That(File.Exists(srcFilename), "Quelldatei existiert nicht");
            try
            {
                _ioHelper.TransferViaHttpPost(_hostAddress, srcFilename);
            }
            catch (Exception e)
            {
                Asserts.Fail(e.Message);
            }
        }
    }
}