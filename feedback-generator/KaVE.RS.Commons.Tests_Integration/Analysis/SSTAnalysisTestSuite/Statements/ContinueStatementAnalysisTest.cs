using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;


namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Statements
{
    internal class ContinueStatementAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void CompletionBefore()
        {
            CompleteInMethod(@"
                $
                continue;
            ");

            AssertBody(
                Fix.EmptyCompletion,
                new ContinueStatement());
        }

        [Test]
        public void CompletionAfter()
        {
            CompleteInMethod(@"
                continue;
                $
            ");

            AssertBody(
                new ContinueStatement(),
                Fix.EmptyCompletion);
        }
    }
}