using JetBrains.Util;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisCalledMethodsTest : KaVEBaseTest
    {
        [Test]
        public void ShouldFindNoCalls()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithoutCalls");

            Assert.IsTrue(ResultContext.CalledMethods.IsEmpty());
        }

        [Test]
        public void ShouldFindCallOnTopLevel()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsOnSingleObject");

            var expected = MethodName.Get(
                "[System.Void, mscorlib, Version=4.0.0.0] [N.I1, TestProject].Do([System.Int32, mscorlib, Version=4.0.0.0] i)");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test]
        public void ShouldFindCallInCondition()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsOnSingleObject");

            var expected = MethodName.Get(
                "[System.Boolean, mscorlib, Version=4.0.0.0] [N.I1, TestProject].Is()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test]
        public void ShouldFindCallInSubBlock()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsOnSingleObject");

            var expected = MethodName.Get(
                "[N.C1, TestProject] [N.I1, TestProject].GetC()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test]
        public void ShouldFindNestedCall()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsOnSingleObject");

            var expected = MethodName.Get(
                "[System.Int32, mscorlib, Version=4.0.0.0] [N.C1, TestProject].M()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }
    }
}