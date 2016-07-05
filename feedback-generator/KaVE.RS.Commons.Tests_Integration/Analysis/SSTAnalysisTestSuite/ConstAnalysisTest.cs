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

using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    internal class ConstAnalysisTest : BaseSSTAnalysisTest
    {
        [Test]
        public void ConstantVariable()
        {
            CompleteInMethod(@"
                const int i = 1;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("1")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void MultipleConstantVariables()
        {
            CompleteInMethod(@"
                const int i = 1, j = 2;
                $
            ");

            AssertBody(
                VarDecl("i", Fix.Int),
                Assign("i", Const("1")),
                VarDecl("j", Fix.Int),
                Assign("j", Const("2")),
                ExprStmt(new CompletionExpression()));
        }

        [Test]
        public void ConstantField()
        {
            CompleteInClass(@"const int SomeConstant = 1; $");

            var expected =
                Sets.NewHashSet(
                    new FieldDeclaration
                    {
                        Name = FieldName.Get("static [System.Int32, mscorlib, 4.0.0.0] [N.C, TestProject].SomeConstant")
                    });
            Assert.AreEqual(expected, ResultSST.Fields);
        }

        [Test]
        public void MultipleConstantFields()
        {
            CompleteInClass(@"const int SomeConstant = 1, OtherConstant = 2; $");

            var expected =
                Sets.NewHashSet(
                    new FieldDeclaration
                    {
                        Name = FieldName.Get("static [System.Int32, mscorlib, 4.0.0.0] [N.C, TestProject].SomeConstant")
                    },
                    new FieldDeclaration
                    {
                        Name =
                            FieldName.Get("static [System.Int32, mscorlib, 4.0.0.0] [N.C, TestProject].OtherConstant")
                    });
            Assert.AreEqual(expected, ResultSST.Fields);
        }
    }
}