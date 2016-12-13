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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    internal class TypeShapeSerializationTest
    {
        // please note that the tests are migrated to ExternalSerializationTestSuite
        // Use this class as a test data generator in case of changes (set a break point).

        [Test]
        public void SmokeTest()
        {
            var sut = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = Names.Type("T,P"),
                    Extends = new TypeHierarchy
                    {
                        Element = Names.Type("S,P")
                    },
                    Implements =
                    {
                        new TypeHierarchy
                        {
                            Element = Names.Type("I,P")
                        }
                    }
                },
                NestedTypes =
                {
                    Names.Type("T2,P")
                },
                Delegates =
                {
                    Names.Type("d:[T3,P] [T3,P].M([p:int] p)").AsDelegateTypeName
                },
                EventHierarchies =
                {
                    new EventHierarchy
                    {
                        Element = Names.Event("[T,P] [T,P].E1"),
                        Super = Names.Event("[T,P] [T,P].E2"),
                        First = Names.Event("[T,P] [T,P].E3")
                    }
                },
                Fields =
                {
                    Names.Field("[T1,P] [T2,P]._f")
                },
                MethodHierarchies =
                {
                    new MethodHierarchy
                    {
                        Element = Names.Method("[T,P] [T,P].M1()"),
                        Super = Names.Method("[T,P] [T,P].M2()"),
                        First = Names.Method("[T,P] [T,P].M3()")
                    }
                },
                PropertyHierarchies =
                {
                    new PropertyHierarchy
                    {
                        Element = Names.Property("get set [T,P] [T,P].P1()"),
                        Super = Names.Property("get set [T,P] [T,P].P2()"),
                        First = Names.Property("get set [T,P] [T,P].P3()")
                    }
                }
            };

            var compact = sut.ToCompactJson();
            var formatted = sut.ToFormattedJson();

            // set breakpoint here and copy string to files
        }
    }
}