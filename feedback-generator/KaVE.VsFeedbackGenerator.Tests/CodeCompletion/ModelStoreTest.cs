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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using KaVE.Model.ObjectUsage;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.CodeCompletion
{
    [TestFixture]
    internal class ModelStoreTest
    {
        private Mock<IIoUtils> _utils;
        private Mock<ILogger> _logger;
        private ModelStore _uut;
        private const string BasePath = "c:/Base/";
        private static readonly string ZipPath = Path.Combine(BasePath, Zip);
        private static readonly string AssemblyPath = Path.Combine(BasePath, Assembly);
        private static readonly string NetworkPath = Path.Combine(BasePath, Assembly, Network);
        private const string Assembly = "assembly";
        private const string Type = "LType";
        private const string Network = "LType.xdsl";
        private const string Zip = "assembly.zip";

        [SetUp]
        public void SetUp()
        {
            _utils = new Mock<IIoUtils>();
            _utils.Setup(u => u.Combine(It.IsAny<string[]>())).Returns<string[]>(Path.Combine);
            _logger = new Mock<ILogger>();
            _uut = new ModelStore(BasePath, _utils.Object, _logger.Object);
        }

        [Test]
        public void ShouldReturnNullIfModelDoesNotExist_Network()
        {
            _utils.Setup(u => u.FileExists(It.IsAny<string>())).Returns(false);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type));

            Assert.IsNull(model);

            _utils.Verify(u => u.FileExists(NetworkPath));
        }

        [Test]
        public void ShouldReturnNullIfModelDoesNotExist_ZipForceReload()
        {
            _utils.Setup(u => u.FileExists(It.IsAny<string>())).Returns(false);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type), true);

            Assert.IsNull(model);

            _utils.Verify(u => u.FileExists(ZipPath));
            _utils.Verify(u => u.FileExists(NetworkPath), Times.Never);
        }

        [Test]
        public void ShouldReturnNullIfModelDoesNotExist_Zip()
        {
            _utils.Setup(u => u.FileExists(It.IsAny<string>())).Returns(false);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type), false);

            Assert.IsNull(model);

            _utils.Verify(u => u.FileExists(ZipPath));
            _utils.Verify(u => u.FileExists(NetworkPath));
        }

        [Test]
        public void ShouldReturnModelIfNetworkExists()
        {
            _utils.Setup(u => u.FileExists(NetworkPath)).Returns(true);
            _utils.Setup(u => u.LoadNetwork(NetworkPath)).Returns(UsageModelFixture.Network);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type));

            AssertCorrectModelLoaded(model);
        }

        [Test]
        public void ShouldReturnModelIfNetworkExistsInZip()
        {
            _utils.Setup(u => u.FileExists(ZipPath)).Returns(true);

            const string content = "CONTENT";
            _utils.Setup(u => u.OpenFile(ZipPath, FileMode.Open, FileAccess.Read))
                  .Returns(GetZipStream(Network, content));

            _utils.Setup(u => u.GetDirectoryName(NetworkPath)).Returns(AssemblyPath);

            var isModelWritten = false;
            _utils.Setup(u => u.WriteAllByte(It.IsAny<byte[]>(), NetworkPath))
                  .Callback<byte[], string>(
                      (bytes, path) =>
                      {
                          if (content.Select(c => (byte) c).SequenceEqual(bytes))
                          {
                              isModelWritten = true;
                          }
                      });

            _utils.Setup(u => u.FileExists(NetworkPath)).Returns(() => isModelWritten);

            _utils.Setup(u => u.LoadNetwork(NetworkPath))
                  .Returns(() => (isModelWritten) ? UsageModelFixture.Network() : null);

            var model = _uut.GetModel(Assembly, new CoReTypeName(Type), true);

            AssertCorrectModelLoaded(model);

            _utils.Verify(u => u.CreateDirectory(AssemblyPath));
        }

        private static void AssertCorrectModelLoaded(UsageModel model)
        {
            var expected = new[]
            {
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Execute()LVoid;"), 0.817),
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Finish()LVoid;"), 0.436),
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Init()LVoid;"), 0.377)
            };

            var actual = model.Query(new Query());

            UsageModelFixture.AssertEqualityIgnoringRoundingErrors(expected, actual);
        }

        private static Stream GetZipStream(string fileName, string content)
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