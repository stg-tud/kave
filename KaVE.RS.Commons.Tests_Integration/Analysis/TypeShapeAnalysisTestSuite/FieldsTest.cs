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
using KaVE.Commons.TestUtils.Model.Naming;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.TypeShapeAnalysisTestSuite
{
    internal class FieldsTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void NoFields()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M()
                    {
                        $
                    }
                }
            ");
            Assert.IsEmpty(ResultContext.TypeShape.Fields);
        }

        [Test]
        public void ShouldRetrieveFields()
        {
            CompleteInNamespace(@"
                public class C
                {
                    private int F1;
                    
                    public string F2;                    

                    public void M()
                    {
                        $
                    }
                }
            ");

            CollectionAssert.AreEqual(
                Sets.NewHashSet(
                    Names.Field("[{0}] [N.C, TestProject].F1", NameFixture.Int),
                    Names.Field("[{0}] [N.C, TestProject].F2", NameFixture.String)),
                ResultContext.TypeShape.Fields);
        }

        [Test]
        public void ShouldRetrieveStaticFields()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public static string F;                    

                    public void M()
                    {
                        $
                    }
                }
            ");

            CollectionAssert.AreEqual(
                Sets.NewHashSet(
                    Names.Field("static [{0}] [N.C, TestProject].F", NameFixture.String)),
                ResultContext.TypeShape.Fields);
        }

        [Test]
        public void ShouldRetrieveFieldsInEnum()
        {
            CompleteInNamespace(@"
                enum E {
                    X,Y,$
                }
            ");
            var type = Names.Type("e:N.E, TestProject");
            CollectionAssert.AreEqual(
                Sets.NewHashSet(
                    Names.Field("[{0}] [{0}].X", type),
                    Names.Field("[{0}] [{0}].Y", type)),
                ResultContext.TypeShape.Fields);
        }

        [Test]
        public void IgnoresAutoPropertyBackingField()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class C
                    {
                        public object P { get; set; }
                        
                        $
                    }
                }");

            Assert.IsEmpty(ResultContext.TypeShape.Fields);
        }
    }
}