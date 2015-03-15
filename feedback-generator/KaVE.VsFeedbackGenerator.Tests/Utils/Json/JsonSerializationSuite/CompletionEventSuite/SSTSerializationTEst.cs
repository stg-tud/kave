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

using System.Collections.Generic;
using KaVE.Model.Collections;
using KaVE.Model.Names.CSharp;
using KaVE.Model.Names.CSharp.MemberNames;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Expressions.LoopHeader;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Blocks;
using KaVE.Model.SSTs.Impl.Declarations;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    public class SSTSerializationTest
    {
        [Test]
        public void VerifyToJson()
        {
            var actual = GetExample().ToCompactJson();
            var expected = GetExampleJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromJson()
        {
            var actual = GetExampleJson().ParseJsonTo<ISST>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyObjToObjEquality()
        {
            var actual = GetExample().ToCompactJson().ParseJsonTo<ISST>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        private static ISST GetExample()
        {
            return new SST
            {
                EnclosingType = TypeName.Get("T,P"),
                Delegates =
                {
                    new DelegateDeclaration
                    {
                        Name = TypeName.Get("T2,P")
                    }
                },
                Events =
                {
                    new EventDeclaration
                    {
                        Name = EventName.Get("[T2,P] [T3,P].E")
                    }
                },
                Fields =
                {
                    new FieldDeclaration
                    {
                        Name = FieldName.Get("[T4,P] [T5,P].F")
                    }
                },
                Methods =
                {
                    new MethodDeclaration
                    {
                        Name = MethodName.Get("[T6,P] [T7,P].M1()"),
                        IsEntryPoint = false,
                        Body = CreateBody()
                    },
                    new MethodDeclaration
                    {
                        Name = MethodName.Get("[T8,P] [T9,P].M2()"),
                        IsEntryPoint = true
                    }
                },
                Properties =
                {
                    new PropertyDeclaration
                    {
                        Name = PropertyName.Get("[T10,P] [T11,P].P"),
                        Get =
                        {
                            new ReturnStatement()
                        },
                        Set =
                        {
                            new Assignment()
                        }
                    }
                }
            };
        }

        private static IList<IStatement> CreateBody()
        {
            // TODO add values to statements to create valid configurations
            var anyVarRef = new VariableReference();
            var anyStmt = new BreakStatement();
            var anyExpr = new ConstantValueExpression();

            return Lists.NewList(
                //
                new DoLoop(),
                new ForEachLoop(),
                new ForLoop(),
                new IfElseBlock(),
                new LockBlock(),
                new SwitchBlock(),
                new TryBlock(),
                new UncheckedBlock(),
                new UnsafeBlock(),
                new UsingBlock(),
                new WhileLoop(),
                //
                new Assignment(),
                new BreakStatement(),
                new ContinueStatement(),
                new ExpressionStatement(),
                new GotoStatement(),
                new LabelledStatement(),
                new ReturnStatement(),
                new ThrowStatement(),
                new UnknownStatement(),
                //
                Nested(new CompletionExpression()),
                Nested(new ComposedExpression()),
                Nested(new IfElseExpression()),
                Nested(new InvocationExpression()),
                Nested(new LambdaExpression()),
                Nested(new LoopHeaderBlockExpression()),
                Nested(new ConstantValueExpression()),
                Nested(new NullExpression()),
                Nested(new ReferenceExpression()),
                Nested(new UnknownExpression()),
                //
                Nested(new EventReference()),
                Nested(new FieldReference()),
                Nested(new MethodReference()),
                Nested(new PropertyReference()),
                Nested(new UnknownReference()),
                Nested(new VariableReference())
                //
                );
        }

        private static IStatement Nested(ILoopHeaderBlockExpression expr)
        {
            return new WhileLoop {Condition = expr};
        }

        private static IStatement Nested(IAssignableExpression expr)
        {
            return new Assignment {Expression = expr};
        }

        private static IStatement Nested(IReference reference)
        {
            return new Assignment {Expression = new ReferenceExpression {Reference = reference}};
        }

        private static string GetExampleJson()
        {
            // do not change! keep for checking exception free reading of old formats!
            return
                "{\"$type\":\"KaVE.Model.SSTs.Impl.SST, KaVE.Model\",\"EnclosingType\":\"CSharp.TypeName:T,P\",\"Fields\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.FieldDeclaration, KaVE.Model\",\"Name\":\"CSharp.MemberNames.FieldName:[T4,P] [T5,P].F\"}],\"Properties\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.PropertyDeclaration, KaVE.Model\",\"Name\":\"CSharp.MemberNames.PropertyName:[T10,P] [T11,P].P\",\"Get\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ReturnStatement, KaVE.Model\",\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}}],\"Set\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}}]}],\"Methods\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.MethodDeclaration, KaVE.Model\",\"Name\":\"CSharp.MemberNames.MethodName:[T6,P] [T7,P].M1()\",\"IsEntryPoint\":false,\"Body\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.DoLoop, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.ForEachLoop, KaVE.Model\",\"Declaration\":{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.VariableDeclaration, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"Type\":\"CSharp.TypeNames.UnknownTypeName:?\",\"IsMissing\":true},\"LoopedReference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.ForLoop, KaVE.Model\",\"Init\":[],\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"Step\":[],\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.IfElseBlock, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"Then\":[],\"Else\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.LockBlock, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.SwitchBlock, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"Sections\":[],\"DefaultSection\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.TryBlock, KaVE.Model\",\"Body\":[],\"CatchBlocks\":[],\"Finally\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.UncheckedBlock, KaVE.Model\",\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.UnsafeBlock, KaVE.Model\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.UsingBlock, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.WhileLoop, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.BreakStatement, KaVE.Model\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ContinueStatement, KaVE.Model\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ExpressionStatement, KaVE.Model\",\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.GotoStatement, KaVE.Model\",\"Label\":\"\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.LabelledStatement, KaVE.Model\",\"Label\":\"\",\"Statement\":{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.UnknownStatement, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ReturnStatement, KaVE.Model\",\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ThrowStatement, KaVE.Model\",\"Exception\":\"CSharp.TypeNames.UnknownTypeName:?\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.UnknownStatement, KaVE.Model\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.CompletionExpression, KaVE.Model\",\"Token\":\"\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.ComposedExpression, KaVE.Model\",\"References\":[]}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.IfElseExpression, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"ThenExpression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"ElseExpression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.InvocationExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"MethodName\":\"CSharp.MemberNames.MethodName:[?] [?].???()\",\"Parameters\":[]}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.LambdaExpression, KaVE.Model\",\"Parameters\":[],\"Body\":[]}},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.WhileLoop, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.LoopHeader.LoopHeaderBlockExpression, KaVE.Model\",\"Body\":[]},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ConstantValueExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.NullExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.EventReference, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"EventName\":\"CSharp.MemberNames.EventName:[?] [?].???\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.FieldReference, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"FieldName\":\"CSharp.MemberNames.FieldName:[?] [?].???\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.MethodReference, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"MethodName\":\"CSharp.MemberNames.MethodName:[?] [?].???()\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.PropertyReference, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true},\"PropertyName\":\"CSharp.MemberNames.PropertyName:[?] [?].???\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\",\"IsMissing\":true}}}]},{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.MethodDeclaration, KaVE.Model\",\"Name\":\"CSharp.MemberNames.MethodName:[T8,P] [T9,P].M2()\",\"IsEntryPoint\":true,\"Body\":[]}],\"Events\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.EventDeclaration, KaVE.Model\",\"Name\":\"CSharp.MemberNames.EventName:[T2,P] [T3,P].E\"}],\"Delegates\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.DelegateDeclaration, KaVE.Model\",\"Name\":\"CSharp.TypeName:T2,P\"}]}";
        }
    }
}