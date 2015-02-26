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
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.LoopCondition;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.LoopCondition;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Statements;
using KaVE.Model.SSTs.Statements.Wrapped;
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
            //mA.Body.Add(new StatementCompletion());
            mA.Body.Add(new ReturnStatement {Expression = new ConstantValueExpression()});

            AssertAllMethods(mA);
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
            //mA.Body.Add(new StatementCompletion());
            mA.Body.Add(new ReturnStatement {Expression = ComposedExpression.Create("i")});

            AssertAllMethods(mA);
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
            //mA.Body.Add(new StatementCompletion());

            var ifElse = new IfElseBlock {Condition = new ConstantValueExpression()};
            ifElse.Then.Add(new ReturnStatement {Expression = new ConstantValueExpression()});

            mA.Body.Add(ifElse);
            mA.Body.Add(new ReturnStatement {Expression = new ConstantValueExpression()});

            AssertAllMethods(mA);
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
            //mA.Body.Add(new StatementCompletion());

            var ifElse = new IfElseBlock {Condition = new ConstantValueExpression()};
            ifElse.Then.Add(new ReturnStatement {Expression = new ConstantValueExpression()});
            ifElse.Else.Add(new ReturnStatement {Expression = new ConstantValueExpression()});

            mA.Body.Add(ifElse);

            AssertAllMethods(mA);
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
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));

            var whileLoop = new WhileLoop {Condition = new ConstantValueExpression()};
            whileLoop.Body.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"i"}}));
            //whileLoop.Body.Add(new StatementCompletion());

            mA.Body.Add(whileLoop);

            AssertAllMethods(mA);
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
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));

            var doLoop = new DoLoop {Condition = new ConstantValueExpression()};
            doLoop.Body.Add(new Assignment("i", new ComposedExpression {Variables = new[] {"i"}}));
            //whileLoop.Body.Add(new StatementCompletion());

            mA.Body.Add(doLoop);

            AssertAllMethods(mA);
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
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            mA.Body.Add(new Assignment("i", new ConstantValueExpression()));
            mA.Body.Add(VariableDeclaration.Create("s", Fix.String));

            var block = new BlockExpression(); //) {Value = new[] {"s"}};
            block.Body.Add(new Assignment("s", InvocationExpression.Create("o", Fix.ToString(Fix.Object))));
            var whileLoop = new WhileLoop {Condition = block};
            //whileLoop.Body.Add(new StatementCompletion());

            mA.Body.Add(whileLoop);

            AssertAllMethods(mA);
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
            forLoop.Init.Add(VariableDeclaration.Create("i", Fix.Int));
            forLoop.Init.Add(new Assignment("i", new ConstantValueExpression()));
            forLoop.Condition = new ComposedExpression {Variables = new[] {"i"}};

            forLoop.Body.Add(InvocationStatement.Create(Fix.ConsoleWrite(Fix.Int), Ref("i")));
            //forLoop.Body.Add(new StatementCompletion());

            mA.Body.Add(forLoop);

            AssertAllMethods(mA);
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
            mA.Body.Add(VariableDeclaration.Create("i", Fix.Int));
            var forLoop = new ForLoop();
            forLoop.Init.Add(new Assignment("i", new ConstantValueExpression()));
            forLoop.Condition = new ComposedExpression {Variables = new[] {"i"}};

            forLoop.Body.Add(InvocationStatement.Create(Fix.ConsoleWrite(Fix.Int), Ref("i")));
            //forLoop.Body.Add(new StatementCompletion());

            mA.Body.Add(forLoop);

            AssertAllMethods(mA);
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
            mA.Body.Add(VariableDeclaration.Create("$0", Fix.IntArray));
            mA.Body.Add(new Assignment("$0", new ConstantValueExpression()));
            var forEachLoop = new ForEachLoop {LoopedIdentifier = "$0", Decl = VariableDeclaration.Create("n", Fix.Int)};
            forEachLoop.Body.Add(InvocationStatement.Create(Fix.ConsoleWrite(Fix.Int), Ref("n")));
            //forEachLoop.Body.Add(new StatementCompletion());

            mA.Body.Add(forEachLoop);

            AssertAllMethods(mA);
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
            mA.Body.Add(VariableDeclaration.Create("o", Fix.Object));

            mA.Body.Add(VariableDeclaration.Create("o", Fix.Object));
            mA.Body.Add(new Assignment("o", InvocationExpression.Create(MethodName.Get("Object.ctor()"))));

            var usingBlock = new UsingBlock {Identifier = "o"};
            usingBlock.Body.Add(InvocationStatement.Create(MethodName.Get("Console.Write"), Ref("n")));
            usingBlock.Body.Add(new StatementCompletion());

            mA.Body.Add(usingBlock);

            AssertAllMethods(mA);
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
            usingBlock.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            usingBlock.Body.Add(new Assignment("$0", new ConstantValueExpression()));
            usingBlock.Body.Add(
                InvocationStatement.Create(
                    "s",
                    MethodName.Get(string.Format("[{0}] [{1}].WriteByte([{2}] value)", Fix.Void, stream, Fix.Byte)),
                    Ref("$0")));
            //usingBlock.Body.Add(new StatementCompletion());
            mA.Body.Add(usingBlock);

            AssertAllMethods(mA);
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
            //mA.Body.Add(new StatementCompletion());
            mA.Body.Add(new ThrowStatement {Exception = Fix.Exception});

            AssertAllMethods(mA);
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
                InvocationStatement.Create(
                    MethodName.Get(
                        string.Format("static [{0}] [System.Console, mscorlib, 4.0.0.0].WriteLine()", Fix.Void))));
            //tryBlock.Body.Add(new StatementCompletion());

            var namedCatch = new CatchBlock {Exception = VariableDeclaration.Create("e", Fix.IOException)};
            namedCatch.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            namedCatch.Body.Add(new Assignment("$0", InvocationExpression.Create("e", Fix.GetHashCode(Fix.Object))));
            namedCatch.Body.Add(InvocationStatement.Create(Fix.ConsoleWrite(Fix.Int), Ref("$0")));
            tryBlock.CatchBlocks.Add(namedCatch);
            var unnamedCatch = new CatchBlock {Exception = VariableDeclaration.Create(null, Fix.Exception)};
            unnamedCatch.Body.Add(VariableDeclaration.Create("$1", Fix.String));
            unnamedCatch.Body.Add(new Assignment("$1", new ConstantValueExpression()));
            unnamedCatch.Body.Add(InvocationStatement.Create(Fix.ConsoleWrite(Fix.String), Ref("$1")));
            tryBlock.CatchBlocks.Add(unnamedCatch);
            var catchAll = new CatchBlock();
            catchAll.Body.Add(VariableDeclaration.Create("$2", Fix.String));
            catchAll.Body.Add(new Assignment("$2", new ConstantValueExpression()));
            catchAll.Body.Add(InvocationStatement.Create(Fix.ConsoleWrite(Fix.String), Ref("$2")));
            tryBlock.CatchBlocks.Add(catchAll);
            tryBlock.Finally.Add(VariableDeclaration.Create("$3", Fix.String));
            tryBlock.Finally.Add(new Assignment("$3", new ConstantValueExpression()));
            tryBlock.Finally.Add(InvocationStatement.Create(Fix.ConsoleWrite(Fix.String), Ref("$3")));

            mA.Body.Add(tryBlock);

            AssertAllMethods(mA);
        }
    }
}