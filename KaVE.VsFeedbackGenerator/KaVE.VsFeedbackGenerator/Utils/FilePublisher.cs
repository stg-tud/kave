using System;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public class FilePublisher : IPublisher
    {
        private readonly Func<string> _requestFileLocation;
        private readonly IIoUtils _ioUtils;

        public FilePublisher([NotNull] Func<string> requestFileLocation)
        {
            _requestFileLocation = requestFileLocation;
            _ioUtils = Registry.GetComponent<IIoUtils>();
        }

        public void Publish(string srcFilename)
        {
            var targetFilename = _requestFileLocation();
            Asserts.That(_ioUtils.FileExists(srcFilename));
            Asserts.Not(targetFilename.IsNullOrEmpty(), Messages.NoFileGiven);

            try
            {
                _ioUtils.CopyFile(srcFilename, targetFilename);
            }
            catch (Exception e)
            {
                Asserts.Fail(Messages.PublishingFileFailed, e.Message);
            }
        }
    }
}