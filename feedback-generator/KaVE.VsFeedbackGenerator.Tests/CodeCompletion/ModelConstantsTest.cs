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
 *    - Uli Fahrer
 */

using System;
using KaVE.Model.ObjectUsage;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.CodeCompletion
{
    [TestFixture]
    internal class ModelConstantsTest
    {
        [Test]
        public void ClassContext()
        {
            var classCtx = new CoReTypeName("LStrangeType");

            var actual = ModelConstants.NewClassContext(classCtx);
            const string expected = "LStrangeType";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodContext()
        {
            var methodCtx = new CoReMethodName("LType.M()LVoid;");

            var actual = ModelConstants.NewMethodContext(methodCtx);
            const string expected = "LType.M()LVoid;";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InitDefinition()
        {
            const string name = "LType.Create()LType;";

            var definition = DefinitionSites.CreateDefinitionByConstructor(name);

            var actual = ModelConstants.NewDefinition(definition);
            var expected = String.Format("INIT:{0}", name);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodReturnDefinition()
        {
            const string name = "LType.M()LType;";

            var definition = DefinitionSites.CreateDefinitionByReturn(name);

            var actual = ModelConstants.NewDefinition(definition);
            var expected = string.Format("RETURN:{0}", name);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParameterDefinition()
        {
            var definition = DefinitionSites.CreateDefinitionByParam("LType.M(LOtherType;)LType;", 0);

            var actual = ModelConstants.NewDefinition(definition);
            const string expected = "PARAM(0):LType.M(LOtherType;)LType;";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FieldDefinition()
        {
            var definition = DefinitionSites.CreateDefinitionByField("LType.object;LType");

            var actual = ModelConstants.NewDefinition(definition);
            const string expected = "FIELD:LType.object;LType";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void UnknownDefinition()
        {
            var definition = DefinitionSites.CreateUnknownDefinitionSite();

            var actual = ModelConstants.NewDefinition(definition);
            const string expected = "UNKNOWN";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParameterSite()
        {
            var callSite = CallSites.CreateParameterCallSite("LType.M()LVoid;", 2345);

            var actual = ModelConstants.NewParameterSite(callSite);
            const string expected = "P_LType.M()LVoid;#2345";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CallSite()
        {
            var callSite = CallSites.CreateReceiverCallSite("LType.M2()LVoid;");

            var actual = ModelConstants.NewReceiverCallSite(callSite);
            const string expected = "C_LType.M2()LVoid;";

            Assert.AreEqual(expected, actual);
        }
    }
}
