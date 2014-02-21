using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    class ContextAnalysisTest : KaVEBaseTest
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
            var expected = MethodName.Get("[System.Void, mscorlib, Version=4.0.0.0] [TestNamespace.TestClass, TestProject].Doit()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetrieveEnclosingType()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");
            var actual = ResultContext.EnclosingClassHierarchy;

            var expected = new TypeHierarchy {Element = TypeName.Get("TestNamespace.TestClass, TestProject")};

            Assert.AreEqual(expected, actual);
        }
    }
}
