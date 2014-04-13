using System.IO;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class FilePublisherTest
    {
        private Mock<IIoHelper> _ioMock;

        [SetUp]
        public void SetUp()
        {
            _ioMock = new Mock<IIoHelper>();
            Registry.RegisterComponent(_ioMock.Object);
        }

        // TODO jetzt überflüssig, da die Registry genutzt wird?
        [Test]
        public void ShouldPublishFile()
        {
            const string content = "some text to test the file-copy";

            var sourceLocation = Path.GetTempFileName();
            var targetLocation = Path.GetTempFileName();
            WriteSourceFile(content, sourceLocation);
            _ioMock.Setup(m => m.CopyFile(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>(new IoHelper().CopyFile);

            var uut = new FilePublisher(() => targetLocation);
            uut.Publish(sourceLocation);

            var actual = ReadPublishedFile(targetLocation);

            Assert.AreEqual(content, actual);
        }

        private static void WriteSourceFile(string content, string location)
        {
            using (var writer = new StreamWriter(new FileStream(location, FileMode.Create, FileAccess.Write)))
            {
                writer.Write(content);
            }
        }

        private static string ReadPublishedFile(string location)
        {
            using (var reader = new StreamReader(new FileStream(location, FileMode.Open)))
            {
                return reader.ReadToEnd();
            }
        }

        [Test]
        public void ShouldInvokeCopy()
        {
            var srcLoc = Path.GetTempFileName();
            var trgLoc = Path.GetTempFileName();
            _ioMock.Setup(m => m.CopyFile(It.IsAny<string>(), It.IsAny<string>()));

            var uut = new FilePublisher(() => trgLoc);
            uut.Publish(srcLoc);

            Registry.Clear();
            _ioMock.Verify(m => m.CopyFile(srcLoc, trgLoc));
        }

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Quelldatei existiert nicht")]
        public void ShouldThrowExceptionOnNonexistingSourceFile()
        {
            var uut = new FilePublisher(Path.GetTempFileName);
            uut.Publish("some illegal location");
        }

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Kein Ziel angegeben")]
        public void ShouldThrowExceptionOnNullArgument()
        {
            var uut = new FilePublisher(() => null);
            uut.Publish(Path.GetTempFileName());
        }

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Invalides Ziel angegeben")]
        public void ShouldThrowExceptionOnEmptyArgument()
        {
            var uut = new FilePublisher(() => "");
            uut.Publish(Path.GetTempFileName());
        }

        [TearDown]
        public void CleanUpRegistry()
        {
            Registry.Clear();
        }
    }

    [TestFixture]
    internal class HttpPublisherTest
    {
        private Mock<IIoHelper> _ioMock;

        [SetUp]
        public void SetUp()
        {
            _ioMock = new Mock<IIoHelper>();
            Registry.RegisterComponent(_ioMock.Object);
        }

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Quelldatei existiert nicht")]
        public void ShouldThrowExceptionOnNonexistingSourceFile()
        {
            var uut = new HttpPublisher("some url");
            uut.Publish("some illegal location");
        }

        [Test]
        public void ShouldInvokeTransfer()
        {
            var srcLoc = Path.GetTempFileName();
            var url = "this is a valid url";
            _ioMock.Setup(m => m.TransferViaHttpPost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var uut = new HttpPublisher(url);
            uut.Publish(srcLoc);

            Registry.Clear();
            _ioMock.Verify(m => m.TransferViaHttpPost(url, srcLoc, null));
        }

        // TODO move this test to HttpPostFileTransferTest?
        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Server nicht erreichbar")]
        public void ShouldThrowExceptionOnUnreachableServer()
        {
            _ioMock.Setup(m => m.TransferViaHttpPost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                   .Callback<string, string, string>(new IoHelper().TransferViaHttpPost);

            var uut = new HttpPublisher("http://asdf:12345");
            uut.Publish(Path.GetTempFileName());
        }

        [TearDown]
        public void CleanUpRegistry()
        {
            Registry.Clear();
        }
    }
}