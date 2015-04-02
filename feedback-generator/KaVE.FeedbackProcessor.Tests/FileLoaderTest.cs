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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests
{
    [TestFixture]
    public class FileLoaderTest
    {
        private FileLoader _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new FileLoader();
        }

        [Test]
        public void ReadFile()
        {
            var file = GivenAFile()
                .With("0.json", "{\"$type\":\"KaVE.Commons.Model.Events.InfoEvent, KaVE.Commons\",\"Info\":\"A\"}");

            var actuals = ReadFile(file);

            CollectionAssert.AreEqual(new[] {new InfoEvent {Info = "A"}}, actuals);
        }

        [Test]
        public void ReadAllFiles()
        {
            var archive = GivenAFile()
                .With("0.json", "{\"$type\":\"KaVE.Commons.Model.Events.InfoEvent, KaVE.Commons\",\"Info\":\"0\"}")
                .With("1.json", "{\"$type\":\"KaVE.Commons.Model.Events.InfoEvent, KaVE.Commons\",\"Info\":\"1\"}")
                .With("2.json", "{\"$type\":\"KaVE.Commons.Model.Events.InfoEvent, KaVE.Commons\",\"Info\":\"2\"}");

            var actuals = ReadFile(archive);

            Assert.AreEqual(3, actuals.Count());
        }

        private IEnumerable<object> ReadFile(FileBuilder file)
        {
            using (var zipFile = file.Create())
            {
                return _sut.ReadAllEvents(zipFile);
            }
        }

        private static FileBuilder GivenAFile()
        {
            return new FileBuilder();
        }

        public class FileBuilder
        {
            private readonly IDictionary<string, string> _contents = new Dictionary<string, string>();

            public FileBuilder With(String fileName, String content)
            {
                _contents[fileName] = content;
                return this;
            }

            public ZipFile Create()
            {
                var zipFile = new ZipFile();
                foreach (var e in _contents)
                {
                    var fileName = e.Key;
                    var json = e.Value;
                    zipFile.AddEntry(fileName, json);
                }
                var ms = new MemoryStream();
                zipFile.Save(ms);
                ms.Position = 0;
                return ZipFile.Read(ms);
            }
        }
    }
}