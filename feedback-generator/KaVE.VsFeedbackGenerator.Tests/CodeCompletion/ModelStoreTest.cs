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
 */

using System;
using System.IO;
using Ionic.Zip;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Exceptions;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;
using Smile;

namespace KaVE.VsFeedbackGenerator.Tests.CodeCompletion
{
    [TestFixture]
    internal class ModelStoreTest
    {
        private Mock<IIoUtils> _utils;
        private Mock<ILogger> _logger;
        private ModelStore _uut;
        private Network _network;
        private const string BasePath = "c:/Base/";
        private const string TempPath = "c:/Temp/";
        private const string Assembly = "assembly";
        private const string Type = "LType";
        private const string Zip = "assembly.zip";
        private const string NetworkFileName = "LType.xdsl";
        private const string DifferentNetworkFileName = "DIFFERENT.xdsl";
        private static readonly string ZipPath = Path.Combine(BasePath, Zip);
        private static readonly string AssemblyPath = Path.Combine(TempPath, Assembly);
        private static readonly string NetworkPath = Path.Combine(AssemblyPath, NetworkFileName);

        [SetUp]
        public void SetUp()
        {
            _network = UsageModelFixture.CreateNetwork();
            _utils = new Mock<IIoUtils>();
            _logger = new Mock<ILogger>();
            _uut = new ModelStore(BasePath, TempPath, _utils.Object, _logger.Object);
        }

        [Test]
        public void ShouldHandleNoNetworkAndNoZipExists()
        {
            _utils.Setup(u => u.FileExists(It.IsAny<string>())).Returns(false);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type));

            Assert.IsNull(model);
        }

        [Test]
        public void ShouldHandleNetworkExists()
        {
            _utils.Setup(u => u.FileExists(NetworkPath)).Returns(true);
            _utils.Setup(u => u.LoadNetwork(NetworkPath)).Returns(_network);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type));

            Assert.AreEqual(new UsageModel(_network), model);
        }

        [Test]
        public void ShouldHandleZipWithEntryForType()
        {
            _utils.Setup(u => u.FileExists(ZipPath)).Returns(true);

            var content = new byte[] {1, 2, 3};
            var zipStream = GetZipStream(NetworkFileName, content);
            _utils.Setup(u => u.OpenFile(ZipPath, FileMode.Open, FileAccess.Read)).Returns(zipStream);

            _utils.Setup(u => u.WriteAllByte(It.IsAny<byte[]>(), NetworkPath))
                  .Callback<byte[], string>(
                      (bytes, path) =>
                      {
                          CollectionAssert.AreEqual(content, bytes);
                          _utils.Setup(u => u.FileExists(NetworkPath)).Returns(true);
                          _utils.Setup(u => u.LoadNetwork(NetworkPath)).Returns(_network);
                      });

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type));

            Assert.AreEqual(new UsageModel(_network), model);

            _utils.Verify(u => u.CreateDirectory(AssemblyPath));
        }

        [Test]
        public void ShouldHandleZipWithoutEntryForType()
        {
            _utils.Setup(u => u.FileExists(ZipPath)).Returns(true);

            var content = new byte[] {1, 2, 3};
            var zipStream = GetZipStream(DifferentNetworkFileName, content);
            _utils.Setup(u => u.OpenFile(ZipPath, FileMode.Open, FileAccess.Read)).Returns(zipStream);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type));

            Assert.IsNull(model);
        }

        [Test]
        public void ShouldHandleNetworkExistsButLoadingFails()
        {
            var exception = new Exception();
            _utils.Setup(u => u.FileExists(NetworkPath)).Returns(true);
            _utils.Setup(u => u.LoadNetwork(NetworkPath)).Throws(exception);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type));

            Assert.IsNull(model);

            _logger.Verify(l => l.Error(exception));
        }

        [Test]
        public void ShouldHandleZipExistsButFileExtractingFails()
        {
            _utils.Setup(u => u.FileExists(ZipPath)).Returns(true);

            var exception = new Exception();
            _utils.Setup(u => u.OpenFile(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>()))
                  .Throws(exception);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type));

            Assert.IsNull(model);

            _logger.Verify(l => l.Error(exception));
        }

        private static Stream GetZipStream(string fileName, byte[] content)
        {
            var zip = new ZipFile();
            zip.AddEntry(fileName, content);
            var stream = new MemoryStream();
            zip.Save(stream);
            stream.Position = 0;
            return stream;
        }
    }
}