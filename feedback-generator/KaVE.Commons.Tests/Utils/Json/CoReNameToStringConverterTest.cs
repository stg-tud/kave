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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json
{
    internal class CoReNameToStringConverterTest
    {
        [Test]
        public void TypeName_Write()
        {
            const string expected = "LP/T";
            var actual = new CoReTypeName(expected).ToFormattedJson();
            Assert.AreEqual(_(expected), actual);
        }

        [Test]
        public void TypeName_Read()
        {
            const string id = "LP/T";
            var actual = _(id).ParseJsonTo<CoReTypeName>();
            var expected = new CoReTypeName(id);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FieldName_Write()
        {
            //"Lorg/eclipse/birt/chart/ui/integrate/SimpleHyperlinkBuilder$2.this$0;Lorg/eclipse/birt/chart/ui/integrate/SimpleHyperlinkBuilder"
            const string expected = "LP/Decl.f;LF";
            var actual = new CoReFieldName(expected).ToFormattedJson();
            Assert.AreEqual(_(expected), actual);
        }

        [Test]
        public void FieldName_Read()
        {
            const string id = "LP/Decl.f;LF";
            var actual = _(id).ParseJsonTo<CoReFieldName>();
            var expected = new CoReFieldName(id);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodName_Write()
        {
            const string expected = "LP/T.m()LV;";
            var actual = new CoReMethodName(expected).ToFormattedJson();
            Assert.AreEqual(_(expected), actual);
        }

        [Test]
        public void MethodName_Read()
        {
            const string id = "LP/T.m()LV;";
            var actual = _(id).ParseJsonTo<CoReMethodName>();
            var expected = new CoReMethodName(id);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CallSiteKindSerialization()
        {
            const CallSiteKind obj = CallSiteKind.RECEIVER;
            const string json = "\"RECEIVER\"";

            var actualJson = obj.ToFormattedJson();
            Assert.AreEqual(json, actualJson);
            var other = json.ParseJsonTo<CallSiteKind>();
            Assert.AreEqual(obj, other);
        }

        [Test]
        public void DefinitionSiteKindSerialization()
        {
            const DefinitionSiteKind obj = DefinitionSiteKind.RETURN;
            const string json = "\"RETURN\"";

            var actualJson = obj.ToFormattedJson();
            Assert.AreEqual(json, actualJson);
            var other = json.ParseJsonTo<DefinitionSiteKind>();
            Assert.AreEqual(obj, other);
        }

        [Test]
        public void CallSiteSerialization()
        {
            var obj = new CallSite
            {
                argIndex = 3,
                kind = CallSiteKind.RECEIVER,
                method = Names.Method("[R,P] [T,P].M()").ToCoReName()
            };

            var json = "{" + Environment.NewLine +
                       "    \"kind\": \"RECEIVER\"," + Environment.NewLine +
                       "    \"method\": \"LT.M()LR;\"," + Environment.NewLine +
                       "    \"argIndex\": 3" + Environment.NewLine +
                       "}";

            var actualJson = obj.ToFormattedJson();
            Assert.AreEqual(json, actualJson);
            var other = json.ParseJsonTo<CallSite>();
            Assert.AreEqual(obj, other);
        }

        [Test]
        public void CallSiteSerialization_DefaultArgIndexIsNotSerialized()
        {
            var obj = new CallSite
            {
                kind = CallSiteKind.RECEIVER,
                method = Names.Method("[R,P] [T,P].M()").ToCoReName()
            };

            var json = "{" + Environment.NewLine +
                       "    \"kind\": \"RECEIVER\"," + Environment.NewLine +
                       "    \"method\": \"LT.M()LR;\"" + Environment.NewLine +
                       "}";

            var actualJson = obj.ToFormattedJson();
            Assert.AreEqual(json, actualJson);
            var other = json.ParseJsonTo<CallSite>();
            Assert.AreEqual(obj, other);
        }

        [Test]
        public void DefinitionSiteSerialization()
        {
            var obj = new DefinitionSite
            {
                kind = DefinitionSiteKind.THIS,
                field = Names.Field("[F,P] [T,P].f").ToCoReName(),
                method = Names.Method("[R,P] [T,P].M()").ToCoReName(),
                argIndex = 4
            };

            var json = "{" + Environment.NewLine +
                       "    \"kind\": \"THIS\"," + Environment.NewLine +
                       "    \"field\": \"LT.f;LF\"," + Environment.NewLine +
                       "    \"method\": \"LT.M()LR;\"," + Environment.NewLine +
                       "    \"argIndex\": 4" + Environment.NewLine +
                       "}";

            var actualJson = obj.ToFormattedJson();
            Assert.AreEqual(json, actualJson);
            var other = json.ParseJsonTo<DefinitionSite>();
            Assert.AreEqual(obj, other);
        }

        [Test]
        public void DefinitionSiteSerialization_NullValues()
        {
            var obj = new DefinitionSite
            {
                kind = DefinitionSiteKind.THIS,
                field = null,
                method = null
            };

            var json = "{" + Environment.NewLine +
                       "    \"kind\": \"THIS\"" + Environment.NewLine +
                       "}";

            var actualJson = obj.ToFormattedJson();
            Assert.AreEqual(json, actualJson);
            var other = json.ParseJsonTo<DefinitionSite>();
            Assert.AreEqual(obj, other);
        }

        [Test]
        public void DefinitionSiteSerialization_DefaultArgIndexIsNotSerialized()
        {
            var obj = new DefinitionSite
            {
                kind = DefinitionSiteKind.THIS,
                field = Names.Field("[F,P] [T,P].f").ToCoReName(),
                method = Names.Method("[R,P] [T,P].M()").ToCoReName()
            };

            var json = "{" + Environment.NewLine +
                       "    \"kind\": \"THIS\"," + Environment.NewLine +
                       "    \"field\": \"LT.f;LF\"," + Environment.NewLine +
                       "    \"method\": \"LT.M()LR;\"" + Environment.NewLine +
                       "}";

            var actualJson = obj.ToFormattedJson();
            Assert.AreEqual(json, actualJson);
            var other = json.ParseJsonTo<DefinitionSite>();
            Assert.AreEqual(obj, other);
        }

        [Test]
        public void QuerySerialization_Simple()
        {
            var obj = new Query
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
                },
                sites =
                {
                    new CallSite
                    {
                        kind = CallSiteKind.PARAMETER,
                        method = new CoReMethodName("LCallSite.param(LParam;)LReturn;"),
                        argIndex = 23
                    }
                }
            };

            var json = obj.ToFormattedJson();
            var other = json.ParseJsonTo<Query>();
            Assert.AreEqual(obj, other);
        }

        [Test]
        public void QuerySerialization_JavaExample()
        {
            // data is exported from Java, do not change!

            var obj = new Query
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
                },
                sites =
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
                }
            };

            const string jsonOriginal =
                @"{""type"":""Lusages/Query"",""classCtx"":""LContext"",""methodCtx"":""LReceiver.equals(LArgument;)LResult;"",""definition"":{""kind"":""THIS"",""field"":""LField.field;LType"",""method"":""LDefiner.define(LScheme;)LPattern;"",""argIndex"":42},""sites"":[{""kind"":""PARAMETER"",""method"":""LCallSite.param(LParam;)LReturn;"",""argIndex"":23},{""kind"":""RECEIVER"",""method"":""LCallSite.param(LParam;)LReturn;""}]}";

            string json = "{" + Environment.NewLine +
                          "    \"$type\": \"KaVE.Commons.Model.ObjectUsage.Query, KaVE.Commons\"," + Environment.NewLine +
                          "    \"type\": \"Lusages/Query\"," + Environment.NewLine +
                          "    \"classCtx\": \"LContext\"," + Environment.NewLine +
                          "    \"methodCtx\": \"LReceiver.equals(LArgument;)LResult;\"," + Environment.NewLine +
                          "    \"definition\": {" + Environment.NewLine +
                          "        \"kind\": \"THIS\"," + Environment.NewLine +
                          "        \"field\": \"LField.field;LType\"," + Environment.NewLine +
                          "        \"method\": \"LDefiner.define(LScheme;)LPattern;\"," + Environment.NewLine +
                          "        \"argIndex\": 42" + Environment.NewLine +
                          "    }," + Environment.NewLine +
                          "    \"sites\": [" + Environment.NewLine +
                          "        {" + Environment.NewLine +
                          "            \"kind\": \"PARAMETER\"," + Environment.NewLine +
                          "            \"method\": \"LCallSite.param(LParam;)LReturn;\"," + Environment.NewLine +
                          "            \"argIndex\": 23" + Environment.NewLine +
                          "        }," + Environment.NewLine +
                          "        {" + Environment.NewLine +
                          "            \"kind\": \"RECEIVER\"," + Environment.NewLine +
                          "            \"method\": \"LCallSite.param(LParam;)LReturn;\"" + Environment.NewLine +
                          "        }" + Environment.NewLine +
                          "    ]" + Environment.NewLine +
                          "}";


            var actualJson = obj.ToFormattedJson();
            Assert.AreEqual(json, actualJson);
            var other = json.ParseJsonTo<Query>();
            Assert.AreEqual(obj, other);
        }

        private string _(string input)
        {
            return "\"" + input + "\"";
        }
    }
}