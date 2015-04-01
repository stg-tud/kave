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
 *    - Uli Fahrer
 */

using KaVE.Model.ObjectUsage;
using NUnit.Framework;

namespace KaVE.Model.Tests.ObjectUsage
{
    [TestFixture]
    internal class QueryTest
    {
        [Test]
        public void ShouldRecognizeEqualQuerys()
        {
            var expected = new Query
            {
                type = new CoReTypeName("LType"),
                definition = DefinitionSites.CreateDefinitionByReturn("LFactory.method()LType;"),
                classCtx = new CoReTypeName("LClass"),
                methodCtx = new CoReMethodName("LReceiver.method(LArgument;)LReturn;"),
            };

            expected.sites.Add(CallSites.CreateParameterCallSite("LReceiver.method(LType;)LReturn;", 3));
            expected.sites.Add(CallSites.CreateParameterCallSite("LType.method(LArgument;)LReturn;", 0));

            var actual = new Query
            {
                type = new CoReTypeName("LType"),
                definition = DefinitionSites.CreateDefinitionByReturn("LFactory.method()LType;"),
                classCtx = new CoReTypeName("LClass"),
                methodCtx = new CoReMethodName("LReceiver.method(LArgument;)LReturn;")
            };

            actual.sites.Add(CallSites.CreateParameterCallSite("LReceiver.method(LType;)LReturn;", 3));
            actual.sites.Add(CallSites.CreateParameterCallSite("LType.method(LArgument;)LReturn;", 0));

            Assert.AreEqual(expected, actual);
        }
    }
}