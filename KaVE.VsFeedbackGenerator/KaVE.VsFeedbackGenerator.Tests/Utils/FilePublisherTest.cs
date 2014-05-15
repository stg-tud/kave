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
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class FilePublisherTest
    {
        private Mock<IIoUtils> _ioMock;
        private const string SomeTargetLocation = "existing target file";
        private MemoryStream _stream;

        [SetUp]
        public void SetUp()
        {
            _ioMock = new Mock<IIoUtils>();
            Registry.RegisterComponent(_ioMock.Object);
            _stream = new MemoryStream();
        }

        [Test]
        public void ShouldInvokeCopy()
        {
            var uut = new FilePublisher(() => SomeTargetLocation);
            uut.Publish(_stream);

            _ioMock.Verify(m => m.WriteAllByte(It.IsAny<byte[]>(), SomeTargetLocation));
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldThrowExceptionOnNullArgument()
        {
            var uut = new FilePublisher(() => null);
            uut.Publish(_stream);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldThrowExceptionOnEmptyArgument()
        {
            var uut = new FilePublisher(() => "");
            uut.Publish(_stream);
        }

        private const string CopyFailureMessage = "Datei-Export fehlgeschlagen: XYZ";

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = CopyFailureMessage)]
        public void ShouldThrowExceptionIfCopyFails()
        {
            _ioMock.Setup(io => io.WriteAllByte(It.IsAny<byte[]>(), SomeTargetLocation)).Throws(new Exception("XYZ"));
            var uut = new FilePublisher(() => SomeTargetLocation);
            uut.Publish(_stream);
        }

        [TearDown]
        public void CleanUpRegistry()
        {
            Registry.Clear();
        }
    }
}