using KaVE.Commons.Model.Naming;
using KaVE.Commons.TestUtils.Model.Naming;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.TypeShapeAnalysisTestSuite
{
    internal class DelegateTest : BaseCSharpCodeCompletionTest
    {
        [Test]
        public void NoDelegates()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M()
                    {
                        $
                    }
                }
            ");

            Assert.IsEmpty(ResultContext.TypeShape.Delegates);
        }

        [Test]
        public void ShouldRetrieveDelegates()
        {
            CompleteInClass(@"
                public delegate void D1(object o);
                public delegate void D2(int o);
                
                $
            ");

            var expected = Sets.NewHashSet(
                Names.Type(
                         "d:[{0}] [N.C+D1, TestProject].([{1}] o)",
                         NameFixture.Void,
                         NameFixture.Object)
                     .AsDelegateTypeName,
                Names.Type(
                         "d:[{0}] [N.C+D2, TestProject].([{1}] o)",
                         NameFixture.Void,
                         NameFixture.Int)
                     .AsDelegateTypeName);

            Assert.AreEqual(expected, ResultContext.TypeShape.Delegates);
        }
    }
}