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

using KaVE.Commons.TestUtils.Utils.Csv;
using KaVE.Commons.Utils.Csv;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Csv
{
    [TestFixture]
    class ReadCsvTest
    {
        [Test]
        public void ReadsRows()
        {
            var actual = CsvTable.Read(new[] {"A,B", "1,2", "3,4"}.ToCsv());

            Assert.AreEqual(2, actual.Rows.Count);
        }

        [Test]
        public void UsesFieldNames()
        {
            var actual = CsvTable.Read(new[] {"A,B","1,2"}.ToCsv());

            Assert.AreEqual("1", actual.Rows[0]["A"]);
            Assert.AreEqual("2", actual.Rows[0]["B"]);
        }

        [Test]
        public void UsesSpecifiedFieldDelimiter()
        {
            var actual = CsvTable.Read(new[] {"A;B","1;2"}.ToCsv(), ';');

            Assert.AreEqual("1", actual.Rows[0]["A"]);
            Assert.AreEqual("2", actual.Rows[0]["B"]);
        }
    }
}
