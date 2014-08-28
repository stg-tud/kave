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
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    [Ignore]
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
            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new Invocation("s", MethodName.Get("string.getHashCode")));

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
            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("v0", Fix.String));
            mA.Body.Add(new Assignment("v0", new ConstantExpression()));
            mA.Body.Add(new Invocation("s", MethodName.Get("string.getHashCode"), "v0"));

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
            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new Invocation("s", MethodName.Get("string.getHashCode"), "this"));

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

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(new VariableDeclaration("v0", Fix.String));
            mA.Body.Add(new Assignment("v0", new Invocation("s", MethodName.Get("string.Normalize"))));
            mA.Body.Add(new Invocation("s", MethodName.Get("string.getHashCode"), "v0"));

            AssertEntryPoints(mA);
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
            mA.Body.Add(new Invocation("this", MethodName.Get("C.B")));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertEntryPoints(mA, mB);
        }

        [Test]
        public void ThisWithOverriddes()
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
            mA.Body.Add(new Invocation("this", MethodName.Get("I.B")));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertEntryPoints(mA, mB);
        }

        [Test]
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
            mA.Body.Add(new Invocation(MethodName.Get("Console.Write"), "v0"));

            var mB = NewMethodDeclaration(Fix.Void, "B");

            AssertEntryPoints(mA, mB);
        }
    }
}