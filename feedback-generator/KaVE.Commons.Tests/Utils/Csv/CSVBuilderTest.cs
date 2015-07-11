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

using KaVE.Commons.Utils.Csv;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Csv
{
    [TestFixture]
    internal class CsvBuilderTest
    {
        private CsvBuilder _uut;

        [SetUp]
        public void CreateBuilder()
        {
            _uut = new CsvBuilder();
        }

        [Test]
        public void AddsColumnsAndFields()
        {
            _uut.StartRow();
            _uut["A"] = 1;
            _uut["B"] = 2;
            _uut.StartRow();
            _uut["A"] = 3;
            _uut["B"] = 4;

            CsvAssert.IsEqual(_uut, new[] {"A,B", "1,2", "3,4"});
        }

        [Test]
        public void HandlesMissingValues()
        {
            _uut.StartRow();
            _uut["A"] = 1;
            _uut["B"] = 2;
            _uut.StartRow();
            _uut["A"] = 3;

            CsvAssert.IsEqual(_uut, new[] {"A,B", "1,2", "3,"});
        }

        [Test]
        public void EscapesDelimiter()
        {
            _uut.StartRow();
            _uut["A"] = "1,5";

            CsvAssert.IsEqual(_uut, new[] {"A", @"""1,5"""});
        }

        [Test]
        public void EscapesQuotes()
        {
            _uut.StartRow();
            _uut["A"] = "foo\"bar";

            CsvAssert.IsEqual(_uut, new[] {"A", @"""foo""""bar"""});
        }

        [Test]
        public void SerializesDate()
        {
            _uut.StartRow();
            _uut["A"] = new System.DateTime(2015, 4, 27);

            CsvAssert.IsEqual(_uut, new[] {"A", "2015-04-27"});
        }

        [Test]
        public void SerializesDateTime()
        {
            _uut.StartRow();
            _uut["A"] = new System.DateTime(2015, 4, 27, 13, 14, 15);

            CsvAssert.IsEqual(_uut, new[] {"A", "2015-04-27 13:14:15"});
        }

        [Test]
        public void SortsByFieldName()
        {
            _uut.StartRow();
            _uut["B"] = 2;
            _uut["A"] = 1;
            _uut["Z"] = 4;
            _uut["C"] = 3;

            CsvAssert.IsEqual(_uut, new[] {"A,B,C,Z", "1,2,3,4"}, CsvBuilder.SortFields.ByName);
        }

        [Test]
        public void SortsByFieldNameExcludeFirstField()
        {
            _uut.StartRow();
            _uut["Header"] = 0;
            _uut["A"] = 1;
            _uut["Z"] = 3;
            _uut["C"] = 2;

            CsvAssert.IsEqual(_uut, new[] {"Header,A,C,Z", "0,1,2,3"}, CsvBuilder.SortFields.ByNameLeaveFirst);
        }
    }
}