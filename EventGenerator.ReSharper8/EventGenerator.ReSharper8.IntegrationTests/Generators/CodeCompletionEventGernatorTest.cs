using JetBrains.ReSharper.Feature.Services.Tests.CSharp.FeatureServicesCSharp.CodeCompletion;
using NUnit.Framework;

namespace KaVE.EventGenerator.ReSharper8.IntegrationTests.Generators
{
    [TestFixture]
    public class CodeCompletionEventGernatorTest : CSharpCodeCompletionListTestBase
    {
        [TestCase("TestTest")]
        public void ShouldTestTestTest(string fileName)
        {
            DoOneTest(fileName);
        }
    }
}
