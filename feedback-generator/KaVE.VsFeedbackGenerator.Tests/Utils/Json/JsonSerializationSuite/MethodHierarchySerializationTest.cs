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
 *    - Dennis Albrecht
 */

using KaVE.Model.Events.CompletionEvent;
using KaVE.TestUtils.Model.Names;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite
{
    [TestFixture]
    internal class MethodHierarchySerializationTest : SerializationTestBase
    {
        [Test]
        public void ShouldSerializeMethodHierarchy()
        {
            var uut = new MethodHierarchy(TestNameFactory.GetAnonymousMethodName())
            {
                First = TestNameFactory.GetAnonymousMethodName(),
                Super = TestNameFactory.GetAnonymousMethodName()
            };
            JsonAssert.SerializationPreservesData(uut);
        }

        [Test]
        public void ShouldSerializeToString()
        {
            var methodHierarchy = new MethodHierarchy(TestNameFactory.GetAnonymousMethodName())
            {
                Super = TestNameFactory.GetAnonymousMethodName(),
                First = TestNameFactory.GetAnonymousMethodName()
            };
            const string expected =
                "{\"$type\":\"KaVE.Model.Events.CompletionEvent.MethodHierarchy, KaVE.Model\",\"Element\":\"CSharp.MethodName:[SomeType1, SomeAssembly2, 9.8.7.6] [SomeType3, SomeAssembly4, 9.8.7.6].Method5()\",\"Super\":\"CSharp.MethodName:[SomeType6, SomeAssembly7, 9.8.7.6] [SomeType8, SomeAssembly9, 9.8.7.6].Method10()\",\"First\":\"CSharp.MethodName:[SomeType11, SomeAssembly12, 9.8.7.6] [SomeType13, SomeAssembly14, 9.8.7.6].Method15()\"}";

            JsonAssert.SerializesTo(methodHierarchy, expected);
        }
    }
}