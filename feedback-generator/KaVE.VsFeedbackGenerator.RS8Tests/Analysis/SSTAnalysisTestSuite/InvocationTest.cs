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
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Statements;
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
            mA.Body.Add(new InvocationStatement("s", Fix.GetHashCode(Fix.String)));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("o", Fix.Object));
            //mA.Body.Add(new Assignment("o", new NewInstance(Fix.Object)));
            mA.Body.Add(new InvocationStatement("o", Fix.GetHashCode(Fix.Object)));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("$0", Fix.String));
            mA.Body.Add(new Assignment("$0", new ConstantExpression()));
            mA.Body.Add(new InvocationStatement("s", Fix.Equals(Fix.String, Fix.String, "value"), "$0"));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new InvocationStatement("s", Fix.Equals(Fix.String, Fix.Object, "obj"), "this"));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new VariableDeclaration("$0", Fix.String));
            mA.Body.Add(
                new Assignment(
                    "$0",
                    new InvocationExpression("s", MethodName.Get(string.Format("[{0}] [{0}].Normalize()", Fix.String)))));
            mA.Body.Add(new InvocationStatement("s", Fix.Equals(Fix.String, Fix.String, "value"), "$0"));

            AssertEntryPoints(mA);
        }

        [Test, Ignore]
        public void ExternalCallChain()
        {
            CompleteInClass(@"
                public void A(string s)
                {
                    s.GetHashCode().ToString();
                    $
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("$0", Fix.Int));
            mA.Body.Add(new Assignment("$0", new InvocationExpression("s", Fix.GetHashCode(Fix.Object))));
            mA.Body.Add(new InvocationStatement("$0", Fix.GetHashCode(Fix.Int)));

            AssertEntryPoints(mA);
        }

        [Test, Ignore]
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
            mA.Body.Add(new InvocationStatement("this", MethodName.Get("C.B")));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertEntryPoints(mA, mB);
        }

        [Test, Ignore]
        public void ThisWithHiding()
        {
            CompleteInFile(@"
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
            mA.Body.Add(new InvocationStatement("this", MethodName.Get("I.B")));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertEntryPoints(mA, mB);
        }

        [Test, Ignore]
        public void StaticInvocation()
        {
            CompleteInFile(@"
                public class C {
                    public void A()
                    {
                        Console.Write(0);
                        $
                    }
                }
            ");

            var mA = NewMethodDeclaration(Fix.Void, "A");

            mA.Body.Add(new VariableDeclaration("v0", Fix.Int));
            mA.Body.Add(new Assignment("v0", new ConstantExpression()));
            mA.Body.Add(new InvocationStatement(MethodName.Get("Console.Write"), "v0"));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertEntryPoints(mA, mB);
        }
    }
}