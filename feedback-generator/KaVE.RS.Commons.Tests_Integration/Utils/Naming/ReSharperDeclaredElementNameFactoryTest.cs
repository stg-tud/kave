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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite;
using KaVE.RS.Commons.Utils.Naming;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Utils.Naming
{
    internal class ReSharperDeclaredElementNameFactoryTest : BaseSSTAnalysisTest
    {
        // this is a new approach based on code, refer to the ".._unit" project for the old tests 

        [Test]
        public void ExtensionMethod()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    class C {
                        public void M() {
                            this.EM();
                            $
                        }
                    }
                    static class H {
                        public static void EM(this C c) {}
                    }
                }
            ");

            AssertBody(
                InvokeStaticStmt(
                    Names.Method("static [{0}] [N.H, TestProject].EM(this [N.C, TestProject] c)", Fix.Void),
                    RefExpr("this")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void NameOfGenericClasses()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    class C {
                        public void M() {
                            G<int> g = new G<int>();
                            $
                        }
                    }
                    class G<T> {
                        public class Nested{}
                    }
                }
            ");

            var type = string.Format("N.G`1[[T -> {0}]], TestProject", Fix.Int);
            var ctor = string.Format("[{0}] [{1}]..ctor()", Fix.Void, type);

            AssertBody(
                VarDecl("g", Names.Type(type)),
                Assign("g", InvokeCtor(Names.Method(ctor))),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void NameOfNestedClassesInGenericClasses()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    class C {
                        public void M() {
                            var n = new G<int>.Nested();
                            $
                        }
                    }
                    class G<T> {
                        public class Nested{}
                    }
                }
            ");

            var type = string.Format("N.G`1[[T -> {0}]]+Nested, TestProject", Fix.Int);
            var ctor = string.Format("[{0}] [{1}]..ctor()", Fix.Void, type);

            AssertBody(
                VarDecl("n", Names.Type(type)),
                Assign("n", InvokeCtor(Names.Method(ctor))),
                ExprStmt(new CompletionExpression()));
        }

        [TestCase("KaVE.Commons", "KaVE.Commons"), TestCase("0123456789_-", "0123456789_-"),
         TestCase("()[]{}=, ", "_________"), TestCase("äöü", "___")]
        public void SanitizeAssemblyName(string input, string expected)
        {
            Assert.AreEqual(expected, ReSharperDeclaredElementNameFactory.SanitizeAssemblyName(input));
        }
    }
}