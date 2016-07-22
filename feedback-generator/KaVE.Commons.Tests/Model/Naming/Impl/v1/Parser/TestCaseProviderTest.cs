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

using System.IO;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v1.Parser
{
    [TestFixture]
    public class TestCaseProviderTest
    {
        private string file;
        private string[] _fileContent;

        [SetUp]
        public void Init()
        {
            file = Path.Combine(Path.GetTempPath(), "tmp.tsv");
            _fileContent = new[]
            {
                "Identifier\tassembly\tnamespace",
                "",
                "#test",
                "n.T,a\ta\tn.",
            };
            File.WriteAllLines(file, _fileContent);
        }

        [Test]
        public void LoadTestFile()
        {
            var lines = TestCaseProvider.LoadTestFile(file);
            Assert.AreNotEqual(_fileContent, lines);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual("n.T,a\ta\tn.", lines[0]);
        }
    }
}