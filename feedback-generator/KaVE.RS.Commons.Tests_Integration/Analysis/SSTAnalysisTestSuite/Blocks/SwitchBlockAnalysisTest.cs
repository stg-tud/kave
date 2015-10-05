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

using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Blocks
{
    internal class SwitchBlockAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Empty()
        {
            CompleteInMethod(@"switch(a) { } $");

            AssertBody(
                new SwitchBlock {Reference = VarRef("a")},
                Fix.EmptyCompletion);
        }

        [Test]
        public void DefaultSectionOnly()
        {
            CompleteInMethod(@"
                switch (a)
                {
                    default:
                        break;
                }
                $");

            AssertBody(
                new SwitchBlock {Reference = VarRef("a"), DefaultSection = {new BreakStatement()}},
                Fix.EmptyCompletion);
        }

        [Test]
        public void Standard()
        {
            CompleteInMethod(@"
                switch (a)
                {
                    case 0:
                    case 1:
                        break;

                    case 2:
                        break;
                        break;

                    default:
                        break;
                }
                $");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("a"),
                    Sections =
                    {
                        new CaseBlock {Label = new ConstantValueExpression()},
                        new CaseBlock {Label = new ConstantValueExpression(), Body = {new BreakStatement()}},
                        new CaseBlock
                        {
                            Label = new ConstantValueExpression(),
                            Body =
                            {
                                new BreakStatement(),
                                new BreakStatement()
                            }
                        }
                    },
                    DefaultSection = {new BreakStatement()}
                },
                Fix.EmptyCompletion);
        }

        [Test]
        public void CompletionInCaseBlock_First()
        {
            CompleteInMethod(@"
                switch (a)
                {
                    default:
                        $
                        break;
                }");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("a"),
                    DefaultSection = { Fix.EmptyCompletion, new BreakStatement() }
                });
        }

        [Test]
        public void CompletionInCaseBlock_Second()
        {
            CompleteInMethod(@"
                switch (a)
                {
                    default:
                        break;
                        $
                }");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("a"),
                    DefaultSection = { new BreakStatement(), Fix.EmptyCompletion }
                });
        }

        [Test]
        public void CompletionOutsideCaseBlockIsIgnored_BeforeFirst()
        {
            CompleteInMethod(@"
                switch (a)
                {
                    $

                    default:
                        break;
                }");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("a"),
                    DefaultSection = { new BreakStatement() }
                });
        }

        [Test]
        public void CompletionOutsideCaseBlockIsIgnored_Empty()
        {
            CompleteInMethod(@"
                switch (a)
                {
                    $
                }");

            AssertBody(
                new SwitchBlock
                {
                    Reference = VarRef("a")
                });
        }
    }
}