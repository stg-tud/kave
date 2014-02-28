using KaVE.JetBrains.Annotations;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisObjectTrackingTest : KaVEBaseTest
    {
        [Test]
        public void ShouldFindCallInPrivateHelper()
        {
            CompleteInClass(@"
                public void M1(object o) {
                    this.M2(o);
                    $
                }
        
                private void M2(object o) {
                    o.GetHashCode();
                }");

            AssertAnalysisFindsCallTo(
                "[System.Int32, mscorlib, Version=4.0.0.0] [System.Object, mscorlib, Version=4.0.0.0].GetHashCode()");
        }

        [Test]
        public void ShouldNotFindCallToPrivateHelper()
        {
            CompleteInFile(@"
                class C {
                    public void M1(object o) {
                        this.M2(o);
                        $
                    }
        
                    private void M2(object o) {
                        o.GetHashCode();
                    }
                }");

            AssertAnalysisDoesNotFindCallTo(
                "[System.Void, mscorlib, Version=4.0.0.0] [C, TestProject].M2([System.Object, mscorlib, Version=4.0.0.0] o)");
        }

        [Test]
        public void ShouldFindCallInPublicLocalHelper()
        {
            CompleteInClass(@"
                public void M1(object o)
                {
                    this.M2(o);
                    $
                }

                public void M2(object o) {
                    o.GetHashCode();
                }");

            AssertAnalysisFindsCallTo(
                "[System.Int32, mscorlib, Version=4.0.0.0] [System.Object, mscorlib, Version=4.0.0.0].GetHashCode()");
        }

        [StringFormatMethod("methodIdentifier")]
        private void AssertAnalysisDoesNotFindCallTo(string methodIdentifier, params object[] args)
        {
            CollectionAssert.DoesNotContain(
                ResultContext.CalledMethods,
                MethodName.Get(string.Format(methodIdentifier, args)));
        }

        [StringFormatMethod("methodIdentifier")]
        private void AssertAnalysisFindsCallTo(string methodIdentifier, params object[] args)
        {
            CollectionAssert.Contains(
                ResultContext.CalledMethods,
                MethodName.Get(string.Format(methodIdentifier, args)));
        }
    }
}