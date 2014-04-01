using System;
using System.IO;
using System.IO.Compression;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class GZipStreamTest
    {
        [Test]
        public void ShouldReadStreamWrittenSingleWriter()
        {
            var memory = new MemoryStream();
            using (var stream = new GZipStream(memory, CompressionMode.Compress))
            {
                stream.WriteByte(5);
                stream.WriteByte(42);
            }
            memory = new MemoryStream(memory.GetBuffer());
            var actual = new Byte[2];
            using (var stream = new GZipStream(memory, CompressionMode.Decompress))
            {
                stream.Read(actual, 0, 2);
            }
            Assert.AreEqual(new byte[] {5, 42}, actual);
        }

        [Test, Ignore("this does not work, since GzipStreams do not support appending to existing data")]
        public void ShouldReadStreamWrittenByMultipleWriters()
        {
            var memory = new MemoryStream();
            using (var stream = new GZipStream(memory, CompressionMode.Compress))
            {
                stream.WriteByte(5);
            }
            var intermediate = memory.GetBuffer();
            memory = new MemoryStream();
            memory.Write(intermediate, 0, intermediate.Length);
            using (var stream = new GZipStream(memory, CompressionMode.Compress))
            {
                stream.WriteByte(1);
            }
            memory = new MemoryStream(memory.GetBuffer());
            var actual = new Byte[2];
            using (var stream = new GZipStream(memory, CompressionMode.Decompress))
            {
                stream.Read(actual, 0, 2);
            }
            Assert.AreEqual(new byte[] {5, 1}, actual);
        }
    }
}