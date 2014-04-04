using JetBrains.Util;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisCalledMethodsTest : KaVEBaseTest
    {
        // TODO inline test files in tests
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
                "[System.Void, mscorlib, 4.0.0.0] [N.I1, TestProject].Do([System.Int32, mscorlib, 4.0.0.0] i)");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test]
        public void ShouldFindCallInCondition()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCalls");

            var expected = MethodName.Get(
                "[System.Boolean, mscorlib, 4.0.0.0] [N.I1, TestProject].Is()");

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
                "[System.Int32, mscorlib, 4.0.0.0] [N.C1, TestProject].M()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (1)")]
        public void ShouldFindCallWithUndefinedTypeLevelTypeParameter()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[TI1] [N.I`1[[TI1 -> TM1]], TestProject].Get()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (3)")]
        public void ShouldFindCallWithDefinedTypeLevelTypeParameter()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[TI1] [N.I`1[[TI1 -> System.String, mscorlib, 4.0.0.0]], TestProject].Get()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (2)")]
        public void ShouldFindCallWithMethodLevelTypeParameter()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[TI2] [N.I`1[[TI1 -> TM1]], TestProject].Get[[TI2 -> System.Int32, mscorlib, 4.0.0.0]]()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (4)")]
        public void ShouldFindCallOnFreeTypeParameterInstance()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test(Description = "Marker: (5)")]
        public void ShouldFindCallOnTransitivlyConstraintTypeParameterInstance()
        {
            WhenCodeCompletionIsInvokedInFile("MethodWithCallsAndGenerics");

            var expected = MethodName.Get("[B] [N.D`1[[B -> TM2]], TestProject].Get()");

            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }

        [Test]
        public void ShouldResolveAmbiguousCall()
        {
            CompleteInMethod(@"
                this.Equals();
                $
            ");

            AssertCalledMethodsContain("[System.Boolean, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].Equals([System.Object, mscorlib, 4.0.0.0] obj)");
        }

        [Test]
        public void ShouldResolveUnresolvableCall()
        {
            CompleteInFile(@"
                class C
                {
                    public void M()
                    {
                        this.Unknown();
                        $
                    }
                }");

            CollectionAssert.IsEmpty(ResultContext.CalledMethods);
        }

        private void AssertCalledMethodsContain(string methodIdentifier)
        {
            var expected = MethodName.Get(methodIdentifier);
            CollectionAssert.Contains(ResultContext.CalledMethods, expected);
        }
    }
}