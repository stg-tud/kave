using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisTest : KaVEBaseTest
    {
        [Test]
        public void ShouldRetrieveContext()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");
            Assert.IsNotNull(ResultContext);
        }

        [Test]
        public void ShouldRetrieveCorrectEnclosingMethodDeclaration()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");

            var actual = ResultContext.EnclosingMethod;
            var expected =
                MethodName.Get("[System.Void, mscorlib, Version=4.0.0.0] [TestNamespace.TestClass, TestProject].Doit()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotContainObjectInTypeHierarchy()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");
            Assert.IsNull(ResultContext.EnclosingClassHierarchy.Extends);
        }

        [Test]
        public void ShouldRetrieveEnclosingType()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.EnclosingClassHierarchy.Element;

            var expected = TypeName.Get("TestNamespace.TestClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSuperType()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.EnclosingClassHierarchy.Extends.Element;

            var expected = TypeName.Get("TestNamespace.SuperClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveImplementedInterfaces()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.EnclosingClassHierarchy.Implements;

            var expected = new HashSet<ITypeHierarchy>
            {
                new TypeHierarchy("TestNamespace.AnInterface, TestProject")
            };

            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore]
        public void ShouldRetrieveFirstMethod()
        {
            WhenCodeCompletionIsInvokedInFile("MethodOverrides");
            var actual = ResultContext.EnclosingMethodFirst;
            var expected =
                MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [TestNamespace.AnInterface, TestProject].Doit()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSuperMethod()
        {
            WhenCodeCompletionIsInvokedInFile("MethodOverrides");
            var actual = ResultContext.EnclosingMethodSuper;
            var expected =
                MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [TestNamespace.SuperClass, TestProject].Doit()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSubstitution()
        {
            WhenCodeCompletionIsInvokedInFile("GenericTypeHierarchy");

            var actual = ResultContext.EnclosingClassHierarchy.Extends.Element;
            var expected = TypeName.Get("N.IC`1[[T -> System.Int32, mscorlib, Version=4.0.0.0]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveFreeSubstitution()
        {
            WhenCodeCompletionIsInvokedInFile("GenericTypeHierarchy");

            var actual = ResultContext.EnclosingClassHierarchy.Element;
            var expected = TypeName.Get("N.C`1[[T -> ?]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveLargeHierarchy()
        {
            WhenCodeCompletionIsInvokedInFile("LargeHierarchy");
            var actual = ResultContext.EnclosingClassHierarchy;
            var expected = new TypeHierarchy("N.B, TestProject")
            {
                Extends = new TypeHierarchy("N.A, TestProject")
                {
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy("N.IA, TestProject")
                        {
                            Implements = new HashSet<ITypeHierarchy>
                            {
                                new TypeHierarchy("N.I0, TestProject")
                            }
                        }
                    }
                },
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy("N.IB`1[[TB -> System.Int32, mscorlib, Version=4.0.0.0]], TestProject"),
                    new TypeHierarchy("N.IC, TestProject")
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldResolveEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("EnclosingMethods");

            var actual = ResultContext.EnclosingMethod;
            var expected = MethodName.Get("[System.Void, mscorlib, Version=4.0.0.0] [N.C6, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldResolveSuperEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("EnclosingMethods");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected = MethodName.Get("[System.Void, mscorlib, Version=4.0.0.0] [N.C4, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore]
        public void ShouldResolveFirstEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("EnclosingMethods");
            
            var actual = ResultContext.EnclosingMethodFirst;
            var expected = MethodName.Get("[System.Void, mscorlib, Version=4.0.0.0] [N.I, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }
    }
}