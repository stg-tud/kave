﻿using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisTargetTypeTest : KaVEBaseTest
    {
        [Test]
        public void ShouldBeTypeOfReference()
        {
            CompleteInMethod(@"
                System.Collections.IList list;
                list.$
            ");

            var expected = TypeName.Get("System.Collections.IList, mscorlib, 4.0.0.0");
            var actual = ResultContext.TriggerTarget;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeTypeOfReferenceWhenPrefixIsTyped()
        {
            CompleteInMethod(@"
                System.Collections.IList list;
                list.Add$
            ");

            var expected = TypeName.Get("System.Collections.IList, mscorlib, 4.0.0.0");
            var actual = ResultContext.TriggerTarget;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeNullIfNotSpecifiedInMethod()
        {
            CompleteInMethod(@"
                $
            ");

            var actual = ResultContext.TriggerTarget;
            Assert.IsNull(actual);
        }

        [Test]
        public void ShouldBeNullIfNotSpecifiedInClass()
        {
            CompleteInClass(@"
                $
            ");

            var actual = ResultContext.TriggerTarget;
            Assert.IsNull(actual);
        }

        [Test]
        public void ShouldBeEnclosingTypeIfExplicitThisIsSpecified()
        {
            CompleteInFile(@"
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
            
            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("N.C, TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeSuperTypeIfExplicitBaseIsSpecified()
        {
            CompleteInFile(@"
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

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("N.S, TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeCastType()
        {
            CompleteInMethod(@"
                object o;
                ((System.Collections.IList) o).$
            ");

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Collections.IList, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeSafeCastType()
        {
            CompleteInMethod(@"
                object o;
                (o as System.Collections.IList).$
            ");

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Collections.IList, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
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

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Collections.IList, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeInstantiatedType()
        {
            CompleteInMethod(@"
                (new object()).$
            ");

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Object, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeFieldValueType()
        {
            CompleteInClass(@"
                private string Field;
                
                public void M()
                {
                    Field.$
                }");

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.String, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeConstType()
        {
            CompleteInMethod(@"
                const string Const;
                Const.$
            ");

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.String, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeNamespace()
        {
            CompleteInMethod(@"
                System.$
            ");

            var actual = ResultContext.TriggerTarget;
            var expected = NamespaceName.Get("System");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIgnoreWhitespaces()
        {
            CompleteInMethod(@"
                object o;
                o.
                    $
            ");

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Object, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIgnoreWhitespacesBeforePrefix()
        {
            CompleteInMethod(@"
                object o;
                o.
                    Equ$
            ");

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Object, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIgnorePreceedingCompleteExpression()
        {
            CompleteInMethod(@"
                object o;
                o.GetHashCode();
                $
            ");

            var actual = ResultContext.TriggerTarget;
            Assert.IsNull(actual);
        }

        [Test]
        public void ShouldBeReferencedType()
        {
            CompleteInMethod(@"
                object.$
            ");

            var actual = ResultContext.TriggerTarget;
            var expected = TypeName.Get("System.Object, mscorlib, 4.0.0.0");
            Assert.AreEqual(expected, actual);
        }
    }
}