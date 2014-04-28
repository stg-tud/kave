using System;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;

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
            Asserts.Not(targetFilename.IsNullOrEmpty());

            try
            {
                _ioUtils.CopyFile(srcFilename, targetFilename);
            }
            catch (Exception e)
            {
                // TODO @Dennis: Move strings to resource file
                Asserts.Fail("Datei-Export fehlgeschlagen: {0}", e.Message);
            }
        }
    }
}