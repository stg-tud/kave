using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SmokeTests
{
    [TestFixture]
    internal class CodeCompletionWithIncompleteReferenceSmokeTest : KaVEBaseTest
    {
        [Test]
        public void UsingKeyword()
        {
            CompleteInFile(@"
                usin$
            ");
        }

        [Test]
        public void UsingNamespace()
        {
            CompleteInFile(@"
                using $
            ");
        }

        [Test]
        public void UsingNamespaceContinuation()
        {
            CompleteInFile(@"
                using Sys$
            ");
        }

        [Test]
        public void UsingSubNamespace()
        {
            CompleteInFile(@"
                using Sytem.$
            ");
        }

        [Test]
        public void UsingSubNamespaceContinuation()
        {
            CompleteInFile(@"
                using Sytem.Coll$
            ");
        }

        [Test]
        public void UsingWithAlias()
        {
            CompleteInFile(@"
                using MyAlias = $
            ");
        }

        [Test]
        public void ClassAnntation()
        {
            CompleteInFile(@"
                [$]
                public class C {}
            ");
        }

        [Test]
        public void ClassAnntationContinuation()
        {
            CompleteInFile(@"
                [Not$]
                public class C {}
            ");
        }

        [Test]
        public void ConstructorDelegationKeyword()
        {
            CompleteInFile(@"
                class C
                {
                    public C(int i) : th$
                    
                    public C(string s) {}
                }");
        }

        [Test]
        public void ConstructorDelegationDisambiguation()
        {
            CompleteInFile(@"
                class C
                {
                    public C(int i) : this$
                    
                    public C(string s) {}
                }");
        }

        [Test]
        public void ConstructorDelegationParameter()
        {
            CompleteInFile(@"
                class C
                {
                    public C(int i) : this($)
                    
                    public C(string s) {}
                }");
        }

        [Test]
        public void MemberAnnotation()
        {
            CompleteInClass(@"
                [$]
                public int _f;");
        }

        [Test]
        public void MemberAnnotationContinuation()
        {
            CompleteInClass(@"
                [Ha$]
                public int _f;");
        }

        [Test]
        public void MemberAccessDirect()
        {
            CompleteInClass(@"
                public void M(object o)
                {
                    o.$
                }
            ");
        }

        [Test]
        public void MemberAccessDirectContinuation()
        {
            CompleteInClass(@"
                public void M(object o)
                {
                    o.ToS$
                }
            ");
        }

        [Test]
        public void MemberAccessUnknownType()
        {
            CompleteInClass(@"
                public void M()
                {
                    o.$
                }
            ");
        }

        [Test]
        public void MemberAccessUnknownTypeContinuation()
        {
            CompleteInClass(@"
                public void M()
                {
                    o.Get$
                }
            ");
        }

        [Test]
        public void MemberAccessInCast()
        {
            CompleteInClass(@"
                public void M(object o)
                {
                    var u = (int) o.$
                }
            ");
        }

        [Test]
        public void MemberAccessInCastContinuation()
        {
            CompleteInClass(@"
                public void M(object o)
                {
                    var u = (int) o.GetH$
                }
            ");
        }

        [Test]
        public void CastSource()
        {
            CompleteInClass(@"
                public void M()
                {
                    var u = (int) $
                }
            ");
        }

        [Test]
        public void MemberAccessOnReturnValue()
        {
            CompleteInClass(@"
                public void M()
                {
                    GetHashCode().$
                }
            ");
        }

        [Test]
        public void MemberAccessOnReturnValueContinuation()
        {
            CompleteInClass(@"
                public void M()
                {
                    GetHashCode().GetH$
                }
            ");
        }

        [Test]
        public void MemberAccessOnField()
        {
            CompleteInClass(@"
                private object _f;

                public void M()
                {
                    _f.$
                }
            ");
        }

        [Test]
        public void MemberAccessOnFieldContinuation()
        {
            CompleteInClass(@"
                private object _f;

                public void M()
                {
                    _f.Eq$
                }
            ");
        }

        [Test]
        public void MissingReturn()
        {
            CompleteInClass(@"
                public int M()
                {
                    $
                }
            ");
        }

        [Test]
        public void MissingArgument()
        {
            CompleteInMethod(@"
                this.Equals($);
            ");
        }

        [Test]
        public void MissingArgumentContinuation()
        {
            CompleteInMethod(@"
                this.Equals(th$);
            ");
        }
    }
}