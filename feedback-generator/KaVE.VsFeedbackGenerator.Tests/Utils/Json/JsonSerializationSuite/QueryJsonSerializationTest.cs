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
using JetBrains.Util;
using KaVE.Model.ObjectUsage;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite
{
    [TestFixture]
    internal class QueryJsonSerializationTest
    {
        [Test]
        public void ShouldSerializeAndDeserializeQuery()
        {
            var expected = Fixture;
            var serialized = expected.ToJson();
            var actual = serialized.ParseJsonToQuery();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSerializeToEqualString()
        {
            const string expected =
                @"{""type"":""Lusages/Query"",""classCtx"":""LContext"",""methodCtx"":""LReceiver.equals(LArgument;)LResult;"",""definition"":{""kind"":""THIS"",""field"":""LField.field;LType"",""method"":""LDefiner.define(LScheme;)LPattern;"",""argIndex"":42},""sites"":[{""kind"":""PARAMETER"",""method"":""LCallSite.param(LParam;)LReturn;"",""argIndex"":23},{""kind"":""RECEIVER"",""method"":""LCallSite.param(LParam;)LReturn;""}]}";
            var actual = Fixture.ToJson();

            Assert.AreEqual(expected, actual);
        }

        private static Query Fixture
        {
            get
            {
                var query = new Query
                {
                    type = new CoReTypeName("Lusages/Query"),
                    classCtx = new CoReTypeName("LContext"),
                    methodCtx = new CoReMethodName("LReceiver.equals(LArgument;)LResult;"),
                    definition = new DefinitionSite
                    {
                        kind = DefinitionSiteKind.THIS,
                        method = new CoReMethodName("LDefiner.define(LScheme;)LPattern;"),
                        field = new CoReFieldName("LField.field;LType"),
                        argIndex = 42
                    }
                };
                query.sites.AddRange(
                    new List<CallSite>
                    {
                        new CallSite
                        {
                            kind = CallSiteKind.PARAMETER,
                            method = new CoReMethodName("LCallSite.param(LParam;)LReturn;"),
                            argIndex = 23
                        },
                        new CallSite
                        {
                            kind = CallSiteKind.RECEIVER,
                            method = new CoReMethodName("LCallSite.param(LParam;)LReturn;"),
                            argIndex = 0
                        }
                    });
                return query;
            }
        }
    }
}