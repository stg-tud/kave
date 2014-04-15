using System;
using System.IO;
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
            var targetLocation = _requestFileLocation();
            Asserts.That(File.Exists(srcFilename), "Quelldatei existiert nicht");
            Asserts.NotNull(targetLocation, "Kein Ziel angegeben");
            Asserts.That(!targetLocation.IsEmpty(), "Invalides Ziel angegeben");

            try
            {
                _ioUtils.CopyFile(srcFilename, targetLocation);
            }
            catch (Exception e)
            {
                Asserts.Fail("File Export failed: {0}", e.Message);
            }
        }
    }
}