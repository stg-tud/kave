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
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    [TestFixture]
    internal class InvocationTest : BaseSSTAnalysisTest
    {
        [Test]
        public void ExternalSimple()
        {
            CompleteInClass(@"
                public void A(string s)
                {
                    s.GetHashCode();
                    $
                }
            ");
            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] s", Fix.String));
            mA.Body.Add(SSTUtil.InvocationExpression("s", Fix.GetHashCode(Fix.String)));

            AssertAllMethods(mA);
        }

        [Test, Ignore]
        public void Constructor()
        {
            CompleteInClass(@"
                public void A()
                {
                    var o = new Object();
                    $
                }
            ");
            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("o", Fix.Object));
            mA.Body.Add(
                SSTUtil.AssignmentToLocal(
                    "o",
                    SSTUtil.InvocationExpression(
                        MethodName.Get(string.Format("[{0}] [{1}]..ctor()", Fix.Void, Fix.Object)))));

            AssertAllMethods(mA);
        }

        [Test]
        public void ExternalSimpleWithConstant()
        {
            CompleteInClass(@"
                public void A(string s)
                {
                    s.Equals(""SomeString"");
                    $
                }
            ");
            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] s", Fix.String));
            mA.Body.Add(VariableDeclaration.Create("$0", Fix.String));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", new ConstantValueExpression()));
            var ref0 = new ReferenceExpression {Reference = new VariableReference {Identifier = "o$"}};
            mA.Body.Add(SSTUtil.InvocationExpression("s", Fix.Equals(Fix.String, Fix.String, "value"), ref0));

            AssertAllMethods(mA);
        }

        [Test]
        public void ExternalSimpleWithArithmetics()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    i.Equals(0 + i);
                    $
                }
            ");
            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.ComposedExpression("i")));
            mA.Body.Add(SSTUtil.InvocationExpression("i", Fix.Equals(Fix.Int, Fix.Int, "obj"), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void ExternalSimpleWithUnary()
        {
            CompleteInClass(@"
                public void A(bool b)
                {
                    b.Equals(!b);
                    $
                }
            ");
            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] b", Fix.Bool));
            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Bool));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.ComposedExpression("b")));
            mA.Body.Add(SSTUtil.InvocationExpression("b", Fix.Equals(Fix.Bool, Fix.Bool, "obj"), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void ExternalSimpleWithArray()
        {
            CompleteInClass(@"
                public void B(int[] a) {}
                public void A(int i)
                {
                    B(new []{i});
                    $
                }
            ");
            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));
            mA.Body.Add(VariableDeclaration.Create("$0", Fix.IntArray));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.ComposedExpression("i")));
            mA.Body.Add(
                SSTUtil.InvocationExpression(
                    "this",
                    MethodName.Get(string.Format("[{0}] [N.C, TestProject].B([{1}] a)", Fix.Void, Fix.IntArray)),
                    Ref("$0")));

            var mB = NewMethodDeclaration(Fix.Void, "B", string.Format("[{0}] a", Fix.IntArray));
            mB.IsEntryPoint = false;

            AssertAllMethods(mA, mB);
        }

        [Test]
        public void ExternalSimpleWithParam()
        {
            CompleteInClass(@"
                public void A(string s)
                {
                    s.Equals(this);
                    $
                }
            ");
            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] s", Fix.String));
            mA.Body.Add(SSTUtil.InvocationExpression("s", Fix.Equals(Fix.String, Fix.Object, "obj"), Ref("this")));

            AssertAllMethods(mA);
        }

        [Test]
        public void ExternalNested()
        {
            CompleteInClass(@"
                public void A(string s)
                {
                    s.Equals(s.Normalize());
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] s", Fix.String));
            mA.Body.Add(VariableDeclaration.Create("$0", Fix.String));
            mA.Body.Add(
                SSTUtil.AssignmentToLocal(
                    "$0",
                    SSTUtil.InvocationExpression(
                        "s",
                        MethodName.Get(string.Format("[{0}] [{0}].Normalize()", Fix.String)))));
            mA.Body.Add(SSTUtil.InvocationExpression("s", Fix.Equals(Fix.String, Fix.String, "value"), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void ExternalCallChain()
        {
            CompleteInClass(@"
                public void A(string s)
                {
                    s.GetHashCode().ToString();
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] s", Fix.String));
            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.InvocationExpression("s", Fix.GetHashCode(Fix.String))));
            mA.Body.Add(SSTUtil.InvocationExpression("$0", Fix.ToString(Fix.Int)));

            AssertAllMethods(mA);
        }

        [Test]
        public void ThisSimple()
        {
            CompleteInClass(@"
                public void A()
                {
                    B();
                    $
                }
                public void B() {}
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(
                SSTUtil.InvocationExpression(
                    "this",
                    MethodName.Get(string.Format("[{0}] [N.C, TestProject].B()", Fix.Void))));

            var mB = NewMethodDeclaration(Fix.Void, "B");
            mB.IsEntryPoint = false;

            AssertAllMethods(mA, mB);
        }

        [Test]
        public void ThisExplicitly()
        {
            CompleteInClass(@"
                public void A()
                {
                    this.B();
                    $
                }
                public void B() {}
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(
                SSTUtil.InvocationExpression(
                    "this",
                    MethodName.Get(string.Format("[{0}] [N.C, TestProject].B()", Fix.Void))));

            var mB = NewMethodDeclaration(Fix.Void, "B");
            mB.IsEntryPoint = false;

            AssertAllMethods(mA, mB);
        }

        [Test]
        public void BaseWithOverride()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public class D
                    {
                        public virtual void B() {}
                    }
                    public class C : D
                    {
                        public void A()
                        {
                            base.B();
                            $
                        }
                        public override void B() {}
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(
                SSTUtil.InvocationExpression(
                    "base",
                    MethodName.Get(string.Format("[{0}] [N.D, TestProject].B()", Fix.Void))));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertAllMethods(mA, mB);
        }

        [Test]
        public void BaseWithShadowing()
        {
            CompleteInCSharpFile(@"
                namespace N
                {
                    public class D
                    {
                        public void B() {}
                    }
                    public class C : D
                    {
                        public void A()
                        {
                            base.B();
                            $
                        }
                        public void B() {}
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(
                SSTUtil.InvocationExpression(
                    "base",
                    MethodName.Get(string.Format("[{0}] [N.D, TestProject].B()", Fix.Void))));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertAllMethods(mA, mB);
        }

        [Test, Ignore]
        public void ThisWithHiding()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    public interface I {
                        void A();
                        void B();
                    }
                    public class C : I {
                        public void A()
                        {
                            B();
                            $
                        }
                        public void B() {}
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(
                SSTUtil.InvocationExpression(
                    "this",
                    MethodName.Get(string.Format("[{0}] [N.I, TestProject].B()", Fix.Void))));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertAllMethods(mA, mB);
        }

        [Test]
        public void StaticInvocation()
        {
            CompleteInClass(@"
                public void A()
                {
                    Console.Write(0);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void As_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    Console.Write(o as int);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.ComposedExpression("o")));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void As_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    Console.Write(1.0 as string);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.String));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.String), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Is_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    Console.Write(o is int);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Bool));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.ComposedExpression("o")));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Bool), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Is_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    Console.Write(1.0 is int);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Bool));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Bool), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Cast_Const()
        {
            CompleteInClass(@"
                public void A()
                {
                    Console.Write((int) 1.0);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Cast_Reference()
        {
            CompleteInClass(@"
                public void A(object o)
                {
                    Console.Write((int) o);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] o", Fix.Object));

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.ComposedExpression("o")));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Assignment()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    int j;
                    Console.Write(j = i);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));

            mA.Body.Add(VariableDeclaration.Create("j", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("j", SSTUtil.ComposedExpression("i")));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("j")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Postfix()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    Console.Write(i++);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.ComposedExpression("i")));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Prefix()
        {
            CompleteInClass(@"
                public void A(int i)
                {
                    Console.Write(++i);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A", string.Format("[{0}] i", Fix.Int));

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", SSTUtil.ComposedExpression("i")));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void TypeOf()
        {
            CompleteInClass(@"
                public void A()
                {
                    Console.Write(typeof(int));
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(VariableDeclaration.Create("$0", TypeName.Get("System.Type, mscorlib, 4.0.0.0")));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Object), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void Default()
        {
            CompleteInClass(@"
                public void A()
                {
                    Console.Write(default(int));
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(SSTUtil.AssignmentToLocal("$0", new ConstantValueExpression()));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("$0")));

            AssertAllMethods(mA);
        }

        [Test]
        public void InlineIfElse()
        {
            CompleteInClass(@"
                public void A()
                {
                    Console.Write(true ? 1 : 2);
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(VariableDeclaration.Create("$0", Fix.Int));
            mA.Body.Add(
                SSTUtil.AssignmentToLocal(
                    "$0",
                    new IfElseExpression
                    {
                        Condition = new ConstantValueExpression(),
                        ThenExpression = new ConstantValueExpression(),
                        ElseExpression = new ConstantValueExpression()
                    }));
            mA.Body.Add(SSTUtil.InvocationExpression(Fix.ConsoleWrite(Fix.Int), Ref("$0")));

            AssertAllMethods(mA);
        }
    }
}