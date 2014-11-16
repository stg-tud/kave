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

using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    [Ignore]
    internal class ControlStructureTest : BaseSSTAnalysisTest
    {
        [Test]
        public void Return()
        {
            CompleteInClass(@"
                public int A()
                {
                    $
                    return 1;
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new CompletionTrigger());
            mA.Body.Add(new ReturnStatement {Expression = new ConstantExpression()});

            AssertEntryPoints(mA);
        }

        [Test]
        public void IfBlock()
        {
            CompleteInClass(@"
                public int A()
                {
                    $
                    if(true) {
                        return 1;
                    }
                    return 2;
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new CompletionTrigger());

            var ifElse = new IfElseBlock {Condition = new ConstantExpression()};
            ifElse.Then.Add(new ReturnStatement {Expression = new ConstantExpression()});

            mA.Body.Add(ifElse);
            mA.Body.Add(new ReturnStatement {Expression = new ConstantExpression()});

            AssertEntryPoints(mA);
        }

        [Test]
        public void IfElseBlock()
        {
            CompleteInClass(@"
                public int A()
                {
                    $
                    if(true) {
                        return 1;
                    } else {
                        return 2;
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new CompletionTrigger());

            var ifElse = new IfElseBlock {Condition = new ConstantExpression()};
            ifElse.Then.Add(new ReturnStatement {Expression = new ConstantExpression()});
            ifElse.Else.Add(new ReturnStatement {Expression = new ConstantExpression()});

            mA.Body.Add(ifElse);

            AssertEntryPoints(mA);
        }

        [Test]
        public void WhileLoop()
        {
            CompleteInClass(@"
                public int A()
                {
                    var i = 0;
                    while(true) {
                        i = i + 1;
                        $
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));

            var whileLoop = new WhileLoop {Condition = new ConstantExpression()};

            whileLoop.Body.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"i"}}));
            whileLoop.Body.Add(new CompletionTrigger());

            mA.Body.Add(whileLoop);

            AssertEntryPoints(mA);
        }

        [Test]
        public void ForLoop()
        {
            CompleteInClass(@"
                public int A()
                {
                    for(int i = 0; i < 10; i++) {
                        Console.Write(i);
                        $
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            var forLoop = new ForLoop();
            forLoop.Init.Add(new VariableDeclaration("i", Fix.Int));
            forLoop.Init.Add(new Assignment("i", new ConstantExpression()));
            forLoop.Condition = new ComposedExpression {Variables = new[] {"i"}};
            forLoop.Step.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"i"}}));

            forLoop.Body.Add(new InvocationStatement(MethodName.Get("Console.Write"), "i"));
            forLoop.Body.Add(new CompletionTrigger());

            mA.Body.Add(forLoop);

            AssertEntryPoints(mA);
        }

        /*  WTF:
         * public static void Main(string[] args) {
                for (dynamic x = 0, y = new MyClass { a = 20, b = 30 }; x < 100; x++, y.a++, y.b--) {
                    Console.Write("X=" + x + " (" + x.GetType() + "\n" +
                                  "Y.a=" + y.a + ",Y.b=" + y.b + " (" + y.GetType() + "\n");
                 }
            }

            class MyClass {
                public int a = 0, b = 0;
            }
         * --> create more complex exmaple with calls and multiple variables
         */

        [Test]
        public void ForEachLoop()
        {
            CompleteInClass(@"
                public int A()
                {
                    foreach(var n in new[] {1, 2, 3}) {
                        Console.Write(n);
                        $
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("v0", Fix.IntArray));
            mA.Body.Add(new Assignment("v0", new ConstantExpression()));
            var forEachLoop = new ForEachLoop {LoopedIdentifier = "v0"};
            forEachLoop.Body.Add(new InvocationStatement(MethodName.Get("Console.Write"), "n"));
            forEachLoop.Body.Add(new CompletionTrigger());

            mA.Body.Add(forEachLoop);

            AssertEntryPoints(mA);
        }

        // TODO: handle "from Window window in windows select window.GetName()" like LINQ expression windows.Select(win => win.getName())

        [Test]
        public void UsingBlock()
        {
            CompleteInClass(@"
                public int A()
                {
                    using (var o = new Object())
	                {
	                    Console.WriteLine(o);
                        $
	                }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("o", Fix.Object));

            mA.Body.Add(new VariableDeclaration("o", Fix.Object));
            mA.Body.Add(new Assignment("o", new InvocationExpression(MethodName.Get("Object.ctor()"))));

            var usingBlock = new UsingBlock {Identifier = "o"};
            usingBlock.Body.Add(new InvocationStatement(MethodName.Get("Console.Write"), "n"));
            usingBlock.Body.Add(new CompletionTrigger());

            mA.Body.Add(usingBlock);

            AssertEntryPoints(mA);
        }

        [Test]
        public void ThrowStatement()
        {
            CompleteInClass(@"
                public void A()
                {
                    $
                    throw new Exception();
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new CompletionTrigger());
            mA.Body.Add(new ThrowStatement {Exception = Fix.Exception});

            AssertEntryPoints(mA);
        }

        [Test]
        public void TryCatchBlock()
        {
            CompleteInClass(@"
                public int A()
                {
                    try {
                        Console.GetHashCode();
                        $
                    } catch(Exception e) {
                        Console.Read();
                        e.GetStacktrace();
                    } catch(Exception) {
                        Console.Read();
                    } catch {
                        Console.Read();
                    } finally {
                        Console.Beep();
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            var tryBlock = new TryBlock();

            tryBlock.Body.Add(new InvocationStatement(MethodName.Get("Console.GetHashCode()")));
            tryBlock.Body.Add(new CompletionTrigger());

            tryBlock.CatchBlocks.Add(new CatchBlock {Exception = new VariableDeclaration("e", Fix.Exception)});
            tryBlock.CatchBlocks.Add(new CatchBlock {Exception = new VariableDeclaration(null, Fix.Exception)});
            tryBlock.CatchBlocks.Add(new CatchBlock());
            tryBlock.Finally.Add(new InvocationStatement(MethodName.Get("Console.Beep()")));

            AssertEntryPoints(mA);
        }
    }
}