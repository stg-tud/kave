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
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.Expressions
{
    internal class ReferenceExpressionTest : BaseSSTAnalysisTest
    {
        [Test]
        public void SimpleDeclaration()
        {
            CompleteInMethod(@"
                object o = null;
                $
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", new NullExpression()),
                Fix.EmptyCompletion);
        }

        [Test]
        public void Reference()
        {
            CompleteInMethod(@"
                object o = null;
                o$
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", new NullExpression()),
                ExprStmt(
                    new CompletionExpression
                    {
                        Token = "o"
                    }));
        }

        [Test]
        public void Reference_WithDot()
        {
            CompleteInMethod(@"
                object o = null;
                o.$
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", new NullExpression()),
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("o"),
                        Token = ""
                    }));
        }

        [Test]
        public void Reference_WithQualifier()
        {
            CompleteInMethod(@"
                object o = null;
                o.f$
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", new NullExpression()),
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("o"),
                        Token = "f"
                    }));
        }

        [Test]
        public void Reference_OnReference()
        {
            CompleteInMethod(@"
                object o = null;
                o.f.g$
            ");

            AssertBody(
                VarDecl("o", Fix.Object),
                Assign("o", new NullExpression()),
                VarDecl("$0", Fix.Unknown),
                Assign(
                    "$0",
                    new ReferenceExpression
                    {
                        // TODO @seb: extend the implementation here and add valid fieldname!
                        Reference = new FieldReference
                        {
                            Reference = VarRef("o"),
                            FieldName = FieldName.Get(string.Format("[{0}] [{0}].f", Fix.Unknown))
                        }
                    }),
                ExprStmt(
                    new CompletionExpression
                    {
                        VariableReference = VarRef("$0"),
                        Token = "g"
                    }));
        }
    }
}