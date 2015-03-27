/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Dennis Albrecht
 *    - Sebastian Proksch
 */

using System;
using System.IO;
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
        private const string FileContent = "arbitrary file content";
        private MemoryStream _stream;

        private static readonly Uri ValidUri = new Uri("http://server");

        private Mock<IIoUtils> _ioUtilsMock;
        private HttpPublisher _uut;

        [SetUp]
        public void SetUp()
        {
            _ioUtilsMock = new Mock<IIoUtils>();
            Registry.RegisterComponent(_ioUtilsMock.Object);
            _uut = new HttpPublisher(ValidUri);
            _stream = new MemoryStream();
        }

        [TearDown]
        public void CleanUpRegistry()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldInvokeTransferToCorrectUri()
        {
            var resp = CreateResponse(true);
            SetupResponse(resp);

            _uut.Publish(_stream);
            _ioUtilsMock.Verify(io => io.TransferByHttp(It.IsAny<HttpContent>(), ValidUri));
        }

        [Test]
        public void ShouldInvokeTransferWithCorrectData()
        {
            HttpContent lastUploadedContent = null;

            _ioUtilsMock.Setup(io => io.TransferByHttp(It.IsAny<HttpContent>(), ValidUri)).Returns(
                (HttpContent content, Uri uri) =>
                {
                    lastUploadedContent = content;
                    return CreateResponse(true);
                });

            _uut.Publish(new MemoryStream(FileContent.AsBytes()));

            var actual = ReadFirstContent(lastUploadedContent);
            Assert.AreEqual(FileContent, actual);
        }

        private const string TransferFailMessage = "Transfer war nicht möglich";

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = TransferFailMessage)]
        public void ShouldFailIfTransferFails()
        {
            _ioUtilsMock.Setup(io => io.TransferByHttp(It.IsAny<HttpContent>(), ValidUri))
                        .Throws(new AssertException(TransferFailMessage));
            _uut.Publish(_stream);
        }

        [Test,
         ExpectedException(typeof (AssertException),
             ExpectedMessage = "Antwort des Servers enthält keine verwertbaren Informationen")]
        public void ShouldFailIfMessageIsEmpty()
        {
            var resp = new HttpResponseMessage
            {
                Content = new StringContent("")
            };
            SetupResponse(resp);

            _uut.Publish(_stream);
        }

        [Test,
         ExpectedException(typeof (InvalidResponseException),
             ExpectedMessage = "Antwort des Servers entspricht nicht dem erwarteten Format:\r\nXYZ")]
        public void ShouldFailIfMessageCannotBeParsed()
        {
            var resp = new HttpResponseMessage
            {
                Content = new StringContent("XYZ")
            };
            SetupResponse(resp);

            _uut.Publish(_stream);
        }

        [Test, ExpectedException(typeof (InvalidResponseException))]
        public void ShouldFailIfMessageCannotBeParsed_verifyLog()
        {
            var resp = new HttpResponseMessage
            {
                Content = new StringContent("XYZ")
            };
            SetupResponse(resp);

            _uut.Publish(_stream);
        }

        [Test,
         ExpectedException(typeof (AssertException),
             ExpectedMessage = "Server meldet eine fehlerhafte Anfrage:\r\nXYZ")]
        public void ShouldFailIfStateIsNotOk()
        {
            var resp = CreateResponse(false, "XYZ");
            SetupResponse(resp);

            _uut.Publish(_stream);
        }

        private void SetupResponse(HttpResponseMessage resp)
        {
            _ioUtilsMock.Setup(io => io.TransferByHttp(It.IsAny<HttpContent>(), ValidUri))
                        .Returns(resp);
        }

        private static HttpResponseMessage CreateResponse(bool isSuccessful, string message = null)
        {
            var state = isSuccessful ? "Ok" : "Fail";
            var responseMessage = "";
            if (message != null)
            {
                responseMessage = ",\"Message\": \"" + message + "\"";
            }

            var resp = new HttpResponseMessage
            {
                Content = new StringContent("{\"Status\": \"" + state + "\"" + responseMessage + "}")
            };
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

            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual("file", byteArrayContent.Headers.ContentDisposition.Name);
            Assert.AreEqual("tmp.zip", byteArrayContent.Headers.ContentDisposition.FileName);

            return byteArrayContent.ReadAsByteArrayAsync().Result.AsString();
        }
    }
}