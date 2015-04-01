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
 *    - Sven Amann
 */

using System.IO;
using KaVE.Commons.Utils.Streams;
using KaVE.VsFeedbackGenerator.Utils.Json;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    [TestFixture]
    public class JsonLogWriterTest
    {
        private ILogWriter<object> _writer;
        private MemoryStream _logStream;

        [SetUp]
        public void SetUpWriter()
        {
            _logStream = new MemoryStream();
            _writer = new JsonLogWriter<object>(_logStream);
        }

        [TearDown]
        public void CleanUp()
        {
            _writer.Dispose();
        }

        [Test]
        public void ShouldSerializeInstance()
        {
            var instance = new SerializationTestTarget { Id = "lalalaloooo" };

            _writer.Write(instance);
            
            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"lalalaloooo\"}\r\n", serialization);
        }

        [Test]
        public void ShouldSerializeMultipleInstances()
        {
            var instance1 = new SerializationTestTarget { Id = "foo" };
            var instance2 = new SerializationTestTarget { Id = "bar" };

            _writer.Write(instance1);
            _writer.Write(instance2);

            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"foo\"}\r\n{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"bar\"}\r\n", serialization);
        }

        [Test]
        public void ShouldSerializeMultipleInstancesIfProvidedAtOnce()
        {
            var instance1 = new SerializationTestTarget { Id = "foo" };
            var instance2 = new SerializationTestTarget { Id = "bar" };

            _writer.WriteAll(new[] { instance1, instance2 });

            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"foo\"}\r\n{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"bar\"}\r\n", serialization);
        }

        [Test]
        public void ShouldAppendToStreamWrittenByOtherWriter()
        {
            var writer = new JsonLogWriter<object>(_logStream);
            writer.Write(new SerializationTestTarget { Id = "foo" });
            
            _writer.Write(new SerializationTestTarget{Id = "bar"});

            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"foo\"}\r\n{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"bar\"}\r\n", serialization);
        }
    }
}
