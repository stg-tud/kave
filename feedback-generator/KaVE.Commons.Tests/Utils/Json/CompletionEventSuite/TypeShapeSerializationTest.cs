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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Json;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    internal class TypeShapeSerializationTest : SerializationTestBase
    {
        [Test]
        public void VerifyToJson()
        {
            var actual = GetExample().ToCompactJson();
            var expected = GetExampleJson_Current();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromCurrentJson()
        {
            var actual = GetExampleJson_Current().ParseJsonTo<ITypeShape>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromJson_Legacy_BeforeRestructuringProjects()
        {
            var actual = GetExampleJson_Legacy_BeforeRestructuringProjects().ParseJsonTo<ITypeShape>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyObjToObjEquality()
        {
            var actual = GetExample().ToCompactJson().ParseJsonTo<ITypeShape>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        private static ITypeShape GetExample()
        {
            return new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get("T,P"),
                    Extends = new TypeHierarchy
                    {
                        Element = TypeName.Get("S,P"),
                    },
                    Implements =
                    {
                        new TypeHierarchy
                        {
                            Element = TypeName.Get("I,P"),
                        }
                    }
                },
                MethodHierarchies =
                {
                    new MethodHierarchy
                    {
                        Element = MethodName.Get("[T,P] [T,P].M1()"),
                        Super = MethodName.Get("[T,P] [T,P].M2()"),
                        First = MethodName.Get("[T,P] [T,P].M3()")
                    }
                }
            };
        }

        private static string GetExampleJson_Current()
        {
            // do not change! keep for checking exception free reading of old formats!
            return
                "{\"$type\":\"KaVE.Commons.Model.TypeShapes.TypeShape, KaVE.Commons\",\"TypeHierarchy\":{\"$type\":\"KaVE.Commons.Model.TypeShapes.TypeHierarchy, KaVE.Commons\",\"Element\":\"CSharp.TypeName:T,P\",\"Extends\":{\"$type\":\"KaVE.Commons.Model.TypeShapes.TypeHierarchy, KaVE.Commons\",\"Element\":\"CSharp.TypeName:S,P\",\"Implements\":[]},\"Implements\":[{\"$type\":\"KaVE.Commons.Model.TypeShapes.TypeHierarchy, KaVE.Commons\",\"Element\":\"CSharp.TypeName:I,P\",\"Implements\":[]}]},\"MethodHierarchies\":[{\"$type\":\"KaVE.Commons.Model.TypeShapes.MethodHierarchy, KaVE.Commons\",\"Element\":\"CSharp.MethodName:[T,P] [T,P].M1()\",\"Super\":\"CSharp.MethodName:[T,P] [T,P].M2()\",\"First\":\"CSharp.MethodName:[T,P] [T,P].M3()\"}]}";
        }

        private static string GetExampleJson_Legacy_BeforeRestructuringProjects()
        {
            // do not change! keep for checking exception free reading of old formats!
            return
                "{\"$type\":\"KaVE.Model.TypeShapes.TypeShape, KaVE.Model\",\"TypeHierarchy\":{\"$type\":\"KaVE.Model.TypeShapes.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.TypeName:T,P\",\"Extends\":{\"$type\":\"KaVE.Model.TypeShapes.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.TypeName:S,P\",\"Implements\":[]},\"Implements\":[{\"$type\":\"KaVE.Model.TypeShapes.TypeHierarchy, KaVE.Model\",\"Element\":\"CSharp.TypeName:I,P\",\"Implements\":[]}]},\"MethodHierarchies\":[{\"$type\":\"KaVE.Model.TypeShapes.MethodHierarchy, KaVE.Model\",\"Element\":\"CSharp.MethodName:[T,P] [T,P].M1()\",\"Super\":\"CSharp.MethodName:[T,P] [T,P].M2()\",\"First\":\"CSharp.MethodName:[T,P] [T,P].M3()\"}]}";
        }
    }
}