using System.IO;
using System.Linq;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.FeaturesTestFramework.Completion;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.RS.Commons.Utils;

namespace KaVE.RS.Commons.Tests_Integration
{
    [TestNetFramework4]
    internal abstract class BaseCodeCompletionTest : CodeCompletionTestBase
    {
        private const string Caret = "$";

        protected override string RelativeTestDataPath
        {
            get { return ""; }
        }

        protected ProposalCollection ResultProposalCollection { get; private set; }

        protected void CompleteInFile(string content, string newExtension = "cs")
        {
            content = content.Replace(Caret, "{caret}");
            var testFile = Path.Combine(BaseTestDataPath.FullPath, "adhoc_test_snippet." + newExtension);

            try
            {
                File.WriteAllText(testFile, content);
                WhenCodeCompletionIsInvokedInFile("adhoc_test_snippet");
            }
            finally
            {
                File.Delete(testFile);
            }
        }

        protected void WhenCodeCompletionIsInvokedInFile(string fileName)
        {
            DoOneTest(fileName);
        }

        protected override void ExecuteCodeCompletion(Suffix suffix,
            ITextControl textControl,
            IntellisenseManager intellisenseManager,
            bool automatic,
            string documentText,
            IContextBoundSettingsStore settingsStore)
        {
            ResultProposalCollection = new ProposalCollection();

            var single = CodeCompletionParameters.CreateSingle(CodeCompletionType.BasicCompletion);
            single.EvaluationMode = EvaluationMode.LightAndFull;
            var result = intellisenseManager.GetCompletionResult(single, textControl);

            if (result != null)
            {
                ResultProposalCollection = result.LookupItems.ToProposalCollection();
            }
        }
    }
}