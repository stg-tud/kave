/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.TriggerPoint
{
    internal class CompletionTargetAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void CompletionOnOpeningBracket()
        {
            CompleteInClass(@" 
                public void M()
                {$
                   
                }");

            AssertCompletionMarker<IMethodDeclaration>(CompletionCase.InBody);

            AssertBody(
                "M",
                Fix.EmptyCompletion);
        }

        [Test]
        public void CompletionOnClosingBracket()
        {
            CompleteInClass(@" 
                public void M()
                {
                   
                $}");

            AssertCompletionMarker<IMethodDeclaration>(CompletionCase.InBody);

            AssertBody(
                "M",
                Fix.EmptyCompletion);
        }

        [Test]
        public void CompletionOnSemicolon()
        {
            CompleteInClass(@" 
                public void M()
                {
                    ;$
                }");

            AssertCompletionMarker<IEmptyStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                "M",
                Fix.EmptyCompletion);
        }

        [Test]
        public void CompletionOnSemicolonAfterStatement()
        {
            CompleteInClass(@" 
                public void M()
                {
                    return;$
                }");

            AssertCompletionMarker<IReturnStatement>(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                "M",
                new ReturnStatement {IsVoid = true},
                Fix.EmptyCompletion);
        }

        #region switch

        [Test]
        public void CompletionInSwitchBlock_Before()
        {
            CompleteInMethod(@"
                $
                switch (this)
                {
                    default:
                        break;
                }");

            AssertCompletionMarker<ISwitchStatement>(CompletionCase.EmptyCompletionBefore);
        }

        [Test]
        public void CompletionInSwitchBlock_AfterLabel()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        $
                }");

            AssertCompletionMarker<ISwitchCaseLabel>(CompletionCase.InBody);
        }

        [Test]
        public void CompletionInSwitchBlock_AfterLabelMulti()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    case 0:
                        $
                    case 1:
                        continue;
                }");

            AssertCompletionMarker<ISwitchCaseLabel>(CompletionCase.InBody);
        }

        [Test]
        public void CompletionInSwitchBlock_AfterLabelNonEmpty()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        $
                        continue;
                }");

            AssertCompletionMarker<IContinueStatement>(CompletionCase.EmptyCompletionBefore);
        }

        [Test]
        public void CompletionInSwitchBlock_Nested()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        continue;
                        $
                }");

            AssertCompletionMarker<IContinueStatement>(CompletionCase.EmptyCompletionAfter);
        }

        [Test]
        public void CompletionInSwitchBlock_Nested2()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        int i;
                        continue;
                        $
                }");

            AssertCompletionMarker<IContinueStatement>(CompletionCase.EmptyCompletionAfter);
        }

        [Test]
        public void CompletionInSwitchBlock_Nested3()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        continue;
                        $
                        int i;
                }");

            AssertCompletionMarker<IContinueStatement>(CompletionCase.EmptyCompletionAfter);
        }

        [Test]
        public void CompletionInSwitchBlock_After()
        {
            CompleteInMethod(@"
                switch (this)
                {
                    default:
                        continue;
                }
                $
            ");

            AssertCompletionMarker<ISwitchStatement>(CompletionCase.EmptyCompletionAfter);
        }

        #endregion
    }
}