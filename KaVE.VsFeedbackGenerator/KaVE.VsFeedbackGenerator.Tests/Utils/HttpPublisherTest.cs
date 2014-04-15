using System;
using System.Linq;
using System.Net.Http;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class HttpPublisherTest
    {
        private const string ExistingFile = "abitrary existing file";
        private const string FileContent = "abitrary file content";

        private static readonly Uri ValidUri = new Uri("http://server");

        private Mock<IIoUtils> _ioUtilsMock;
        private HttpPublisher _uut;

        [SetUp]
        public void SetUp()
        {
            _ioUtilsMock = new Mock<IIoUtils>();
            _ioUtilsMock.Setup(io => io.FileExists(ExistingFile)).Returns(true);
            _ioUtilsMock.Setup(io => io.ReadFile(ExistingFile)).Returns(FileContent);
            Registry.RegisterComponent(_ioUtilsMock.Object);
            _uut = new HttpPublisher(ValidUri);
        }

        [TearDown]
        public void CleanUpRegistry()
        {
            Registry.Clear();
        }

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Quelldatei existiert nicht")]
        public void ShouldThrowExceptionOnNonexistingSourceFile()
        {
            _uut.Publish("some illegal path");
        }

        [Test]
        public void ShouldInvokeTransferToCorrectUri()
        {
            var resp = CreateResponse(true);
            SetupResponse(resp);

            _uut.Publish(ExistingFile);
            _ioUtilsMock.Verify(m => m.TransferByHttp(It.IsAny<HttpContent>(), ValidUri, It.IsAny<int>()));
        }

        [Test]
        public void ShouldInvokeTransferWithCorrectData()
        {
            HttpContent lastUploadedContent = null;

            _ioUtilsMock.Setup(io => io.TransferByHttp(It.IsAny<HttpContent>(), ValidUri, It.IsAny<int>())).Returns(
                (HttpContent content, Uri uri, int timeout) =>
                {
                    lastUploadedContent = content;
                    return CreateResponse(true);
                });

            _uut.Publish(ExistingFile);

            var actual = ReadFirstContent(lastUploadedContent);
            Assert.AreEqual(FileContent, actual);
        }

        [Test, Ignore]
        public void ShouldFailIfMessageIsNull()
        {
            throw new NotImplementedException();
        }

        [Test, Ignore]
        public void ShouldFailIfMessageCannotBeParsed()
        {
            throw new NotImplementedException();
        }

        [Test, Ignore]
        public void ShouldFailIfStateIsNotOk()
        {
            throw new NotImplementedException();
        }

        private void SetupResponse(HttpResponseMessage resp)
        {
            _ioUtilsMock.Setup(io => io.TransferByHttp(It.IsAny<HttpContent>(), ValidUri, It.IsAny<int>()))
                        .Returns(resp);
        }

        private static HttpResponseMessage CreateResponse(bool isSuccessful, string message = null)
        {
            string state = isSuccessful ? "Ok" : "Fail";
            string amessage = "";
            if (message != null)
            {
                amessage = "\"Message\": \"" + message + "\"";
            }

            var resp = new HttpResponseMessage();
            resp.Content = new StringContent("{\"State\": \"" + state + "\"" + amessage + "}");
            return resp;
        }

        private static string ReadFirstContent([NotNull] HttpContent content)
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
            Assert.AreEqual("tmp.log", byteArrayContent.Headers.ContentDisposition.FileName);

            return byteArrayContent.ReadAsByteArrayAsync().Result.GetString();
        }
    }
}