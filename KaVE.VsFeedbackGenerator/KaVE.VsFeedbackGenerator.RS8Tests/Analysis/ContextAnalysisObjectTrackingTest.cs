using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis
{
    [TestFixture]
    internal class ContextAnalysisObjectTrackingTest : KaVEBaseTest
    {
        [UsedImplicitly]
        private static readonly string[] Visibilities = {"public", "protected", "internal", "private"};

        [TestCaseSource("Visibilities")]
        public void ShouldFindCallInLocalMethod(string helperVisibility)
        {
            CompleteInClass(@"
                public void M1(object o) {
                    this.M2(o);
                    $
                }
        
                " + helperVisibility + @" void M2(object o) {
                    o.GetHashCode();
                }");

            AssertAnalysisFindsCallsTo(
                "[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");
        }

        [TestCaseSource("Visibilities")]
        public void ShouldNotFindCallToLocalMethod(string helperVisibility)
        {
            CompleteInFile(@"
                class C {
                    public void M1(object o) {
                        this.M2(o);
                        $
                    }
        
                    " + helperVisibility + @" void M2(object o) {}
                }");

            AssertAnalysisDoesNotFindCallTo(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M2([System.Object, mscorlib, 4.0.0.0] o)");
        }

        [Test]
        public void ShouldFindCallToMethodDeclaredBySupertype()
        {
            CompleteInFile(@"
                public interface I
                {
                    void M(object o);
                }

                public class C : I
                {
                    public override void M(object o) {}

                    public void E(object o)
                    {
                        M(o);
                        $
                    }
                }");

            AssertAnalysisFindsCallsTo("[System.Void, mscorlib, 4.0.0.0] [I, TestProject].M([System.Object, mscorlib, 4.0.0.0] o)");
        }

        [Test]
        public void ShouldNotFindCallInMethodDeclaredBySupertype()
        {
            CompleteInFile(@"
                public interface I
                {
                    void M(object o);
                }

                public class C : I
                {
                    public override void M(object o)
                    {
                        o.GetHashCode();
                    }

                    public void E(object o)
                    {
                        M(o);
                        $
                    }
                }");

            AssertAnalysisDoesNotFindCallTo(
                "[System.Int32, mscorlib, 4.0.0.0] [System.Object, mscorlib, 4.0.0.0].GetHashCode()");
        }

        [Test]
        public void ShouldIgnoreCallToAbstractLocalMethod()
        {
            CompleteInFile(@"
                abstract class A
                {
                    public abstract void M(object o);

                    public void E(object o)
                    {
                        M(o);
                        $
                    }
                }");

            AssertAnalysisDoesNotFindCallTo("[System.Void, mscorlib, 4.0.0.0] [A, TestProject].M([System.Object, mscorlib, 4.0.0.0] o)");
        }

        [Test]
        public void ShouldFindCallToMethodFromOtherClass()
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

            AssertAnalysisFindsCallsTo("[System.Void, mscorlib, 4.0.0.0] [HelperClass, TestProject].M()");
        }

        [Test]
        public void ShouldFindCallToMethodFromOtherClassByItsFirstDeclaration()
        {
            CompleteInFile(@"
                public interface I
                {
                    void M();
                }

                public class HelperClass : I
                {
                    public override void M() {}
                }
                
                public class C
                {
                    public void E(HelperClass h)
                    {
                        h.M();
                        $
                    }
                }");

            AssertAnalysisFindsCallsTo("[System.Void, mscorlib, 4.0.0.0] [I, TestProject].M()");
        }

        private void AssertAnalysisDoesNotFindCallTo(string methodIdentifier)
        {
            CollectionAssert.DoesNotContain(
                ResultContext.CalledMethods,
                MethodName.Get(methodIdentifier));
        }

        private void AssertAnalysisFindsCallsTo(params string[] methodIdentifiers)
        {
            var actual = ResultContext.CalledMethods;
            var expected = methodIdentifiers.Select(MethodName.Get);
            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}