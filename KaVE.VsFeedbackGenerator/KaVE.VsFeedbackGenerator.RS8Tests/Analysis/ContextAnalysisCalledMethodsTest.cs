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
            WhenCodeCompletionIsInvokedInFile("MethodWithCalls");

            var expected = MethodName.Get(
                "[System.Void, mscorlib, Version=4.0.0.0] [N.I1, TestProject].Do([System.Int32, mscorlib, Version=4.0.0.0] i)");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test]
        public void ShouldFindCallInCondition()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCalls");

            var expected = MethodName.Get(
                "[System.Boolean, mscorlib, Version=4.0.0.0] [N.I1, TestProject].Is()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test]
        public void ShouldFindCallInSubBlock()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCalls");

            var expected = MethodName.Get(
                "[N.C1, TestProject] [N.I1, TestProject].GetC()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test]
        public void ShouldFindNestedCall()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCalls");

            var expected = MethodName.Get(
                "[System.Int32, mscorlib, Version=4.0.0.0] [N.C1, TestProject].M()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (1)")]
        public void ShouldFindCallWithUndefinedTypeLevelTypeParameter()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[TI1 -> ?] [N.I1`1[[TI1 -> TM1 -> ?]], TestProject].Get()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (3)")]
        public void ShouldFindCallWithDefinedTypeLevelTypeParameter()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[TI1 -> ?] [N.I1`1[[TI1 -> System.String, mscorlib, Verion=4.0.0.0]], TestProject].Get()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (2)"), Ignore]
        public void ShouldFindCallWithMethodLevelTypeParameter()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[TI2 -> ?] [N.I1`1[[TI1 -> TM1 -> ?]], TestProject].Get`1[[TI2 -> System.Int32, mscorlib, Version=4.0.0.0]]()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (4)")]
        public void ShouldFindCallOnConstraintTypeParameterInstance()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[System.Int32, mscorlib, Version=4.0.0.0] [TM2 -> System.Object, mscorlib, Version=4.0.0.0].GetHashCode()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }
    }
}