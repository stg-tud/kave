/*
 * Copyright 2017 Sebastian Proksch
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

using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate.ContextStastisticsExtractorTestSuite
{
    internal class ContextStatisticsExtractorTestBase
    {
        protected ContextStatisticsExtractor Sut;

        [SetUp]
        public void SetUp()
        {
            Sut = new ContextStatisticsExtractor(new ContextFilter(GeneratedCode.Include, Duplication.Include));
        }

        protected IContextStatistics Extract(params Context[] contexts)
        {
            return Sut.Extract(contexts);
        }

        protected static void AssertUniqueAssemblies(IContextStatistics actual, params string[] ids)
        {
            Assert.AreEqual(Sets.NewHashSetFrom(ids.Select(id => Names.Assembly(id))), actual.UniqueAssemblies);
        }

        protected static void AssertUniqueAsmMethods(IContextStatistics actual, params string[] ids)
        {
            Assert.AreEqual(Sets.NewHashSetFrom(ids.Select(id => Names.Method(id))), actual.UniqueAsmMethods);
        }

        protected static IExpressionStatement InvStmt(string methodId)
        {
            return Stmt(
                new InvocationExpression
                {
                    MethodName = Names.Method(methodId)
                }
            );
        }

        protected static IExpressionStatement Stmt(IAssignableExpression expr)
        {
            return new ExpressionStatement
            {
                Expression = expr
            };
        }

        protected IReferenceExpression RefExpr(IReference aref)
        {
            return new ReferenceExpression
            {
                Reference = aref
            };
        }

        protected static Context CreateContextWithSSTAndMethodBody(params IStatement[] stmts)
        {
            return new Context
            {
                SST = new SST
                {
                    EnclosingType = Names.Type("C,P"),
                    Methods =
                    {
                        new MethodDeclaration
                        {
                            Name = Names.Method("[p:void] [T,P].M()"),
                            Body = Lists.NewListFrom(stmts)
                        }
                    }
                }
            };
        }

        protected static Context CreateContextWithMethodDeclarationAndHierarchy(string elemId,
            string superId,
            string firstId)
        {
            var mh = new MethodHierarchy
            {
                Element = Names.Method(elemId)
            };
            if (superId != null)
            {
                mh.Super = Names.Method(superId);
            }
            if (firstId != null)
            {
                mh.First = Names.Method(firstId);
            }
            return new Context
            {
                TypeShape = new TypeShape {MethodHierarchies = {mh}},
                SST = new SST
                {
                    EnclosingType = Names.Type("C,P"),
                    Methods =
                    {
                        new MethodDeclaration {Name = Names.Method(elemId)}
                    }
                }
            };
        }
    }
}