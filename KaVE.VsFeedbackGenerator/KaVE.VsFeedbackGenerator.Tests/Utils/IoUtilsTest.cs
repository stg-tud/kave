using System;
using System.IO;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class IoUtilsTest
    {
        private IoUtils _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new IoUtils();
        }

        [Test, Ignore]
        public void TransferByHttpNotImplementedYet()
        {
            throw new NotImplementedException();
        }
        //[Test, ExpectedException(typeof (AssertException), ExpectedMessage = "Server nicht erreichbar")]
        //public void ShouldThrowExceptionOnUnreachableServer()
        
        [Test]
        public void ShouldReturnExistingTempFileName()
        {
            var fileName = _sut.GetTempFileName();
            Assert.True(File.Exists(fileName));
        }

        [Test]
        public void ShouldNotReturnTempFileNameTwice()
        {
            var a = _sut.GetTempFileName();
            var b = _sut.GetTempFileName();
            Assert.AreNotEqual(a, b);
        }

        [Test]
        public void ShouldDetectIfFileExists()
        {
            var fileName = Path.GetTempFileName();
            Assert.True(_sut.FileExists(fileName));
            File.Delete(fileName);
            Assert.False(_sut.FileExists(fileName));
        }

        [Test]
        public void ShouldCopyFile()
        {
            var a = Path.GetTempFileName();
            var b = Path.GetTempFileName();
            File.Delete(b);

            const string expected = "blubb";
            File.WriteAllText(a, expected);
            _sut.CopyFile(a, b);
            Assert.True(File.Exists(a));
            Assert.True(File.Exists(b));

            var actual = File.ReadAllText(b);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldReadcontentsOfFile()
        {
            var a = Path.GetTempFileName();
            const string expected = "blubb";
            File.WriteAllText(a, expected);
            var actual = _sut.ReadFile(a);
            Assert.AreEqual(expected, actual);
        }
    }
}