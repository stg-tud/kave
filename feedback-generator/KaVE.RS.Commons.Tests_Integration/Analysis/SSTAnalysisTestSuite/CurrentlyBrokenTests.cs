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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
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

            AssertBody(ExprStmt(Fix.CompletionOnType(Fix.Object, "")));
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

        [Test]
        public void UnnecessaryReassignmentOfThis()
        {
            CompleteInClass(@"
                public void M()
                {
                    this.GetH$
                }");

            //C $0;
            //$0 = this;
            //$0.GetH$;

            AssertBody(
                "M",
                ExprStmt(Fix.CompletionOnVar(VarRef("this"), "GetH")));
        }

        [Test]
        public void AssigningArray()
        {
            CompleteInMethod(@"
                var array = new[] {1, 2, 3, 4, 5};
                array.$");

            // Analysis will assign UnknownExpression. Should probably be ConstantValueExpression instead.
            AssertBody(
                VarDecl("array", ArrayTypeName.From(Fix.Int, 1)),
                Assign("array", new ConstantValueExpression()),
                ExprStmt(Fix.CompletionOnVar(VarRef("array"), "")));

            Assert.Fail();
        }

        [Test]
        public void ExtensionMethod()
        {
            CompleteInMethod(@"
                static class C2
                {
                    public static void DoSth(this C1 a, int arg) { }
                }

                class C1
                {
                    public void M()
                    {
                        var a = new C1();
                        a.DoSth(1);
                        $
                    }
                }");


            //{
            //    "$type": "[SST:Statements.ExpressionStatement]",
            //    "Expression": {
            //        "$type": "[SST:Expressions.Assignable.InvocationExpression]",
            //        "Reference": {
            //            "$type": "[SST:References.VariableReference]",
            //            "Identifier": ""
            //        },
            //        "MethodName": "CSharp.MethodName:static [System.Void, mscorlib, 4.0.0.0] [AnalysisTests.C2, AnalysisTests].DoSth([AnalysisTests.C1, AnalysisTests] a, [System.String, mscorlib, 4.0.0.0] arg)",
            //        "Parameters": [
            //            {
            //                "$type": "[SST:Expressions.Simple.ConstantValueExpression]"
            //            }
            //        ]
            //    }
            //},

            // Should add instance "a" as first parameter.
            Assert.Fail();
        }

        [Test]
        public void CompletingNewMember()
        {
            CompleteInClass(@"public str$");

            // This will produce a broken method declaration with name "CSharp.MethodName:[?] [?].???()".
            // It seems impossible to represent a trigger point like this in a meaningful way.

            Assert.Fail();
        }

        [Test]
        public void LostTriggerPoint()
        {
            CompleteInClass(@"
                public C Method()
                {
                    this.Method().$

                    Console.WriteLine(""asdf"");
                    return this;
                }");

            // Trigger point is lost in this case. The static method call is corrupted.
            // This only happens if no prefix is used. Results are as expected when completing with a prefix.

            //C1 $0;
            //$0 = this.Method();
            //.???("...");
            //return this;

            //{
            //    "$type": "[SST:Statements.ExpressionStatement]",
            //    "Expression": {
            //        "$type": "[SST:Expressions.Assignable.InvocationExpression]",
            //        "Reference": {
            //            "$type": "[SST:References.VariableReference]",
            //            "Identifier": ""
            //        },
            //        "MethodName": "CSharp.MethodName:[?] [?].???()",
            //        "Parameters": [
            //            {
            //                "$type": "[SST:Expressions.Simple.ConstantValueExpression]"
            //            }
            //        ]
            //    }
            //},

            Assert.Fail();
        }

        [Test]
        public void FieldMistakenForVariable()
        {
            // Same thing happens with properties etc. Does not happen when using explicit this.
            CompleteInClass(@" 
                public string Str;

                public void M()
                {
                    Str.$
                }");


            //{
            //    "$type": "[SST:Statements.ExpressionStatement]",
            //    "Expression": {
            //        "$type": "[SST:Expressions.Assignable.CompletionExpression]",
            //        "VariableReference": {
            //            "$type": "[SST:References.VariableReference]",
            //            "Identifier": "Str"
            //        },
            //        "Token": ""
            //    }
            //}

            AssertBody(
                "M",
                VarDecl("$0", Fix.String),
                Assign("$0", RefExpr(FieldRef(Fix.Field(Fix.String, Type("C"), "Str"), VarRef("this")))),
                ExprStmt(Fix.CompletionOnVar(VarRef("$0"), "")));
        }

        [Test]
        public void TriggerInside_WithToken()
        {
            // Exception => ReferenceExpression:e
            // Affected Node => ReferenceExpression:e
            // Case => Undefined
            CompleteInMethod(@"throw e$");

            AssertBody(
                VarDecl("$0", Fix.Exception),
                Assign("$0", new CompletionExpression {Token = "e"}),
                new ThrowStatement {Reference = VarRef("$0")});
        }

        [Test]
        public void TriggerInside_WithNewObject()
        {
            // Exception => IObjectCreationExpression 
            // Affected Node => ReferenceName:Ex
            // Case => Undefined
            CompleteInMethod(@"throw new Ex$");

            AssertBody(
                VarDecl("$0", Fix.Exception),
                Assign("$0", new CompletionExpression()),
                // not sure how this completion would look
                new ThrowStatement {Reference = VarRef("$0")});
        }
    }
}