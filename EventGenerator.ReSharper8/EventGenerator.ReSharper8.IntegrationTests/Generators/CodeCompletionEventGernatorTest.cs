using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Feature.Services.Tests.CSharp.FeatureServices.CodeCompletion;
using JetBrains.ReSharper.TestFramework;
using KaVE.EventGenerator.ReSharper8.Utils;
using KaVE.Model.Events.CompletionEvent;
using NUnit.Framework;

namespace KaVE.EventGenerator.ReSharper8.IntegrationTests.Generators
{
    [TestReferences(new string[] { "System.dll", "System.Core.dll" })]
    [Category("Code Completion")]
    [TestNetFramework4]
    [TestFixture]
    public class CodeCompletionEventGernatorTest : CodeCompletionTestBase
    {
        private ProposalCollection _lastProposalList;

        protected override bool ExecuteAction
        {
            get { return false; }
        }

        protected override string RelativeTestDataPath
        {
            get
            {
                return "CodeCompletion\\ProposalNames";
            }
        }

        /// <summary>
        /// Intercepts the LookupItem list and captures as a proposal collection for later validation.
        /// </summary>
        protected override IEnumerable<ILookupItem> GetItemsFromResult(ICodeCompletionResult result, JetHashSet<ILookupItem> best)
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
         TestCase("UncompilableFileProposals")]
        public void TestCompletionListTranslation(string fileName)
        {
            DoOneTest(fileName);
            DoCheckProposalNames(fileName);
        }

        private void DoCheckProposalNames(string fileName)
        {
            var actualProposalIdentifier = _lastProposalList.Proposals.Select(p => p.Name.Identifier);
            using (var goldFile = new StreamReader(GetExpectedProposalIdentifierFile(fileName)))
            {
                using (var iter = actualProposalIdentifier.GetEnumerator())
                {
                    string name;
                    while ((name = goldFile.ReadLine()) != null && iter.MoveNext())
                    {
                        Assert.AreEqual(name, iter.Current);
                    }
                    Assert.IsNull(name, "missing item from gold file: {0}", name);
                    Assert.IsFalse(iter.MoveNext(), "unexpected item: {0}", iter.Current);
                }
            }
        }

        private string GetExpectedProposalIdentifierFile(string fileName)
        {
            return ExpandRelativePaths(new[] {fileName + ".cs.gold.names"}).First();
        }
    }
}
