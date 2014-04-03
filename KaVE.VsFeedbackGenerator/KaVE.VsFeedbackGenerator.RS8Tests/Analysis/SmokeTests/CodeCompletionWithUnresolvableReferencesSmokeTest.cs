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
        public void UnknownMember()
        {
            CompleteInMethod(@"
                this.Foo.$
            ");
        }

        [Test, Ignore]
        public void UnknownReturnType()
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
        public void UnkownNamespace()
        {
            CompleteInMethod(@"
                System.Unkown.$
            ");
        }

        [Test]
        public void InaccessibleMember()
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
    }
}