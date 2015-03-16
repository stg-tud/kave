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

using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture, Ignore]
    internal class ContextAnalysisCompletionTargetTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void ShouldBeReference()
        {
            CompleteInMethod(@"
                System.Collections.IList list;
                list.$
            ");

            var expected = LocalVariableName.Get("[i:System.Collections.IList, mscorlib, 4.0.0.0] list");
            //var actual = ResultContext.TriggerTarget;
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeReferenceWhenPrefixIsTyped()
        {
            CompleteInMethod(@"
                System.Collections.IList list;
                list.Add$
            ");

            var expected = LocalVariableName.Get("[i:System.Collections.IList, mscorlib, 4.0.0.0] list");
            //var actual = ResultContext.TriggerTarget;
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeNullIfNotSpecifiedInMethod()
        {
            CompleteInMethod(@"
                $
            ");

            //var actual = ResultContext.TriggerTarget;
            //Assert.IsNull(actual);
        }

        [Test]
        public void ShouldBeNullIfNotSpecifiedInClass()
        {
            CompleteInClass(@"
                $
            ");

            //var actual = ResultContext.TriggerTarget;
            //Assert.IsNull(actual);
        }

        [Test]
        public void ShouldBeEnclosingTypeIfExplicitThisIsSpecified()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class C
                    {
                        public void M()
                        {
                            this.$
                        }
                    }
                }
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("N.C, TestProject");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeSuperTypeIfExplicitBaseIsSpecified()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    class S {}

                    class C : S
                    {
                        public void M()
                        {
                            base.$
                        }
                    }
                }
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("N.S, TestProject");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeCastType()
        {
            CompleteInMethod(@"
                object o;
                ((System.Collections.IList) o).$
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("i:System.Collections.IList, mscorlib, 4.0.0.0");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeSafeCastType()
        {
            CompleteInMethod(@"
                object o;
                (o as System.Collections.IList).$
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("i:System.Collections.IList, mscorlib, 4.0.0.0");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeReturnType()
        {
            CompleteInClass(@"
                public System.Collections.IList GetList() {}
                
                public void M()
                {
                    GetList().$
                }");

            //var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("i:System.Collections.IList, mscorlib, 4.0.0.0");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeInstantiatedType()
        {
            CompleteInMethod(@"
                (new object()).$
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Object, mscorlib, 4.0.0.0");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeField()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    private string Field;
                
                    public void M()
                    {
                        Field.$
                    }
                }");

            //var actual = ResultContext.TriggerTarget;
            var expected = FieldName.Get("[System.String, mscorlib, 4.0.0.0] [C, TestProject].Field");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeConstLocalVariable()
        {
            CompleteInMethod(@"
                const string Const;
                Const.$
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = LocalVariableName.Get("[System.String, mscorlib, 4.0.0.0] Const");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeNamespace()
        {
            CompleteInMethod(@"
                System.$
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = NamespaceName.Get("System");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIgnoreWhitespaces()
        {
            CompleteInMethod(@"
                object o;
                o.
                    $
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = LocalVariableName.Get("[System.Object, mscorlib, 4.0.0.0] o");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIgnoreWhitespacesBeforePrefix()
        {
            CompleteInMethod(@"
                object o;
                o.
                    Equ$
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = LocalVariableName.Get("[System.Object, mscorlib, 4.0.0.0] o");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIgnorePreceedingCompleteExpression()
        {
            CompleteInMethod(@"
                object o;
                o.GetHashCode();
                $
            ");

            //var actual = ResultContext.TriggerTarget;
            //Assert.IsNull(actual);
        }

        [Test]
        public void ShouldBeReferencedType()
        {
            CompleteInMethod(@"
                object.$
            ");

            //var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Object, mscorlib, 4.0.0.0");
            //Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeParameter()
        {
            CompleteInClass(@"
                void M(object param)
                {
                    param.$
                }");

            var expected = ParameterName.Get("[System.Object, mscorlib, 4.0.0.0] param");
            //var actual = ResultContext.TriggerTarget;
            //Assert.AreEqual(expected, actual);
        }
    }
}