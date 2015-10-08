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
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Utils;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Utils
{
    [TestFixture]
    public class FileHandlerTest
    {
        private FileHandler _uut;

        private Mock<IIoUtils> _ioUtilMock;
        private Mock<ILogger> _errorHandlerMock;

        private readonly string _filePath = Path.GetTempFileName();

        private string _directoryPath;

        [SetUp]
        public void Init()
        {
            _ioUtilMock = new Mock<IIoUtils>();
            Registry.RegisterComponent(_ioUtilMock.Object);

            _errorHandlerMock = new Mock<ILogger>();
            Registry.RegisterComponent(_errorHandlerMock.Object);

            _directoryPath = Path.GetTempPath();
            _uut = new FileHandler(_filePath, _directoryPath);
        }

        [TearDown]
        public void Reset()
        {
            Registry.Clear();
        }

        private static void AssertCollectionAreEqual(IList<IStatistic> expected, IList<IStatistic> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                var expectedStatistic = expected[i];
                var actualStatistic = actual[i];
                CollectionAssert.AreEqual(expectedStatistic.GetCollection(), actualStatistic.GetCollection());
            }
        }

        [Test]
        public void CreateDirectoryOnInitializeWhenDirectoryDoesnotExist()
        {
            _ioUtilMock.Verify(io => io.DirectoryExists(_directoryPath));
            _ioUtilMock.Verify(io => io.CreateDirectory(_directoryPath));
        }

        [Test]
        public void DeleteFileTest()
        {
            _uut.DeleteFile();
            _ioUtilMock.Verify(io => io.DeleteFile(_filePath));
        }

        [Test]
        public void IfContentIsNullNothingIsWritten()
        {
            _uut.WriteContentToFile(null);
            _ioUtilMock.Verify(io => io.WriteAllByte(It.IsAny<byte[]>(), It.IsAny<string>()), Times.Never);
        }

        [Test, ExpectedException(typeof (JsonReaderException))]
        public void JsonDeserializationErrorShouldThrowExceptionInReadFromFile()
        {
            var statisticDictionary = new Dictionary<Type, IStatistic>
            {
                {typeof (BuildStatistic), new BuildStatistic()},
                {typeof (CompletionStatistic), new CompletionStatistic()},
                {typeof (GlobalStatistic), new GlobalStatistic()},
                {typeof (SolutionStatistic), new SolutionStatistic()}
            };

            var jsonString = JsonSerialization.JsonSerializeObject(statisticDictionary);

            jsonString = jsonString.Substring(0, 40);

            _ioUtilMock.Setup(io => io.FileExists(_filePath)).Returns(true);
            _ioUtilMock.Setup(io => io.ReadFile(_filePath)).Returns(jsonString);

            _uut.ReadContentFromFile<Dictionary<Type, IStatistic>>();
        }

        [Test]
        public void JsonSerializationTest()
        {
            var statisticDictionary = new Dictionary<Type, IStatistic>
            {
                {typeof (BuildStatistic), new BuildStatistic()},
                {typeof (CompletionStatistic), new CompletionStatistic()},
                {typeof (GlobalStatistic), new GlobalStatistic()},
                {typeof (SolutionStatistic), new SolutionStatistic()}
            };

            var commandStatistic = new CommandStatistic();
            commandStatistic.CommandTypeValues.Add("ShowOptions", 10);
            commandStatistic.CommandTypeValues.Add("{66BD4C1D-3401-4BCC-A942-E4990827E6F7}:8289:", 1000);
            commandStatistic.CommandTypeValues.Add("{5EFC7975-14BC-11CF-9B2B-00AA00573819}:26:Edit.Paste", 10000000);
            commandStatistic.CommandTypeValues.Add("TextControl.Backspace", 1);

            statisticDictionary.Add(typeof (CommandStatistic), commandStatistic);

            var jsonString = JsonSerialization.JsonSerializeObject(statisticDictionary);

            var dictionaryAfterDeserialize =
                JsonSerialization.JsonDeserializeObject<Dictionary<Type, IStatistic>>(jsonString);

            AssertCollectionAreEqual(statisticDictionary.Values.ToList(), dictionaryAfterDeserialize.Values.ToList());
        }

        [Test]
        public void ReadContentFromFileReturnsDeserializedObject()
        {
            const string testString = "This is a Test String";
            var jsonString = JsonSerialization.JsonSerializeObject(testString);

            _ioUtilMock.Setup(io => io.FileExists(_filePath)).Returns(true);
            _ioUtilMock.Setup(io => io.ReadFile(_filePath)).Returns(jsonString);
            var actualOutput = _uut.ReadContentFromFile<string>();
            _ioUtilMock.Verify(io => io.ReadFile(_filePath));

            Assert.AreEqual(testString, actualOutput);
        }

        [Test]
        public void ResetFileTest()
        {
            _uut.ResetFile();
            _ioUtilMock.Verify(io => io.DeleteFile(_filePath));
            _ioUtilMock.Verify(io => io.CreateFile(_filePath));
        }

        [Test]
        public void ShouldSendErrorMessageWhenWritingFails()
        {
            _ioUtilMock.Setup(io => io.OpenFile(_filePath, FileMode.Create, FileAccess.Write))
                       .Throws<UnauthorizedAccessException>();

            _uut.WriteContentToFile("Test String");

            _errorHandlerMock.Verify(
                handler =>
                    handler.Error(It.IsAny<UnauthorizedAccessException>(), "Could not write file"));
        }

        [Test]
        public void WriteContentToFileOpensCorrectFileStream()
        {
            _ioUtilMock.Setup(io => io.FileExists(_filePath)).Returns(true);

            var streamMock = new Mock<Stream>();

            streamMock.Setup(stream => stream.CanWrite).Returns(true);

            _ioUtilMock.Setup(io => io.OpenFile(_filePath, FileMode.Create, FileAccess.Write))
                       .Returns(streamMock.Object);
            _uut.WriteContentToFile("");

            _ioUtilMock.Verify(io => io.OpenFile(_filePath, FileMode.Create, FileAccess.Write));
        }
    }
}