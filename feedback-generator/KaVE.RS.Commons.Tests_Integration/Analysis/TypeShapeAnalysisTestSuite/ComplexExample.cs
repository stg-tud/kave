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
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.TypeShapeAnalysisTestSuite
{
    internal class ComplexExample : BaseCSharpCodeCompletionTest
    {
        // please not, changes should also be reflected in the "AnalysisTestCases" repository
        private readonly string _src = @"
            namespace BasicCases
            {
                // should not occur in hierarchies
                public class TypeShapeOther
                {
                    public delegate void D();

                    public int F;

                    public virtual int P { get; set; }
                    public virtual event Action E;

                    public virtual void M() {}
                }

                public class TypeShapeFirst : TypeShapeOther
                {
                    public new virtual int P { get; set; }
                    public new virtual event Action E;

                    public new virtual void M() {}
                }

                public class TypeShapeSuper : TypeShapeFirst
                {
                    public override int P { get; set; }
                    public override event Action E;

                    public override void M() {}
                }

                public class TypeShapeElem : TypeShapeSuper
                {
                    public new delegate void D(); // cannot override
                    public new int F; // cannot override

                    $

                    public override int P { get; set; }
                    public override event Action E;

                    public override void M() {}

                    public class N {}

                    public static class SN {}
                }
            }";

        [Test]
        public void TypeHierarchy()
        {
            CompleteInNamespace(_src);

            var actual = ResultContext.TypeShape.TypeHierarchy;
            var expected = new TypeHierarchy
            {
                Element = Names.Type("N.BasicCases.TypeShapeElem, TestProject"),
                Extends = new TypeHierarchy
                {
                    Element = Names.Type("N.BasicCases.TypeShapeSuper, TestProject"),
                    Extends = new TypeHierarchy
                    {
                        Element = Names.Type("N.BasicCases.TypeShapeFirst, TestProject"),
                        Extends = new TypeHierarchy
                        {
                            Element = Names.Type("N.BasicCases.TypeShapeOther, TestProject")
                        }
                    }
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NestedTypes()
        {
            CompleteInNamespace(_src);

            var actuals = ResultContext.TypeShape.NestedTypes;
            var expecteds = Sets.NewHashSet<ITypeHierarchy>(
                new TypeHierarchy("N.BasicCases.TypeShapeElem+N, TestProject"),
                new TypeHierarchy("N.BasicCases.TypeShapeElem+SN, TestProject"));
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void Delegates()
        {
            CompleteInNamespace(_src);

            var actuals = ResultContext.TypeShape.Delegates;
            var expecteds =
                Sets.NewHashSet(
                    Names.Type("d:[p:void] [N.BasicCases.TypeShapeElem+D, TestProject].()").AsDelegateTypeName);
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void Events()
        {
            CompleteInNamespace(_src);

            var actuals = ResultContext.TypeShape.EventHierarchies;
            var expecteds = Sets.NewHashSet<IMemberHierarchy<IEventName>>(
                new EventHierarchy
                {
                    Element = Names.Event("[{0}] [N.BasicCases.TypeShapeElem, TestProject].E", Fix.Action),
                    Super = Names.Event("[{0}] [N.BasicCases.TypeShapeSuper, TestProject].E", Fix.Action),
                    First = Names.Event("[{0}] [N.BasicCases.TypeShapeFirst, TestProject].E", Fix.Action)
                });
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void Fields()
        {
            CompleteInNamespace(_src);

            var actuals = ResultContext.TypeShape.Fields;
            var expecteds = Sets.NewHashSet(Names.Field("[p:int] [N.BasicCases.TypeShapeElem, TestProject].F"));
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void Methods()
        {
            CompleteInNamespace(_src);

            var actuals = ResultContext.TypeShape.MethodHierarchies;
            var expecteds = Sets.NewHashSet<IMemberHierarchy<IMethodName>>(
                new MethodHierarchy
                {
                    Element = Names.Method("[p:void] [N.BasicCases.TypeShapeElem, TestProject].M()"),
                    Super = Names.Method("[p:void] [N.BasicCases.TypeShapeSuper, TestProject].M()"),
                    First = Names.Method("[p:void] [N.BasicCases.TypeShapeFirst, TestProject].M()")
                });
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void Properties()
        {
            CompleteInNamespace(_src);

            var actuals = ResultContext.TypeShape.PropertyHierarchies;
            var expecteds = Sets.NewHashSet<IMemberHierarchy<IPropertyName>>(
                new PropertyHierarchy
                {
                    Element = Names.Property("set get [p:int] [N.BasicCases.TypeShapeElem, TestProject].P()"),
                    Super = Names.Property("set get [p:int] [N.BasicCases.TypeShapeSuper, TestProject].P()"),
                    First = Names.Property("set get [p:int] [N.BasicCases.TypeShapeFirst, TestProject].P()")
                });
            Assert.AreEqual(expecteds, actuals);
        }
    }
}