using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SmokeTests
{
    [TestFixture]
    internal class CodeCompletionInIncompleteDeclarationsSmokeTest : KaVEBaseTest
    {
        [Test]
        public void NamespaceDeclarationKeyword()
        {
            CompleteInFile(@"
                names$
            ");
        }

        [Test]
        public void NamespaceDeclarationName()
        {
            CompleteInFile(@"
                namespace $
            ");
        }

        [Test]
        public void NamespaceDeclarationNameContinuation()
        {
            CompleteInFile(@"
                namespace N$
            ");
        }

        [Test]
        public void NamespaceDeclarationSubNamespaceName()
        {
            CompleteInFile(@"
                namespace N.$
            ");
        }

        [Test]
        public void NamespaceDeclarationSubNamespaceNameContinuation()
        {
            CompleteInFile(@"
                namespace N.M$
            ");
        }

        [Test]
        public void TypeDeclarationKeyword()
        {
            CompleteInFile(@"
                cla$
            ");
        }

        [Test]
        public void TypeDeclarationName()
        {
            CompleteInFile(@"
                class $
            ");
        }

        [Test]
        public void ClassDeclarationNameContinuation()
        {
            CompleteInFile(@"
                class Foo$
            ");
        }

        [Test]
        public void MemberDeclarationOnlyName()
        {
            CompleteInClass(@"
                ToStr$
            ");
        }

        [Test]
        public void MemberDeclarationModifier()
        {
            CompleteInClass(@"
                pu$
            ");
        }

        [Test]
        public void MemberDeclarationValueType()
        {
            CompleteInClass(@"
                public lo$
            ");
        }

        [Test]
        public void MemberDeclarationName()
        {
            CompleteInClass(@"
                public void $
            ");
        }

        [Test]
        public void MemberDeclarationAfterName()
        {
            CompleteInClass(@"
                public void M$
            ");
        }

        [Test]
        public void MethodDeclarationParameterType()
        {
            CompleteInClass(@"
                public void M(in$)
            ");
        }

        [Test]
        public void MethodDeclarationParameterName()
        {
            CompleteInClass(@"
                public void M(int $)
            ");
        }

        [Test]
        public void MethodDeclarationParameterNew()
        {
            CompleteInClass(@"
                public void M(int i, $)
            ");
        }

        [Test]
        public void MethodDeclarationAfterSignature()
        {
            CompleteInClass(@"
                public void M()$
            ");
        }

        [Test]
        public void MethodDeclarationDuplication()
        {
            CompleteInClass(@"
                public void M() {}
                public void M()
                {
                    $
                }");
        }

        [Test]
        public void MethodDeclarationDuplicationOverloadCreation()
        {
            CompleteInClass(@"
                public void M() {}
                public void M($) {}");
        }
    }
}