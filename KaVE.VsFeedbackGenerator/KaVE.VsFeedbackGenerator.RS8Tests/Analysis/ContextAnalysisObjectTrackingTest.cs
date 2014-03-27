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
        public void ShouldFindCallInInternalHelper()
        {
            CompleteInClass(@"
                public void M1(object o) {
                    this.M2(o);
                    $
                }
        
                internal void M2(object o) {
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

        [Test]
        public void ShouldFindCallToHelperFromOtherClass()
        {
            CompleteInFile(@"
                public class HelperClass
                {
                    public void M() {}
                }
                
                public class C
                {
                    public void E(HelperClass h)
                    {
                        h.M();
                        $
                    }
                }");

            AssertAnalysisFindsCallTo("[System.Void, mscorlib, Version=4.0.0.0] [HelperClass, TestProject].M()");
        }

        [Test]
        public void ShouldFindCallToMethodDeclaredBySupertype()
        {
            CompleteInFile(@"
                public interface I
                {
                    void M();
                }

                public class C : I
                {
                    public override void M() {}

                    public void E()
                    {
                        M();
                        $
                    }
                }");

            AssertAnalysisFindsCallTo("[System.Void, mscorlib, Version=4.0.0.0] [C, TestProject].M()");
        }

        [Test]
        public void ShouldNotFindCallInMethodDeclaredBySupertype()
        {
            CompleteInFile(@"
                public interface I
                {
                    void M();
                }

                public class C : I
                {
                    public override void M()
                    {
                        this.GetHashCode();
                    }

                    public void E()
                    {
                        M();
                        $
                    }
                }");

            AssertAnalysisDoesNotFindCallTo(
                "[System.Int32, mscorlib, Version=4.0.0.0] [System.Object, mscorlib, Version=4.0.0.0].GetHashCode()");
        }

        private void AssertAnalysisDoesNotFindCallTo(string methodIdentifier)
        {
            CollectionAssert.DoesNotContain(
                ResultContext.CalledMethods,
                MethodName.Get(methodIdentifier));
        }

        private void AssertAnalysisFindsCallTo(string methodIdentifier)
        {
            CollectionAssert.Contains(
                ResultContext.CalledMethods,
                MethodName.Get(methodIdentifier));
        }
    }
}