using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SmokeTests
{
    [TestFixture]
    internal class CodeCompletionWithMissingTokensSmokeTest : KaVEBaseTest
    {
        [Test, Ignore]
        public void MissingClassBodyOpeningBrace()
        {
            CompleteInFile(@"
                class C $
                }");
        }

        [Test]
        public void MissingClassBodyClosingBrace()
        {
            CompleteInFile(@"
                class C
                {
                    $
            ");
        }

        [Test]
        public void DuplicatedClassBodyOpeningBrace()
        {
            CompleteInFile(@"
                class C
                {
                    {$
                }
            ");
        }

        [Test, Ignore]
        public void DuplicatedClassBodyClosingBrace()
        {
            CompleteInFile(@"
                class C
                {
                    }$
                }
            ");
        }

        [Test]
        public void MissingConditionOpeningBrace()
        {
            CompleteInMethod(@"
                if ($ {}
            ");
        }

        [Test]
        public void MissingConditionClosingBrace()
        {
            CompleteInMethod(@"
                if $) {}
            ");
        }

        [Test]
        public void MissingOperator()
        {
            CompleteInMethod(@"
                if (true fa$) {}
            ");
        }

        [Test, Ignore]
        public void MissingDeclarationName()
        {
            CompleteInMethod(@"
                var = $;
            ");
        }

        [Test, Ignore]
        public void MissingDeclarationType()
        {
            CompleteInMethod(@"
                v = $;
            ");
        }

        [Test]
        public void MissingAssignmentOperator()
        {
            CompleteInMethod(@"
                var v $;
            ");
        }

        [Test]
        public void MissingSemicolonInFor()
        {
            CompleteInMethod(@"
                for (;$) {}
            ");
        }

        [Test]
        public void MissingConditionInIf()
        {
            CompleteInMethod(@"
                if ($) {}
            ");
        }

        [Test]
        public void MissingConditionInWhile()
        {
            CompleteInMethod(@"
                while ($) {}
            ");
        }

        [Test]
        public void MissingSemicolon()
        {
            CompleteInMethod(@"
                object o = new object()
                $
            ");
        }
    }
}