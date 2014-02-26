using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisEnclosingMethodTest : KaVEBaseTest
    {
        [Test]
        public void ShouldResolveEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = ResultContext.EnclosingMethod;
            var expected =
                MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C6, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldResolveSuperEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = ResultContext.EnclosingMethodSuper;
            var expected =
                MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.C4, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore]
        public void ShouldResolveFirstEnclosingMethod()
        {
            WhenCodeCompletionIsInvokedInFile("SimpleHierarchy");

            var actual = ResultContext.EnclosingMethodFirst;
            var expected =
                MethodName.Get(
                    "[System.Void, mscorlib, Version=4.0.0.0] [N.I, TestProject].M([System.Int32, mscorlib, Version=4.0.0.0] i)");
            Assert.AreEqual(expected, actual);
        }
    }
}