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

using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Impl.Blocks;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize
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
                    Condition = AnyBlockExpr,
                    Body = {AnyStatement}
                },
                new DoLoop
                {
                    Condition = AnyBlockExprAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void DoLoop_NullSafe()
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
        public void ForEachLoop_NullSafe()
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
                    Condition = AnyBlockExpr,
                    Step = {AnyStatement},
                    Body = {AnyStatement}
                },
                new ForLoop
                {
                    Init = {AnyStatementAnonymized},
                    Condition = AnyBlockExprAnonymized,
                    Step = {AnyStatementAnonymized},
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void ForLoop_NullSafe()
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
        public void IfElseBlock_NullSafe()
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
        public void LockBlock_NullSafe()
        {
            _sut.Visit(new LockBlock(), 0);
        }

        [Test]
        public void SwitchBlock()
        {
            var caseBlock =
                new CaseBlock
                {
                    Label = new ConstantValueExpression(), // TODO include label somehow!
                    Body = {AnyStatement}
                };

            var caseBlockAnonymized =
                new CaseBlock
                {
                    Label = new ConstantValueExpression(), // not anonymized
                    Body = {AnyStatementAnonymized}
                };

            AssertAnonymization(
                new SwitchBlock
                {
                    Reference = AnyVarReference,
                    Sections = {caseBlock},
                    DefaultSection = {AnyStatement}
                },
                new SwitchBlock
                {
                    Reference = AnyVarReferenceAnonymized,
                    Sections = {caseBlockAnonymized},
                    DefaultSection = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void SwitchBlock_NullSafe()
        {
            _sut.Visit(new SwitchBlock(), 0);
        }

        [Test]
        public void TryBlock()
        {
            var catchBlock = new CatchBlock
            {
                Exception = new VariableDeclaration
                {
                    Reference = AnyVarReference,
                    Type = Type("a")
                },
                Body = {AnyStatement}
            };
            var catchBlockAnonymized = new CatchBlock
            {
                Exception = new VariableDeclaration
                {
                    Reference = AnyVarReferenceAnonymized,
                    Type = TypeAnonymized("a")
                },
                Body = {AnyStatementAnonymized}
            };

            AssertAnonymization(
                new TryBlock
                {
                    Body = {AnyStatement},
                    CatchBlocks = {catchBlock},
                    Finally = {AnyStatement}
                },
                new TryBlock
                {
                    Body = {AnyStatementAnonymized},
                    CatchBlocks = {catchBlockAnonymized},
                    Finally = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void TryBlock_NullSafe()
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
        public void UncheckedBlock_NullSafe()
        {
            _sut.Visit(new UncheckedBlock(), 0);
        }

        [Test]
        public void UnsafeBlock()
        {
            AssertAnonymization(new UnsafeBlock(), new UnsafeBlock());
        }

        [Test]
        public void UnsafeBlock_NullSafe()
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
        public void UsingBlock_NullSafe()
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
                    Condition = AnyBlockExpr,
                    Body = {AnyStatement}
                },
                new WhileLoop
                {
                    Condition = AnyBlockExprAnonymized,
                    Body = {AnyStatementAnonymized}
                });
        }

        [Test]
        public void WhileLoop_NullSafe()
        {
            _sut.Visit(new WhileLoop(), 0);
        }
    }
}