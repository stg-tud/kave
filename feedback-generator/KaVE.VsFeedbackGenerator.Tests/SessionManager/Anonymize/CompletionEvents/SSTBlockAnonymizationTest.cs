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
 * 
 * Contributors:
 *    - Sebastian Proksch
 */

using JetBrains;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize.CompletionEvents;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize.CompletionEvents
{
    public class SSTBlockAnonymizationTest : SSTAnonymizationBaseTest
    {
        private SSTStatementAnonymization _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SSTStatementAnonymization(ExpressionAnonymizationMock, ReferenceAnonymizationMock);
        }

        private void AssertAnonymization(IStatement statement, IStatement expected)
        {
            var actual = statement.Accept(_sut, 0);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DoLoop()
        {
            AssertAnonymization(
                new DoLoop
                {
                    Condition = AnyExpression,
                    Body = {AnyStatement}
                },
                new DoLoop
                {
                    Condition = AnyExpressionAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void DoLoop_BlockCondition()
        {
            AssertAnonymization(
                new DoLoop
                {
                    Condition = AnyLoopHeaderBlock,
                    Body = {AnyStatement}
                },
                new DoLoop
                {
                    Condition = AnyLoopHeaderBlockAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void DoLoop_DefaultSafe()
        {
            _sut.Visit(new DoLoop(), 0);
        }

        [Test]
        public void ForEachLoop()
        {
            AssertAnonymization(
                new ForEachLoop
                {
                    Declaration = AnyVarDeclaration,
                    LoopedReference = AnyVarReference,
                    Body = {AnyStatement}
                },
                new ForEachLoop
                {
                    Declaration = AnyVarDeclarationAnonymized,
                    LoopedReference = AnyVarReferenceAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void ForEachLoop_DefaultSafe()
        {
            _sut.Visit(new ForEachLoop(), 0);
        }

        [Test]
        public void ForLoop()
        {
            AssertAnonymization(
                new ForLoop
                {
                    Init = {AnyStatement},
                    Condition = AnyExpression,
                    Step = {AnyStatement},
                    Body = {AnyStatement}
                },
                new ForLoop
                {
                    Init = {AnyStatementAnonymized},
                    Condition = AnyExpressionAnonymized,
                    Step = {AnyStatementAnonymized},
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void ForLoop_BlockCondition()
        {
            AssertAnonymization(
                new ForLoop
                {
                    Init = {AnyStatement},
                    Condition = AnyLoopHeaderBlock,
                    Step = {AnyStatement},
                    Body = {AnyStatement}
                },
                new ForLoop
                {
                    Init = {AnyStatementAnonymized},
                    Condition = AnyLoopHeaderBlockAnonymized,
                    Step = {AnyStatementAnonymized},
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void ForLoop_DefaultSafe()
        {
            _sut.Visit(new ForLoop(), 0);
        }

        [Test]
        public void IfElseBlock()
        {
            AssertAnonymization(
                new IfElseBlock
                {
                    Condition = AnyExpression,
                    Then = {AnyStatement},
                    Else = {AnyStatement}
                },
                new IfElseBlock
                {
                    Condition = AnyExpressionAnonymized,
                    Then = {AnyStatementAnonymized},
                    Else = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void IfElseBlock_DefaultSafe()
        {
            _sut.Visit(new IfElseBlock(), 0);
        }

        [Test]
        public void LockBlock()
        {
            AssertAnonymization(
                new LockBlock
                {
                    Reference = AnyVarReference,
                    Body = {AnyStatement}
                },
                new LockBlock
                {
                    Reference = AnyVarReferenceAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void LockBlock_DefaultSafe()
        {
            _sut.Visit(new LockBlock(), 0);
        }

        [Test]
        public void SwitchBlock()
        {
            AssertAnonymization(
                new SwitchBlock
                {
                    Reference = AnyVarReference,
                    Sections =
                    {
                        new CaseBlock
                        {
                            Label = new ConstantValueExpression {Value = "a"},
                            Body = {AnyStatement}
                        }
                    },
                    DefaultSection = {AnyStatement}
                },
                new SwitchBlock
                {
                    Reference = AnyVarReferenceAnonymized,
                    Sections =
                    {
                        new CaseBlock
                        {
                            Label = new ConstantValueExpression {Value = "a"}, // not anonymized
                            Body = {AnyStatementAnonymized}
                        }
                    },
                    DefaultSection = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void SwitchBlock_DefaultSafe()
        {
            _sut.Visit(new SwitchBlock(), 0);
        }

        [Test]
        public void TryBlock()
        {
            var someParameter = ParameterName.Get("[{0}] p".FormatEx(Type("a")));
            AssertAnonymization(
                new TryBlock
                {
                    Body = {AnyStatement},
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            Parameter = someParameter,
                            Body = {AnyStatement}
                        }
                    },
                    Finally = {AnyStatement}
                },
                new TryBlock
                {
                    Body = {AnyStatementAnonymized},
                    CatchBlocks =
                    {
                        new CatchBlock
                        {
                            // ReSharper disable once AssignNullToNotNullAttribute
                            Parameter = someParameter.ToAnonymousName(),
                            Body = {AnyStatementAnonymized}
                        }
                    },
                    Finally = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void TryBlock_DefaultSafe()
        {
            _sut.Visit(new TryBlock(), 0);
        }

        [Test]
        public void UncheckedBlock()
        {
            AssertAnonymization(
                new UncheckedBlock
                {
                    Body = {AnyStatement}
                },
                new UncheckedBlock
                {
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void UncheckedBlock_DefaultSafe()
        {
            _sut.Visit(new UncheckedBlock(), 0);
        }

        [Test]
        public void UnsafeBlock()
        {
            AssertAnonymization(new UnsafeBlock(), new UnsafeBlock());
        }

        [Test]
        public void UnsafeBlock_DefaultSafe()
        {
            _sut.Visit(new UnsafeBlock(), 0);
        }

        [Test]
        public void UsingBlock()
        {
            AssertAnonymization(
                new UsingBlock
                {
                    Reference = AnyVarReference,
                    Body = {AnyStatement}
                },
                new UsingBlock
                {
                    Reference = AnyVarReferenceAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void UsingBlock_DefaultSafe()
        {
            _sut.Visit(new UsingBlock(), 0);
        }

        [Test]
        public void WhileLoop()
        {
            AssertAnonymization(
                new WhileLoop
                {
                    Condition = AnyExpression,
                    Body = {AnyStatement}
                },
                new WhileLoop
                {
                    Condition = AnyExpressionAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void WhileLoop_BlockCondition()
        {
            AssertAnonymization(
                new WhileLoop
                {
                    Condition = AnyLoopHeaderBlock,
                    Body = {AnyStatement}
                },
                new WhileLoop
                {
                    Condition = AnyLoopHeaderBlockAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void WhileLoop_DefaultSafe()
        {
            _sut.Visit(new WhileLoop(), 0);
        }
    }
}