using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SmokeTests
{
    [TestFixture]
    internal class CodeCompletionWithUnresolvableReferencesSmokeTest : KaVEBaseTest
    {
        [Test]
        public void MemberAccessOnUnkownType()
        {
            CompleteInMethod(@"
                o.$
            ");
        }

        [Test]
        public void MemberAccessOnUnknownMember()
        {
            CompleteInMethod(@"
                this.Foo.$
            ");
        }

        [Test]
        public void MemberAccessOnUnknownReturnType()
        {
            CompleteInClass(@"
                private Bar M() { return null; }
                
                public void N()
                {
                    M().$
                }
            ");
        }

        [Test]
        public void MemberAccessOnUnkownNamespace()
        {
            CompleteInMethod(@"
                System.Unkown.$
            ");
        }

        [Test]
        public void CallToInaccessibleMethod()
        {
            CompleteInFile(@"
                class C
                {
                    private void M(){}
                }
                
                class D
                {
                    public void M(C c)
                    {
                        c.M();
                        $
                    }
                }");
        }

        [Test]
        public void CallToUnresolvableMethod()
        {
            CompleteInMethod(@"
                Unknown();
                $
            ");
        }
    }
}