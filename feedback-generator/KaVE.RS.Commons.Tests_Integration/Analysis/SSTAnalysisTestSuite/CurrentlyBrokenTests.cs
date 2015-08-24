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

using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    internal class CurrentlyBrokenTests : BaseSSTAnalysisTest
    {
        [Test, Ignore]
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
                        TypeReference = SSTAnalysisFixture.Object,
                        Token = ""
                    }));
        }

        [Test, Ignore]
        public void ChainedMethodCalls()
        {
            // Result is the same with explicit this before call to SomeString
            CompleteInClass(@"
                public void M()
                {
                    SomeString().Normalize().$
                }                

                public string SomeString()
                {
                    return ""str"";
                }");

            //Void M()
            //{
            //    String $0;
            //    $0 = %% NULL REFERENCE %%.Normalize();
            //    $0.$;
            //}

            AssertBody(
                VarDecl("$0", Fix.String),
                VarAssign("$0", Invoke("this", Fix.Method(Fix.String, Type("C"), "SomeString"))),
                VarDecl("$1", Fix.String),
                VarAssign("$1", Invoke("$0", Fix.Method(Fix.String, Fix.String, "Normalize"))),
                ExprStmt(new CompletionExpression {VariableReference = VarRef("$1"), Token = ""}));
        }

        [Test, Ignore]
        public void MultipleConditionsInIfBlock()
        {
            CompleteInMethod(@"
                if (true && true)
                {
                    $
                }");

            //if (???) <-- UnknownExpression
            //{
            //    $;
            //}

            // TODO: define expected body
        }

        [Test, Ignore]
        public void SettingField()
        {
            CompleteInClass(@"        
                public bool b;

                public void M()
                {
                    this.b = true;
                    $
                }");

            //"Body": [
            //            {
            //                "$type": "[SST:Statements.Assignment]",
            //                "Reference": {
            //                    "$type": "[SST:References.FieldReference]",
            //                    "Reference": {
            //                        "$type": "[SST:References.VariableReference]",
            //                        "Identifier": ""
            //                    },
            //                    "FieldName": "CSharp.FieldName:[?] [?].???"
            //                },
            //                "Expression": {
            //                    "$type": "[SST:Expressions.Assignable.CompletionExpression]",
            //                    "Token": ""
            //                }
            //            }
            //        ]

            AssertBody(
                Assign(FieldRef(Fix.Field(Fix.Bool, Type("C"), "b"), VarRef("this")), new ConstantValueExpression { Value = "true" }),
                ExprStmt(new CompletionExpression()));
        }

        [Test, Ignore]
        public void SettingFieldWithoutExplicitThis()
        {
            CompleteInClass(@"
                public bool b;

                public void M()
                {
                    b = true;
                    $
                }");

            //"Body": [
            //           {
            //               "$type": "[SST:Statements.Assignment]",
            //               "Reference": {
            //                   "$type": "[SST:References.VariableReference]",
            //                   "Identifier": "b"
            //               },
            //               "Expression": {
            //                   "$type": "[SST:Expressions.Assignable.CompletionExpression]",
            //                   "Token": ""
            //               }
            //           }
            //       ]

            // Same as above. Implicit this should be added back to SST during analysis since its not possible
            // to have a FieldReference without a reference to its declaring type.
            AssertBody(
                Assign(FieldRef(Fix.Field(Fix.Bool, Type("C"), "b"), VarRef("this")), new ConstantValueExpression { Value = "true" }),
                ExprStmt(new CompletionExpression()));
        }
    }
}