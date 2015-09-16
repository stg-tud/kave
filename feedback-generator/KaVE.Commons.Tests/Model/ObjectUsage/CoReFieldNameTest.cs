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

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.ObjectUsage
{
    internal class CoReFieldNameTest
    {
        [Test]
        public void ShouldRecognizeEqualFieldNames()
        {
            Assert.AreEqual(
                new CoReFieldName("LField.field;LType"),
                new CoReFieldName("LField.field;LType"));
        }

        [TestCase("[System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Constant"),
         TestCase("KaVE.Model.ObjectUsage.Query.type"), TestCase("LType.field;LType;"),
         ExpectedException(typeof (AssertException))]
        public void ShouldRejectInvalidFieldNames(string fieldName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CoReFieldName(fieldName);
        }

        [TestCase("LType.field;LType"), TestCase("LType._field;LType"),
         TestCase("LKaVE/Model/ObjectUsage/Query.type;LKave/Model/ObjectUsage/CoReTypeName"),
         TestCase("LSampleIB/SecuritiesWindow._сandles;LEcng/Collections/SynchronizedDictionary")
            // _candles contains no regular c, but some unicode code!
        ]
        public void ShouldAcceptValidFieldNames(string fieldName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            var f = new CoReFieldName(fieldName);
        }
    }
}