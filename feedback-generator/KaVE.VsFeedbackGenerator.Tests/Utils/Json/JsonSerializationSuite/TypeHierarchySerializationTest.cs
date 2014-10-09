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

using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.TestUtils.Model.Names;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite
{
    [TestFixture]
    internal class TypeHierarchySerializationTest : SerializationTestBase
    {
        [Test]
        public void ShouldSerializeTypeHierarchy()
        {
            var uut = new TypeHierarchy("Test.Class")
            {
                Extends = new TypeHierarchy("Another.Test.Class"),
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy("Some.Interface")
                }
            };
            JsonAssert.SerializationPreservesData(uut);
        }

        [Test]
        public void ShouldSerializeToString()
        {
            var typeHierarchy = new TypeHierarchy(TestNameFactory.GetAnonymousTypeName().Identifier)
            {
                Extends = new TypeHierarchy(TestNameFactory.GetAnonymousTypeName().Identifier),
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy(TestNameFactory.GetAnonymousTypeName().Identifier),
                    new TypeHierarchy(TestNameFactory.GetAnonymousTypeName().Identifier)
                }
            };
            const string expected =
                "{\"$type\":\"KaVE.Model.Events.CompletionEvent.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.TypeName:SomeType1, SomeAssembly2, 9.8.7.6\",\"Extends\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.TypeName:SomeType3, SomeAssembly4, 9.8.7.6\",\"Implements\":[]},\"Implements\":[{\"$type\":\"KaVE.Model.Events.CompletionEvent.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.TypeName:SomeType5, SomeAssembly6, 9.8.7.6\",\"Implements\":[]},{\"$type\":\"KaVE.Model.Events.CompletionEvent.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.TypeName:SomeType7, SomeAssembly8, 9.8.7.6\",\"Implements\":[]}]}";

            JsonAssert.SerializesTo(typeHierarchy, expected);
        }
    }
}