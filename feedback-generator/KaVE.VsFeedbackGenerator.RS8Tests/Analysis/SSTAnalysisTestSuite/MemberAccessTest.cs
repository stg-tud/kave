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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class MemberAccessTest : BaseSSTAnalysisTest
    {
        [Test, Ignore]
        public void MemberAccess()
        {
            CompleteInCSharpFile(@"
                public class A
                {
                    public int I = 3;
                }

                public class B
                {
                    public void M()
                    {
                        var a = new A();
                        var i = a.I;
                        $
                    }
                }
            ");

            AssertBody(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = SSTAnalysisFixture.Type("A")
                },
                new Assignment
                {
                    Reference = VarRef("a"),
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("a"),
                        MethodName =
                            SSTAnalysisFixture.Method(SSTAnalysisFixture.Int, SSTAnalysisFixture.Type("A"), ".ctor"),
                    }
                },
                new VariableDeclaration
                {
                    Reference = VarRef("c"),
                    Type = SSTAnalysisFixture.Int
                },
                new Assignment
                {
                    Reference = VarRef("a"),
                    Expression = new ReferenceExpression
                    {
                        Reference = new FieldReference
                        {
                            Reference = VarRef("a"),
                            FieldName =
                                SSTAnalysisFixture.Field(SSTAnalysisFixture.Int, SSTAnalysisFixture.Type("A"), "I"),
                        }
                    }
                });
        }

        [Test, Ignore]
        public void MemberAccess_Constant1()
        {
            CompleteInCSharpFile(@"
                public class A
                {
                    public static readonly int I = 3;
                }

                public class B
                {
                    public void M()
                    {
                        var i = A.I;
                        $
                    }
                }
            ");

            var body = Lists.NewList<IStatement>();
            body.Add(SSTUtil.Declare("i", SSTAnalysisFixture.Int));
            var field = FieldName.Get("[System.Int32, mscore, 4.0.0.0] [N.A, TestProject].I");
            //body.Add(SSTUtil.AssignmentToLocal("i", new ConstantReferenceExpression {Reference = field}));

            AssertBody(body);
            // TODO
        }
    }
}