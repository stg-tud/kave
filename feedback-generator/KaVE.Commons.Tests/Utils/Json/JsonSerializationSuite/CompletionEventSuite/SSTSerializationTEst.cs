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
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    public class SSTSerializationTest
    {
        [Test]
        public void VerifyObjToObjEquality()
        {
            var actual = GetExample().ToCompactJson().ParseJsonTo<ISST>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromCurrentJson()
        {
            var actual = GetExampleJson_Current().ParseJsonTo<ISST>();
            var expected = GetExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyToJson()
        {
            var actual = GetExample().ToCompactJson();
            var expected = GetExampleJson_Current();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JsonDoesNotContainFullNamespace()
        {
            var actual = GetExample().ToCompactJson();
            Assert.False(actual.Contains("KaVE.Commons.Model.SSTs.Impl"));
        }

        [Test]
        public void JsonDoesNotContainAssembly()
        {
            var actual = GetExample().ToCompactJson();
            Assert.False(actual.Contains(".SST, KaVE.Commons"));
        }

        [Test]
        public void VerifyFromJson_Legacy_BeforeRestructuringProjects()
        {
            var actual = GetExampleJson_Legacy_BeforeRestructuringProjects().ParseJsonTo<ISST>();
            var expected = GetLegacyExample();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VerifyFromJson_Legacy_BeforeExtendingCatchBlocksAndReturnsAndCustomSerialization()
        {
            var actual =
                GetExampleJson_Legacy_BeforeExtendingCatchBlocksAndReturnsAndCustomSerialization().ParseJsonTo<ISST>();
            var expected = GetLegacyExample();
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
                        Name = DelegateTypeName.Get("d:[R,P] [T2,P].()")
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
                        Body = CreateBody(true)
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

        private static ISST GetLegacyExample()
        {
            return new SST
            {
                EnclosingType = TypeName.Get("T,P"),
                Delegates =
                {
                    new DelegateDeclaration
                    {
                        Name = DelegateTypeName.Get("d:T2,P")
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
                        Body = CreateBody(false)
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

        private static IKaVEList<IStatement> CreateBody(bool shouldAddDetails)
        {
            // TODO add values to statements to create valid configurations
            //var anyVarRef = new VariableReference();
            //var anyStmt = new BreakStatement();
            //var anyExpr = new ConstantValueExpression();

            return Lists.NewList(
                //
                new DoLoop(),
                new ForEachLoop(),
                new ForLoop(),
                new IfElseBlock(),
                new LockBlock(),
                new SwitchBlock(),
                shouldAddDetails ? CreateComplexTryBlock() : new TryBlock(),
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

        private static IStatement CreateComplexTryBlock()
        {
            return new TryBlock
            {
                Body = {new ContinueStatement()},
                Finally = {new BreakStatement()},
                CatchBlocks =
                {
                    new CatchBlock
                    {
                        Parameter = ParameterName.Get("[T,P] p"),
                        Body =
                        {
                            new ContinueStatement()
                        },
                        IsGeneral = true,
                        IsUnnamed = true
                    }
                }
            };
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

        private static string GetExampleJson_Current()
        {
            // do not change! keep for checking exception free reading of old formats!
            return
                "{\"$type\":\"[SST:SST]\",\"EnclosingType\":\"CSharp.TypeName:T,P\",\"Fields\":[{\"$type\":\"[SST:Declarations.FieldDeclaration]\",\"Name\":\"CSharp.FieldName:[T4,P] [T5,P].F\"}],\"Properties\":[{\"$type\":\"[SST:Declarations.PropertyDeclaration]\",\"Name\":\"CSharp.PropertyName:[T10,P] [T11,P].P\",\"Get\":[{\"$type\":\"[SST:Statements.ReturnStatement]\",\"Expression\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"},\"IsVoid\":false}],\"Set\":[{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"}}]}],\"Methods\":[{\"$type\":\"[SST:Declarations.MethodDeclaration]\",\"Name\":\"CSharp.MethodName:[T6,P] [T7,P].M1()\",\"IsEntryPoint\":false,\"Body\":[{\"$type\":\"[SST:Blocks.DoLoop]\",\"Condition\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"},\"Body\":[]},{\"$type\":\"[SST:Blocks.ForEachLoop]\",\"Declaration\":{\"$type\":\"[SST:Statements.VariableDeclaration]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"Type\":\"CSharp.UnknownTypeName:?\"},\"LoopedReference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"[SST:Blocks.ForLoop]\",\"Init\":[],\"Condition\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"},\"Step\":[],\"Body\":[]},{\"$type\":\"[SST:Blocks.IfElseBlock]\",\"Condition\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"},\"Then\":[],\"Else\":[]},{\"$type\":\"[SST:Blocks.LockBlock]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"[SST:Blocks.SwitchBlock]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"Sections\":[],\"DefaultSection\":[]},{\"$type\":\"[SST:Blocks.TryBlock]\",\"Body\":[{\"$type\":\"[SST:Statements.ContinueStatement]\"}],\"CatchBlocks\":[{\"$type\":\"[SST:Blocks.CatchBlock]\",\"Parameter\":\"CSharp.ParameterName:[T,P] p\",\"Body\":[{\"$type\":\"[SST:Statements.ContinueStatement]\"}],\"IsGeneral\":true,\"IsUnnamed\":true}],\"Finally\":[{\"$type\":\"[SST:Statements.BreakStatement]\"}]},{\"$type\":\"[SST:Blocks.UncheckedBlock]\",\"Body\":[]},{\"$type\":\"[SST:Blocks.UnsafeBlock]\"},{\"$type\":\"[SST:Blocks.UsingBlock]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"[SST:Blocks.WhileLoop]\",\"Condition\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"},\"Body\":[]},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"}},{\"$type\":\"[SST:Statements.BreakStatement]\"},{\"$type\":\"[SST:Statements.ContinueStatement]\"},{\"$type\":\"[SST:Statements.ExpressionStatement]\",\"Expression\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"}},{\"$type\":\"[SST:Statements.GotoStatement]\",\"Label\":\"\"},{\"$type\":\"[SST:Statements.LabelledStatement]\",\"Label\":\"\",\"Statement\":{\"$type\":\"[SST:Statements.UnknownStatement]\"}},{\"$type\":\"[SST:Statements.ReturnStatement]\",\"Expression\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"},\"IsVoid\":false},{\"$type\":\"[SST:Statements.ThrowStatement]\",\"Exception\":\"CSharp.UnknownTypeName:?\"},{\"$type\":\"[SST:Statements.UnknownStatement]\"},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Assignable.CompletionExpression]\",\"Token\":\"\"}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Assignable.ComposedExpression]\",\"References\":[]}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Assignable.IfElseExpression]\",\"Condition\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"},\"ThenExpression\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"},\"ElseExpression\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"}}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Assignable.InvocationExpression]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"MethodName\":\"CSharp.MethodName:[?] [?].???()\",\"Parameters\":[]}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Assignable.LambdaExpression]\",\"Name\":\"CSharp.LambdaName:???\",\"Body\":[]}},{\"$type\":\"[SST:Blocks.WhileLoop]\",\"Condition\":{\"$type\":\"[SST:Expressions.LoopHeader.LoopHeaderBlockExpression]\",\"Body\":[]},\"Body\":[]},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.ConstantValueExpression]\"}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.NullExpression]\"}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.ReferenceExpression]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"}}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.UnknownExpression]\"}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.ReferenceExpression]\",\"Reference\":{\"$type\":\"[SST:References.EventReference]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"EventName\":\"CSharp.EventName:[?] [?].???\"}}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.ReferenceExpression]\",\"Reference\":{\"$type\":\"[SST:References.FieldReference]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"FieldName\":\"CSharp.FieldName:[?] [?].???\"}}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.ReferenceExpression]\",\"Reference\":{\"$type\":\"[SST:References.MethodReference]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"MethodName\":\"CSharp.MethodName:[?] [?].???()\"}}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.ReferenceExpression]\",\"Reference\":{\"$type\":\"[SST:References.PropertyReference]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"},\"PropertyName\":\"CSharp.PropertyName:[?] [?].???\"}}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.ReferenceExpression]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"}}},{\"$type\":\"[SST:Statements.Assignment]\",\"Reference\":{\"$type\":\"[SST:References.UnknownReference]\"},\"Expression\":{\"$type\":\"[SST:Expressions.Simple.ReferenceExpression]\",\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"}}}]},{\"$type\":\"[SST:Declarations.MethodDeclaration]\",\"Name\":\"CSharp.MethodName:[T8,P] [T9,P].M2()\",\"IsEntryPoint\":true,\"Body\":[]}],\"Events\":[{\"$type\":\"[SST:Declarations.EventDeclaration]\",\"Name\":\"CSharp.EventName:[T2,P] [T3,P].E\"}],\"Delegates\":[{\"$type\":\"[SST:Declarations.DelegateDeclaration]\",\"Name\":\"CSharp.DelegateTypeName:d:[R,P] [T2,P].()\"}]}";
        }

        private static string GetExampleJson_Legacy_BeforeRestructuringProjects()
        {
            // do not change! keep for checking exception free reading of old formats!
            return
                "{\"$type\":\"KaVE.Model.SSTs.Impl.SST, KaVE.Model\",\"EnclosingType\":\"CSharp.TypeName:T,P\",\"Fields\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.FieldDeclaration, KaVE.Model\",\"Name\":\"CSharp.FieldName:[T4,P] [T5,P].F\"}],\"Properties\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.PropertyDeclaration, KaVE.Model\",\"Name\":\"CSharp.PropertyName:[T10,P] [T11,P].P\",\"Get\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ReturnStatement, KaVE.Model\",\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}}],\"Set\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}}]}],\"Methods\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.MethodDeclaration, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T6,P] [T7,P].M1()\",\"IsEntryPoint\":false,\"Body\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.DoLoop, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.ForEachLoop, KaVE.Model\",\"Declaration\":{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.VariableDeclaration, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"Type\":\"CSharp.UnknownTypeName:?\"},\"LoopedReference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.ForLoop, KaVE.Model\",\"Init\":[],\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"Step\":[],\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.IfElseBlock, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"Then\":[],\"Else\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.LockBlock, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.SwitchBlock, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"Sections\":[],\"DefaultSection\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.TryBlock, KaVE.Model\",\"Body\":[],\"CatchBlocks\":[],\"Finally\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.UncheckedBlock, KaVE.Model\",\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.UnsafeBlock, KaVE.Model\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.UsingBlock, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.WhileLoop, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.BreakStatement, KaVE.Model\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ContinueStatement, KaVE.Model\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ExpressionStatement, KaVE.Model\",\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.GotoStatement, KaVE.Model\",\"Label\":\"\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.LabelledStatement, KaVE.Model\",\"Label\":\"\",\"Statement\":{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.UnknownStatement, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ReturnStatement, KaVE.Model\",\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.ThrowStatement, KaVE.Model\",\"Exception\":\"CSharp.UnknownTypeName:?\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.UnknownStatement, KaVE.Model\"},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.CompletionExpression, KaVE.Model\",\"Token\":\"\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.ComposedExpression, KaVE.Model\",\"References\":[]}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.IfElseExpression, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"ThenExpression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"},\"ElseExpression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.InvocationExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"MethodName\":\"CSharp.MethodName:[?] [?].???()\",\"Parameters\":[]}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Assignable.LambdaExpression, KaVE.Model\",\"Parameters\":[],\"Body\":[]}},{\"$type\":\"KaVE.Model.SSTs.Impl.Blocks.WhileLoop, KaVE.Model\",\"Condition\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.LoopHeader.LoopHeaderBlockExpression, KaVE.Model\",\"Body\":[]},\"Body\":[]},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ConstantValueExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.NullExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Model\"}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.EventReference, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"EventName\":\"CSharp.EventName:[?] [?].???\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.FieldReference, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"FieldName\":\"CSharp.FieldName:[?] [?].???\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.MethodReference, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"MethodName\":\"CSharp.MethodName:[?] [?].???()\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.PropertyReference, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"},\"PropertyName\":\"CSharp.PropertyName:[?] [?].???\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"}}},{\"$type\":\"KaVE.Model.SSTs.Impl.Statements.Assignment, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.UnknownReference, KaVE.Model\"},\"Expression\":{\"$type\":\"KaVE.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Model\",\"Reference\":{\"$type\":\"KaVE.Model.SSTs.Impl.References.VariableReference, KaVE.Model\",\"Identifier\":\"\"}}}]},{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.MethodDeclaration, KaVE.Model\",\"Name\":\"CSharp.MethodName:[T8,P] [T9,P].M2()\",\"IsEntryPoint\":true,\"Body\":[]}],\"Events\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.EventDeclaration, KaVE.Model\",\"Name\":\"CSharp.EventName:[T2,P] [T3,P].E\"}],\"Delegates\":[{\"$type\":\"KaVE.Model.SSTs.Impl.Declarations.DelegateDeclaration, KaVE.Model\",\"Name\":\"CSharp.TypeName:d:T2,P\"}]}";
        }

        private static string GetExampleJson_Legacy_BeforeExtendingCatchBlocksAndReturnsAndCustomSerialization()
        {
            // do not change! keep for checking exception free reading of old formats!
            return
                "{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.SST, KaVE.Commons\",\"EnclosingType\":\"CSharp.TypeName:T,P\",\"Fields\":[{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Declarations.FieldDeclaration, KaVE.Commons\",\"Name\":\"CSharp.FieldName:[T4,P] [T5,P].F\"}],\"Properties\":[{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Declarations.PropertyDeclaration, KaVE.Commons\",\"Name\":\"CSharp.PropertyName:[T10,P] [T11,P].P\",\"Get\":[{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.ReturnStatement, KaVE.Commons\",\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"}}],\"Set\":[{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"}}]}],\"Methods\":[{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Declarations.MethodDeclaration, KaVE.Commons\",\"Name\":\"CSharp.MethodName:[T6,P] [T7,P].M1()\",\"IsEntryPoint\":false,\"Body\":[{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.DoLoop, KaVE.Commons\",\"Condition\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"},\"Body\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.ForEachLoop, KaVE.Commons\",\"Declaration\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Declarations.VariableDeclaration, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"Type\":\"CSharp.UnknownTypeName:?\"},\"LoopedReference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.ForLoop, KaVE.Commons\",\"Init\":[],\"Condition\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"},\"Step\":[],\"Body\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.IfElseBlock, KaVE.Commons\",\"Condition\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"},\"Then\":[],\"Else\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.LockBlock, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.SwitchBlock, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"Sections\":[],\"DefaultSection\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.TryBlock, KaVE.Commons\",\"Body\":[],\"CatchBlocks\":[],\"Finally\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.UncheckedBlock, KaVE.Commons\",\"Body\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.UnsafeBlock, KaVE.Commons\"},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.UsingBlock, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"Body\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.WhileLoop, KaVE.Commons\",\"Condition\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"},\"Body\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.BreakStatement, KaVE.Commons\"},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.ContinueStatement, KaVE.Commons\"},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.ExpressionStatement, KaVE.Commons\",\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.GotoStatement, KaVE.Commons\",\"Label\":\"\"},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.LabelledStatement, KaVE.Commons\",\"Label\":\"\",\"Statement\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.UnknownStatement, KaVE.Commons\"}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.ReturnStatement, KaVE.Commons\",\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.ThrowStatement, KaVE.Commons\",\"Exception\":\"CSharp.UnknownTypeName:?\"},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.UnknownStatement, KaVE.Commons\"},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable.CompletionExpression, KaVE.Commons\",\"Token\":\"\"}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable.ComposedExpression, KaVE.Commons\",\"References\":[]}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable.IfElseExpression, KaVE.Commons\",\"Condition\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"},\"ThenExpression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"},\"ElseExpression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"}}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable.InvocationExpression, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"MethodName\":\"CSharp.MethodName:[?] [?].???()\",\"Parameters\":[]}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable.LambdaExpression, KaVE.Commons\",\"Parameters\":[],\"Body\":[]}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Blocks.WhileLoop, KaVE.Commons\",\"Condition\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader.LoopHeaderBlockExpression, KaVE.Commons\",\"Body\":[]},\"Body\":[]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.ConstantValueExpression, KaVE.Commons\"}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.NullExpression, KaVE.Commons\"}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"}}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.UnknownExpression, KaVE.Commons\"}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.EventReference, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"EventName\":\"CSharp.EventName:[?] [?].???\"}}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.FieldReference, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"FieldName\":\"CSharp.FieldName:[?] [?].???\"}}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.MethodReference, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"MethodName\":\"CSharp.MethodName:[?] [?].???()\"}}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.PropertyReference, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"},\"PropertyName\":\"CSharp.PropertyName:[?] [?].???\"}}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"}}},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Statements.Assignment, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.UnknownReference, KaVE.Commons\"},\"Expression\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Expressions.Simple.ReferenceExpression, KaVE.Commons\",\"Reference\":{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.References.VariableReference, KaVE.Commons\",\"Identifier\":\"\"}}}]},{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Declarations.MethodDeclaration, KaVE.Commons\",\"Name\":\"CSharp.MethodName:[T8,P] [T9,P].M2()\",\"IsEntryPoint\":true,\"Body\":[]}],\"Events\":[{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Declarations.EventDeclaration, KaVE.Commons\",\"Name\":\"CSharp.EventName:[T2,P] [T3,P].E\"}],\"Delegates\":[{\"$type\":\"KaVE.Commons.Model.SSTs.Impl.Declarations.DelegateDeclaration, KaVE.Commons\",\"Name\":\"CSharp.TypeName:d:T2,P\"}]}";
        }
    }
}