using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.Tests.CSharp.FeatureServices.CodeCompletion;
using JetBrains.ReSharper.TestFramework;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Generators
{
    [Category("Code Completion"), TestNetFramework4, TestFixture]
    public class CodeCompletionEventGernatorTest : CodeCompletionTestBase
    {
        private ProposalCollection _lastProposalList;

        protected override bool ExecuteAction
        {
            get { return false; }
        }

        protected override string RelativeTestDataPath
        {
            get { return "CodeCompletion\\ProposalNames"; }
        }

        /// <summary>
        /// Intercepts the LookupItem list and captures as a proposal collection for later validation.
        /// </summary>
        protected override IEnumerable<ILookupItem> GetItemsFromResult(ICodeCompletionResult result,
            JetHashSet<ILookupItem> best)
        {
            var itemsFromResult = base.GetItemsFromResult(result, best).ToList();
            _lastProposalList = itemsFromResult.ToProposalCollection();
            return itemsFromResult;
        }

        [TestCase("ProofOfConcept"),
         TestCase("VariableProposals"),
         TestCase("MethodProposals"),
         TestCase("MemberKindProposals"),
         TestCase("GenericTypeProposals"),
         TestCase("GenericTypeInstanceProposals"),
         TestCase("StaticMemberProposals"),
         TestCase("NamespaceProposals"),
         TestCase("NewInstanceProposals"),
         TestCase("NewArrayInstanceProposals"),
         TestCase("ArrayTypeProposals"),
         TestCase("UncompilableFileProposals"),
         TestCase("MethodOverloadProposals"),
         TestCase("ClassLevelProposals")]
        // Test cases for keywords (e.g., private), templates (e.g., for), and
        // combined proposals (e.g., return true) donnot seem possible, as such
        // proposals are not made by the test environment's completion engine.
        public void TestCompletionListTranslation(string fileName)
        {
            DoOneTest(fileName);
            DoCheckProposalNamesAgainstGoldFile(fileName);
        }

        private void DoCheckProposalNamesAgainstGoldFile(string fileName)
        {
            var actualProposalIdentifier = _lastProposalList.Proposals.Select(p => p.Name.Identifier);
            using (var goldFile = new StreamReader(GetProposalNamesGoldFile(fileName)))
            {
                using (var iter = actualProposalIdentifier.GetEnumerator())
                {
                    string name;
                    var lineNumber = 1;
                    while ((name = goldFile.ReadLine()) != null && iter.MoveNext())
                    {
                        Assert.AreEqual(name, iter.Current, "difference in line {0}", lineNumber);
                        lineNumber++;
                    }
                    Assert.IsNull(name, "missing item from gold file: {0}", name);
                    Assert.IsFalse(iter.MoveNext(), "unexpected item: {0}", iter.Current);
                }
            }
        }

        private string GetProposalNamesGoldFile(string fileName)
        {
            return ExpandRelativePaths(new[] {fileName + ".cs.gold.names"}).First();
        }
    }
}