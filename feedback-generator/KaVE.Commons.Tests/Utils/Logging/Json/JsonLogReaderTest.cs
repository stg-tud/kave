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
using System.Linq;
using KaVE.Commons.Tests.Utils.Json;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.Logging;
using KaVE.Commons.Utils.Logging.Json;
using KaVE.Commons.Utils.Streams;
using KaVE.JetBrains.Annotations;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Logging.Json
{
    public class JsonLogReaderTest
    {
        private Mock<ILogger> _mockLogger;

        [SetUp]
        public void SetUpRegistry()
        {
            _mockLogger = new Mock<ILogger>();
        }

        [Test]
        public void ShouldDeserializeInstance()
        {
            SerializationTestTarget instance;
            using (var reader = CreateReader("{\"Id\":\"0xDEADBEEF\"}"))
            {
                instance = reader.ReadNext();
            }

            Assert.AreEqual("0xDEADBEEF", instance.Id);
        }

        [Test]
        public void ShouldDeserializeMultipleInstancesOneAtATime()
        {
            SerializationTestTarget instance1;
            SerializationTestTarget instance2;
            using (var reader = CreateReader("{\"Id\":\"A\"}\r\n{\"Id\":\"B\"}\r\n"))
            {
                instance1 = reader.ReadNext();
                instance2 = reader.ReadNext();
            }

            Assert.AreEqual("A", instance1.Id);
            Assert.AreEqual("B", instance2.Id);
        }

        [Test]
        public void ShouldReadNullFromEmptyStream()
        {
            SerializationTestTarget instance;
            using (var reader = CreateReader(""))
            {
                instance = reader.ReadNext();
            }

            Assert.IsNull(instance);
        }

        [Test]
        public void ShouldIgnoreLeadingWhitespaces()
        {
            SerializationTestTarget instance;
            using (var reader = CreateReader("      {\"Id\":\"Later\"}"))
            {
                instance = reader.ReadNext();
            }

            Assert.AreEqual("Later", instance.Id);
        }

        [Test]
        public void ShouldIgnoreTrailingWhitespaces()
        {
            SerializationTestTarget instance;
            using (var reader = CreateReader("{\"Id\":\"Later\"}   \r\n       "))
            {
                reader.ReadNext();
                instance = reader.ReadNext();
            }

            Assert.IsNull(instance);
        }

        [Test]
        public void ShouldEnumerateLogContent()
        {
            SerializationTestTarget[] instances;
            using (var reader = CreateReader("{\"Id\":\"0\"}\r\n{\"Id\":\"1\"}\r\n{\"Id\":\"2\"}"))
            {
                instances = reader.ReadAll().ToArray();
            }

            Assert.AreEqual("0", instances[0].Id);
            Assert.AreEqual("1", instances[1].Id);
            Assert.AreEqual("2", instances[2].Id);
        }

        [Test]
        public void ShouldSkipOverBrokeLines()
        {
            SerializationTestTarget[] instances;
            using (var reader = CreateReader("{\"Id\":\"0\"}\r\nI'am a broken line\r\n{\"Id\":\"2\"}"))
            {
                instances = reader.ReadAll().ToArray();
            }

            Assert.AreEqual("0", instances[0].Id);
            Assert.AreEqual("2", instances[1].Id);
        }

        [Test]
        public void ShouldReadNothingFromBrokenFile()
        {
            using (var reader = CreateReader("Broken Content\r\nMore broken stuff\r\nAll broken"))
            {
                Assert.IsNull(reader.ReadNext());
            }
        }

        [Test]
        public void ShouldAllowUnixLikeLinebreaks()
        {
            SerializationTestTarget[] instances;
            using (var reader = CreateReader("{\"Id\":\"0\"}\n{\"Id\":\"1\"}"))
            {
                instances = reader.ReadAll().ToArray();
            }

            Assert.AreEqual(2, instances.Length);
        }

        [Test]
        public void ShouldFireErrorEventWhenIgnoringLine()
        {
            using (var reader = CreateReader("broken line"))
            {
                reader.ReadNext();
            }

            _mockLogger.Verify(g => g.Error(It.IsAny<JsonReaderException>(), "broken line"));
        }

        [Test]
        public void ShouldEscapeErrorContent()
        {
            using (var reader = CreateReader("broken line with { curly braces }"))
            {
                reader.ReadNext();
            }

            _mockLogger.Verify(g => g.Error(It.IsAny<JsonReaderException>(), "broken line with {{ curly braces }}"));
        }

        [NotNull]
        private ILogReader<SerializationTestTarget> CreateReader(string input)
        {
            return new JsonLogReader<SerializationTestTarget>(input.AsStream(), _mockLogger.Object);
        }
    }
}