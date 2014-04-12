using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    // TODO check if any of these tests are still relevant and either move or remove them
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

            var actual = ResultContext.EnclosingMethodHierarchy.Element;
            var expected =
                MethodName.Get("[System.Void, mscorlib, 4.0.0.0] [TestNamespace.TestClass, TestProject].Doit()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotContainObjectInTypeHierarchy()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");
            Assert.IsNull(ResultContext.TypeShape.TypeHierarchy.Extends);
        }

        [Test]
        public void ShouldRetrieveEnclosingType()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.TypeShape.TypeHierarchy.Element;

            var expected = TypeName.Get("TestNamespace.TestClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSuperType()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.TypeShape.TypeHierarchy.Extends.Element;

            var expected = TypeName.Get("TestNamespace.SuperClass, TestProject");

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveImplementedInterfaces()
        {
            WhenCodeCompletionIsInvokedInFile("TypeHierarchy");
            var actual = ResultContext.TypeShape.TypeHierarchy.Implements;

            var expected = new HashSet<ITypeHierarchy>
            {
                new TypeHierarchy("i:TestNamespace.AnInterface, TestProject")
            };

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveSubstitution()
        {
            WhenCodeCompletionIsInvokedInFile("GenericTypeHierarchy");

            var actual = ResultContext.TypeShape.TypeHierarchy.Extends.Element;
            var expected = TypeName.Get("N.IC`1[[T -> System.Int32, mscorlib, 4.0.0.0]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveFreeSubstitution()
        {
            WhenCodeCompletionIsInvokedInFile("GenericTypeHierarchy");

            var actual = ResultContext.TypeShape.TypeHierarchy.Element;
            var expected = TypeName.Get("N.C`1[[T]], TestProject");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveLargeHierarchy()
        {
            WhenCodeCompletionIsInvokedInFile("LargeHierarchy");
            var actual = ResultContext.TypeShape.TypeHierarchy;
            var expected = new TypeHierarchy("N.B, TestProject")
            {
                Extends = new TypeHierarchy("N.A, TestProject")
                {
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy("i:N.IA, TestProject")
                        {
                            Implements = new HashSet<ITypeHierarchy>
                            {
                                new TypeHierarchy("i:N.I0, TestProject")
                            }
                        }
                    }
                },
                Implements = new HashSet<ITypeHierarchy>
                {
                    new TypeHierarchy("i:N.IB`1[[TB -> System.Int32, mscorlib, 4.0.0.0]], TestProject"),
                    new TypeHierarchy("i:N.IC, TestProject")
                }
            };
            Assert.AreEqual(expected, actual);
        }
    }
}