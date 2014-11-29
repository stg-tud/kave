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
    [TestFixture]
    internal class ControlStructureTest : BaseSSTAnalysisTest
    {
        [Test]
        public void ReturnConst()
        {
            CompleteInClass(@"
                public int A()
                {
                    $
                    return 1;
                }
            ");

            var mA = NewMethodDeclaration(Fix.Int, "A");
            //mA.Body.Add(new CompletionTrigger());
            mA.Body.Add(new ReturnStatement {Expression = new ConstantExpression()});

            AssertEntryPoints(mA);
        }

        [Test]
        public void ReturnRef()
        {
            CompleteInClass(@"
                public int A(int i)
                {
                    $
                    return i;
                }
            ");

            var mA = NewMethodDeclaration(Fix.Int, "A", string.Format("[{0}] i", Fix.Int));
            //mA.Body.Add(new CompletionTrigger());
            mA.Body.Add(new ReturnStatement {Expression = ComposedExpression.Create("i")});

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

            var mA = NewMethodDeclaration(Fix.Int, "A");
            //mA.Body.Add(new CompletionTrigger());

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

            var mA = NewMethodDeclaration(Fix.Int, "A");
            //mA.Body.Add(new CompletionTrigger());

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

            var mA = NewMethodDeclaration(Fix.Int, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            var whileLoop = new WhileLoop {Condition = new ConstantExpression()};
            whileLoop.Body.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"i"}}));
            //whileLoop.Body.Add(new CompletionTrigger());

            mA.Body.Add(whileLoop);

            AssertEntryPoints(mA);
        }

        [Test]
        public void DoLoop()
        {
            CompleteInClass(@"
                public int A()
                {
                    var i = 0;
                    do
                    {
                        i = i + 1;
                        $
                    } while (true);
                }
            ");

            var mA = NewMethodDeclaration(Fix.Int, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));

            var doLoop = new DoLoop {Condition = new ConstantExpression()};
            doLoop.Body.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"i"}}));
            //whileLoop.Body.Add(new CompletionTrigger());

            mA.Body.Add(doLoop);

            AssertEntryPoints(mA);
        }

        [Test]
        public void WhileLoop_AssignmentInCondition()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    var i = 0;
                    string s;
                    while((s = o.ToString()) != null) {
                        i++;
                        $
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantExpression()));
            mA.Body.Add(new VariableDeclaration("s", Fix.String));

            var block = new BlockExpression();//) {Value = new[] {"s"}};
            block.Body.Add(new Assignment("s", new InvocationExpression("o", Fix.ToString(Fix.Object))));
            var whileLoop = new WhileLoop {Condition = block};
            //whileLoop.Body.Add(new CompletionTrigger());

            mA.Body.Add(whileLoop);

            AssertEntryPoints(mA);
        }

        [Test]
        public void ForLoop()
        {
            CompleteInClass(@"
                public void A()
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

            forLoop.Body.Add(new InvocationStatement(Fix.ConsoleWrite(Fix.Int), "i"));
            //forLoop.Body.Add(new CompletionTrigger());

            mA.Body.Add(forLoop);

            AssertEntryPoints(mA);
        }

        [Test]
        public void ForLoop_Predeclaration()
        {
            CompleteInClass(@"
                public void A()
                {
                    int i;
                    for(i = 0; i < 10; i++) {
                        Console.Write(i);
                        $
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("i", Fix.Int));
            var forLoop = new ForLoop();
            forLoop.Init.Add(new Assignment("i", new ConstantExpression()));
            forLoop.Condition = new ComposedExpression {Variables = new[] {"i"}};

            forLoop.Body.Add(new InvocationStatement(Fix.ConsoleWrite(Fix.Int), "i"));
            //forLoop.Body.Add(new CompletionTrigger());

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
                public void A()
                {
                    foreach(var n in new[] {1, 2, 3}) {
                        Console.Write(n);
                        $
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("$0", Fix.IntArray));
            mA.Body.Add(new Assignment("$0", new ConstantExpression()));
            var forEachLoop = new ForEachLoop {LoopedIdentifier = "$0", Decl = new VariableDeclaration("n", Fix.Int)};
            forEachLoop.Body.Add(new InvocationStatement(Fix.ConsoleWrite(Fix.Int), "n"));
            //forEachLoop.Body.Add(new CompletionTrigger());

            mA.Body.Add(forEachLoop);

            AssertEntryPoints(mA);
        }

        // TODO: handle "from Window window in windows select window.GetName()" like LINQ expression windows.Select(win => win.getName())

        [Test, Ignore]
        public void UsingBlock()
        {
            // TODO @Seb: object isn't disposable
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
        public void UsingBlockFromParameter()
        {
            var stream = TypeName.Get("System.IO.FileStream, mscorlib, 4.0.0.0");

            CompleteInClass(@"
                public void A(System.IO.FileStream s)
                {
                    using (s)
                    {
                        s.WriteByte(123);
                        $
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] s", stream));
            var usingBlock = new UsingBlock {Identifier = "s"};
            usingBlock.Body.Add(new VariableDeclaration("$0", Fix.Int));
            usingBlock.Body.Add(new Assignment("$0", new ConstantExpression()));
            usingBlock.Body.Add(
                new InvocationStatement(
                    "s",
                    MethodName.Get(string.Format("[{0}] [{1}].WriteByte([{2}] value)", Fix.Void, stream, Fix.Byte)),
                    "$0"));
            //usingBlock.Body.Add(new CompletionTrigger());
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
            //mA.Body.Add(new CompletionTrigger());
            mA.Body.Add(new ThrowStatement {Exception = Fix.Exception});

            AssertEntryPoints(mA);
        }

        [Test]
        public void TryCatchBlock()
        {
            CompleteInClass(@"
                public void A()
                {
                    try {
                        Console.WriteLine();
                        $
                    } catch(IOException e) {
                        Console.Write(e.GetHashCode());
                    } catch(Exception) {
                        Console.Write(""catch this"");
                    } catch {
                        Console.Write(""catch all"");
                    } finally {
                        Console.Write(""final"");
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            var tryBlock = new TryBlock();
            tryBlock.Body.Add(
                new InvocationStatement(
                    MethodName.Get(
                        string.Format("static [{0}] [System.Console, mscorlib, 4.0.0.0].WriteLine()", Fix.Void))));
            //tryBlock.Body.Add(new CompletionTrigger());

            var namedCatch = new CatchBlock {Exception = new VariableDeclaration("e", Fix.IOException)};
            namedCatch.Body.Add(new VariableDeclaration("$0", Fix.Int));
            namedCatch.Body.Add(new Assignment("$0", new InvocationExpression("e", Fix.GetHashCode(Fix.Object))));
            namedCatch.Body.Add(new InvocationStatement(Fix.ConsoleWrite(Fix.Int), "$0"));
            tryBlock.CatchBlocks.Add(namedCatch);
            var unnamedCatch = new CatchBlock {Exception = new VariableDeclaration(null, Fix.Exception)};
            unnamedCatch.Body.Add(new VariableDeclaration("$1", Fix.String));
            unnamedCatch.Body.Add(new Assignment("$1", new ConstantExpression()));
            unnamedCatch.Body.Add(new InvocationStatement(Fix.ConsoleWrite(Fix.String), "$1"));
            tryBlock.CatchBlocks.Add(unnamedCatch);
            var catchAll = new CatchBlock();
            catchAll.Body.Add(new VariableDeclaration("$2", Fix.String));
            catchAll.Body.Add(new Assignment("$2", new ConstantExpression()));
            catchAll.Body.Add(new InvocationStatement(Fix.ConsoleWrite(Fix.String), "$2"));
            tryBlock.CatchBlocks.Add(catchAll);
            tryBlock.Finally.Add(new VariableDeclaration("$3", Fix.String));
            tryBlock.Finally.Add(new Assignment("$3", new ConstantExpression()));
            tryBlock.Finally.Add(new InvocationStatement(Fix.ConsoleWrite(Fix.String), "$3"));

            mA.Body.Add(tryBlock);

            AssertEntryPoints(mA);
        }
    }
}