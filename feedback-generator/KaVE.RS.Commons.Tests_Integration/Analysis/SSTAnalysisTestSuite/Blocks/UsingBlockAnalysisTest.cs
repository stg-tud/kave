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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.Blocks
{
    internal class UsingBlockAnalysisTest : BaseSSTAnalysisTest
    {
        private readonly ITypeName _streamWriter = TypeName.Get("System.IO.StreamWriter, mscorlib, 4.0.0.0");

        private readonly IMethodName _streamWriterCtor =
            MethodName.Get(
                "[System.Void, mscorlib, 4.0.0.0] [System.IO.StreamWriter, mscorlib, 4.0.0.0]..ctor([System.String, mscorlib, 4.0.0.0] path)");

        [Test]
        public void Default()
        {
            CompleteInMethod(@"
                using (var sw = new StreamWriter(""file.txt""))
                {
                    $
                }");

            AssertBody(
                VarDecl("sw", _streamWriter),
                VarAssign("sw", InvokeCtor(_streamWriterCtor, new ConstantValueExpression())),
                new UsingBlock
                {
                    Reference = VarRef("sw"),
                    Body =
                    {
                        Fix.EmptyCompletion
                    }
                });
        }

        [Test]
        public void Default_CompletionWithToken()
        {
            CompleteInMethod(@"
                using (var sw = new StreamWriter(""file.txt""))
                {
                    s$
                }");

            AssertBody(
                VarDecl("sw", _streamWriter),
                VarAssign("sw", InvokeCtor(_streamWriterCtor, new ConstantValueExpression())),
                new UsingBlock
                {
                    Reference = VarRef("sw"),
                    Body =
                    {
                        new ExpressionStatement {Expression = new CompletionExpression {Token = "s"}}
                    }
                });
        }

        [Test]
        public void Unassigned()
        {
            CompleteInMethod(@"
                using (new StreamWriter(""file.txt""))
                {
                    $
                    return;
                }");

            AssertBody(
                VarDecl("$0", _streamWriter),
                VarAssign("$0", InvokeCtor(_streamWriterCtor, new ConstantValueExpression())),
                new UsingBlock
                {
                    Reference = VarRef("$0"),
                    Body =
                    {
                        Fix.EmptyCompletion,
                        new ReturnStatement {IsVoid = true}
                    }
                });
        }

        [Test]
        public void OnVarRef()
        {
            CompleteInMethod(@"
                var sw = new StreamWriter(""file.txt"");
                using (sw)
                {
                    $
                    return;
                }");

            AssertBody(
                VarDecl("sw", _streamWriter),
                VarAssign("sw", InvokeCtor(_streamWriterCtor, new ConstantValueExpression())),
                new UsingBlock
                {
                    Reference = VarRef("sw"),
                    Body =
                    {
                        Fix.EmptyCompletion,
                        new ReturnStatement {IsVoid = true}
                    }
                });
        }

        [Test]
        public void NestedUsings()
        {
            CompleteInMethod(@"
                using (var sw = new StreamWriter(""file.txt""))
                using (var sw2 = new StreamWriter(""file2.txt""))
                {
                    $
                    return;
                }");

            AssertBody(
                VarDecl("sw", _streamWriter),
                VarAssign("sw", InvokeCtor(_streamWriterCtor, new ConstantValueExpression())),
                new UsingBlock
                {
                    Reference = VarRef("sw"),
                    Body =
                    {
                        VarDecl("sw2", _streamWriter),
                        VarAssign("sw2", InvokeCtor(_streamWriterCtor, new ConstantValueExpression())),
                        new UsingBlock
                        {
                            Reference = VarRef("sw2"),
                            Body =
                            {
                                Fix.EmptyCompletion,
                                new ReturnStatement {IsVoid = true}
                            }
                        }
                    }
                });
        }

        [Test]
        public void VariableFromMethodCall()
        {
            CompleteInClass(@"
                public StreamWriter M()
                {
                    return new StreamWriter(""file.txt"");
                }
                
                public void N()
                {
                    using (M())
                    {
                        $
                        return;
                    }
                }");

            AssertBody(
                "N",
                VarDecl("$0", _streamWriter),
                VarAssign("$0", Invoke("this", Fix.Method(_streamWriter, Type("C"), "M"))),
                new UsingBlock
                {
                    Reference = VarRef("$0"),
                    Body =
                    {
                        Fix.EmptyCompletion,
                        new ReturnStatement {IsVoid = true}
                    }
                });
        }

        [Test]
        public void Default_CompleteBefore()
        {
            CompleteInMethod(@"
                $
                using (var sw = new StreamWriter(""file.txt""))
                {
                    return;
                }");

            AssertBody(
                Fix.EmptyCompletion,
                VarDecl("sw", _streamWriter),
                VarAssign("sw", InvokeCtor(_streamWriterCtor, new ConstantValueExpression())),
                new UsingBlock
                {
                    Reference = VarRef("sw"),
                    Body =
                    {
                        new ReturnStatement {IsVoid = true}
                    }
                });
        }

        [Test]
        public void Default_CompleteAfter()
        {
            CompleteInMethod(@"
                using (var sw = new StreamWriter(""file.txt""))
                {
                    return;
                }
                $");

            AssertBody(
                VarDecl("sw", _streamWriter),
                VarAssign("sw", InvokeCtor(_streamWriterCtor, new ConstantValueExpression())),
                new UsingBlock
                {
                    Reference = VarRef("sw"),
                    Body =
                    {
                        new ReturnStatement {IsVoid = true}
                    }
                },
                Fix.EmptyCompletion);
        }
    }
}