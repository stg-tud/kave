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

using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.VsFeedbackGenerator.Analysis.CompletionTarget;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    // only covers basic cases of declarations, assignments, constant int values, and different completion points
    internal class BasicTest : BaseSSTAnalysisTest
    {
        [Test]
        public void LonelyVariableDeclaration()
        {
            CompleteInMethod(@"
                int i;
                $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
        }

        [Test]
        public void AssignmentOfConstantValue()
        {
            CompleteInMethod(@"
                int i = 1;
                $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = new ConstantValueExpression()
                },
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
        }

        [Test]
        public void PlainCompletion()
        {
            CompleteInClass(@"
                public void A() {
                    $
                }
            ");

            AssertNodeIsMethodDeclaration("A", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.InBody);

            AssertBody(
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
        }

        [Test]
        public void CompletionBeforeDeclaration()
        {
            CompleteInMethod(@"
                $
                int i;
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                },
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                });
        }

        [Test]
        public void CompletionAfterDeclaration()
        {
            CompleteInMethod(@"
                int i;
                $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
        }

        [Test]
        public void CompletionInBetweenDeclarations()
        {
            CompleteInMethod(@"
                int i;
                $
                int j;
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                },
                new VariableDeclaration
                {
                    Reference = VarRef("j"),
                    Type = Fix.Int
                });
        }

        [Test]
        public void CompletionBeforeExpressionStatement()
        {
            CompleteInMethod(@"
                $
                int i = 1;
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionBefore);

            AssertBody(
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                },
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = new ConstantValueExpression()
                });
        }

        [Test]
        public void CompletionAfterExpressionStatement()
        {
            CompleteInMethod(@"
                int i = 1;
                $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = new ConstantValueExpression()
                },
                new ExpressionStatement
                {
                    Expression = new CompletionExpression()
                });
        }

        [Test]
        public void CompletionInDeclarationInitializer()
        {
            CompleteInMethod(@"
                int i = $
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.Undefined);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = new CompletionExpression()
                });
        }

        [Test]
        public void CompletionInDeclarationInitializerWithoutSpace()
        {
            TestAnalysisTrigger.IsPrintingType = true;

            CompleteInMethod(@"
                int i=$
            ");

            AssertNodeIsVariableDeclaration("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.Undefined);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = new CompletionExpression()
                });
        }

        [Test]
        public void CompletionInDeclarationInitializerWithReference()
        {
            CompleteInMethod(@"
                int i = t$
            ");

            AssertNodeIsReference("t", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.Undefined);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = new CompletionExpression
                    {
                        Token = "t"
                    }
                });
        }

        [Test]
        public void CompletionInAssignment()
        {
            CompleteInMethod(@"
                int i;
                i = $
            ");

            AssertNodeIsAssignment("i", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("i"),
                    Type = Fix.Int
                },
                new Assignment
                {
                    Reference = VarRef("i"),
                    Expression = new CompletionExpression()
                });
        }

        [Test]
        public void CompletionInIfBody()
        {
            CompleteInMethod(@"
                if(true)
                {
                    $
                }
            ");

            AssertNodeIsIf(LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.InBody);

            AssertBody(
                new IfElseBlock
                {
                    Condition = new ConstantValueExpression(),
                    Then =
                    {
                        new ExpressionStatement
                        {
                            Expression = new CompletionExpression()
                        }
                    }
                });
        }

        [Test]
        public void MethodCall()
        {
            CompleteInMethod(@"
                this.GetHashCode();
                $
            ");

            AssertNodeIsCall("GetHashCode", LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.EmptyCompletionAfter);

            AssertBody(
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("this"),
                        MethodName = Fix.Object_GetHashCode,
                    }
                });
        }

        [Test]
        public void TriggeredOutsideOfMethods()
        {
            CompleteInClass(@"
                public static void M() {}
                $
            ");

            // TODO think about this simplification, perhaps it is better to create an artificial "no match" ITreeNode
            Assert.IsNull(LastCompletionMarker.AffectedNode);
            AssertCompletionCase(CompletionCase.Undefined);
        }
    }
}