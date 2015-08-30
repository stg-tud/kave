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

using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    [Ignore]
    internal class CurrentlyBrokenTests : BaseSSTAnalysisTest
    {
        [Test]
        public void StaticMembersWhenCalledByAlias()
        {
            // This completion is only misanalysed when an alias is used. Completing "Object.$" yields correct results.
            // This is also true for other aliases such as "string" or "int".
            CompleteInMethod(@"
                object.$
            ");

            AssertBody(
                ExprStmt(
                    new CompletionExpression
                    {
                        TypeReference = Fix.Object,
                        Token = ""
                    }));
        }

        [Test]
        public void MultipleConditionsInIfBlock()
        {
            CompleteInMethod(@"
                if (true && true)
                {
                    $
                }");

            AssertBody(
                VarDecl("$0", Fix.Bool),
                Assign("$0", new ComposedExpression()),
                new IfElseBlock
                {
                    Condition = RefExpr("$0"),
                    Then =
                    {
                        ExprStmt(new CompletionExpression())
                    }
                });
        }

        [Test]
        public void CallingMethodWithProperty()
        {
            CompleteInClass(@"        
                public string Name { get; set; }

                public void M()
                {
                    N(this.Name);
                }

                public void N(string s)
                {
                    s.$
                }");

            var nMethod = Fix.Method(Fix.Void, Type("C"), "N", Fix.Parameter(Fix.String, "s"));
            var namePropertyRef = PropertyRef(Fix.Property(Fix.String, Type("C"), "Name"), VarRef("this"));

            AssertBody(
                "M",
                ExprStmt(Invoke("this", nMethod, RefExpr(namePropertyRef))));
        }
    }
}