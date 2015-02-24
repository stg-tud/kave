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

using KaVE.Model.Collections;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.Basic;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class BasicExpressionTest : BaseSSTAnalysisTest
    {
        [Test]
        public void ConstantValue()
        {
            CompleteInMethod(@"
                int i = 1;
                $
            ");

            var body = Lists.NewList<Statement>();
            body.Add(new VariableDeclaration("i", Fix.Int));
            body.Add(new Assignment("i", new ConstantValueExpression()));

            AssertBody(body);
        }

        [Test]
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
                        var i = A.I;
                        $
                    }
                }
            ");

            var body = Lists.NewList<Statement>();
            body.Add(new VariableDeclaration("a", TypeName.Get("A, TestProject")));
            var methodName = MethodName.Get("[System.Void, mscore, 4.0.0.0] [A.TestProject]..ctor()");
            body.Add(new Assignment("a", new InvocationExpression {Identifier = "a", Name = methodName}));
            body.Add(new VariableDeclaration("c", Fix.Int));
            body.Add(new Assignment("i", new MemberAccessExpression {Identifier = "a", MemberName = "I"}));

            AssertBody(body);
        }

        [Test]
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

            var body = Lists.NewList<Statement>();
            body.Add(new VariableDeclaration("i", Fix.Int));
            var field = FieldName.Get("[System.Int32, mscore, 4.0.0.0] [N.A, TestProject].I");
            body.Add(new Assignment("i", new ConstantReferenceExpression {Reference = field}));

            AssertBody(body);
            // TODO
        }

        [Test]
        public void NullReference()
        {
            CompleteInMethod(@"
                object o = null;
                $
            ");

            var body = Lists.NewList<Statement>();
            body.Add(new VariableDeclaration("o", Fix.Object));
            body.Add(new Assignment("o", new NullExpression()));

            AssertBody(body);
        }

        [Test]
        public void ReferenceExpression()
        {
            CompleteInMethod(@"
                object o = new object();
                object o2 = o;
                $
            ");

            var body = Lists.NewList<Statement>();
            body.Add(new VariableDeclaration("o", Fix.Object));
            body.Add(new Assignment("o", new InvocationExpression {Identifier = "o", Name = Fix.Object_Init}));
            body.Add(new VariableDeclaration("o2", Fix.Object));
            body.Add(new Assignment("o2", new ReferenceExpression {Identifier = "o"}));

            AssertBody(body);
        }
    }
}