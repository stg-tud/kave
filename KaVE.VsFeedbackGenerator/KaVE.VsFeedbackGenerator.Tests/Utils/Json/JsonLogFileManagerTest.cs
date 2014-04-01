using System.IO;
using System.IO.Compression;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    [TestFixture]
    internal class JsonLogFileManagerTest : LogFileManagerTest
    {
        protected override ILogFileManager<string> CreateManager(string baseLocation)
        {
            return new JsonLogFileManager<string>(baseLocation);
        }

        [Test, Ignore("we currently don't write compressed logs, because we cannot")]
        public void DefaultJsonLogFileManagerWritesCompressed()
        {
            const string message = "test-string";
            var file = Path.Combine(IoTestHelper.GetTempDirectoryName(), "File" + Uut.DefaultExtension);
            using (var writer = Uut.NewLogWriter(file))
            {
                writer.Write(message);
            }

            var zipStream = new GZipStream(new FileStream(file, FileMode.Open), CompressionMode.Decompress);
            string actual;
            using (var reader = new StreamReader(zipStream, JsonLogSerialization.Encoding))
            {
                actual = reader.ReadLine();
            }
            const string expected = "\"" + message + "\"";

            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore("we currently don't write compressed logs, because we cannot")]
        public void ShouldHaveCorrectExtension()
        {
            Assert.AreEqual(".log.gz", Uut.DefaultExtension);
        }
    }
}
