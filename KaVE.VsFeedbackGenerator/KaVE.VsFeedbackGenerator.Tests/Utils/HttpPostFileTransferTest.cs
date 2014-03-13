using System.IO;
using System.Linq;
using System.Net.Http;
using KaVE.JetBrains.Annotations;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class HttpPostFileTransferTest
    {
        private const string FileContent = "Some funny text to verify the functionality of HttpPostFileTransfer";

        [Test]
        public void CreateHttpContentWithNonexistingFileReturnsNull()
        {
            var actual = HttpPostFileTransfer.CreateHttpContent("This is a nonexisting file");
            Assert.IsNull(actual);
        }

        [Test]
        public void CreateHttpContentReturnsCorrectFormat()
        {
            var file = Path.GetTempFileName();
            var content = HttpPostFileTransfer.CreateHttpContent(file);
            AssertFormat(content, Path.GetFileName(file));
        }

        [Test]
        public void CreateHttpContentUsingGivenFileNameReturnsCorrectFormat()
        {
            var file = Path.GetTempFileName();
            const string name = "Funny-File.trial";
            var content = HttpPostFileTransfer.CreateHttpContent(file, name);
            AssertFormat(content, name);
        }

        [Test]
        public void CreateHttpContentContainsCorrectByteArray()
        {
            var file = Path.GetTempFileName();
            using (var writer = new StreamWriter(File.OpenWrite(file)))
            {
                writer.Write(FileContent);
            }

            var content = HttpPostFileTransfer.CreateHttpContent(file) as MultipartFormDataContent;
            Assert.IsNotNull(content);
            var byteArrayContent = content.First() as ByteArrayContent;
            Assert.IsNotNull(byteArrayContent);
            var actual = byteArrayContent.ReadAsStringAsync().Result;
            Assert.AreEqual(FileContent, actual);
        }

        private static void AssertFormat([NotNull] HttpContent content, [NotNull] string filename)
        {
            // IsNotNull-Asserts are semantically equivalent to the corresponding IsInstanceOf-Assert but R# produces warnings otherwise
            // IsInstanceOf-Asserts are kept, because if they fail the result is more informative
            Assert.IsInstanceOf<MultipartFormDataContent>(content);
            var multipartFormDataContent = content as MultipartFormDataContent;
            Assert.IsNotNull(multipartFormDataContent);
            Assert.AreEqual(1, multipartFormDataContent.Count());
            var element = multipartFormDataContent.First();
            Assert.IsInstanceOf<ByteArrayContent>(element);
            var byteArrayContent = element as ByteArrayContent;
            Assert.IsNotNull(byteArrayContent);
            Assert.AreEqual("file", byteArrayContent.Headers.ContentDisposition.Name);
            Assert.AreEqual(filename, byteArrayContent.Headers.ContentDisposition.FileName);
            Assert.AreEqual(new byte[0], byteArrayContent.ReadAsByteArrayAsync().Result);
        }
    }
}