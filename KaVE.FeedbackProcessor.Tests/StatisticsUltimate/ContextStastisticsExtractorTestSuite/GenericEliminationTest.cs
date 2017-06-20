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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate.ContextStastisticsExtractorTestSuite
{
    internal class GenericEliminationTest : ContextStatisticsExtractorTestBase
    {
        [Test]
        public void MethodCalls()
        {
            var b1 = CreateContextWithSSTAndMethodBody(
                InvStmt("[T`1[[T -> p:double]],P] [D`1[[T -> p:double]],A,1.2.3.4].M()"));
            var b2 = CreateContextWithSSTAndMethodBody(
                InvStmt("[T`1[[T -> p:float]],P] [D`1[[T -> p:float]],A,1.2.3.4].M()"));

            var actual = Extract(b1, b2);

            Assert.AreEqual(2, actual.NumAsmCalls);
            Assert.AreEqual(
                Sets.NewHashSet(Names.Method("[T`1[[T]],P] [D`1[[T]],A,1.2.3.4].M()")),
                actual.UniqueAsmMethods);
            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), actual.UniqueAssemblies);
        }

        [Test]
        public void MethodRef()
        {
            var b1 = CreateContextWithSSTAndMethodBody(
                RefStmt(
                    new MethodReference
                    {
                        MethodName = Names.Method("[T`1[[T -> p:double]],P] [D`1[[T -> p:double]],A,1.2.3.4].M()")
                    }));
            var b2 = CreateContextWithSSTAndMethodBody(
                RefStmt(
                    new MethodReference
                    {
                        MethodName = Names.Method("[T`1[[T -> p:float]],P] [D`1[[T -> p:float]],A,1.2.3.4].M()")
                    }
                ));

            var actual = Extract(b1, b2);

            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(
                Sets.NewHashSet(Names.Method("[T`1[[T]],P] [D`1[[T]],A,1.2.3.4].M()")),
                actual.UniqueAsmMethods);
            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), actual.UniqueAssemblies);
        }

        [Test]
        public void FieldRef()
        {
            var b1 = CreateContextWithSSTAndMethodBody(
                RefStmt(
                    new FieldReference
                    {
                        FieldName = Names.Field("[T`1[[T -> p:double]],P] [D`1[[T -> p:double]],A,1.2.3.4]._f")
                    }));
            var b2 = CreateContextWithSSTAndMethodBody(
                RefStmt(
                    new FieldReference
                    {
                        FieldName = Names.Field("[T`1[[T -> p:float]],P] [D`1[[T -> p:float]],A,1.2.3.4]._f")
                    }
                ));

            var actual = Extract(b1, b2);

            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(
                Sets.NewHashSet(Names.Field("[T`1[[T]],P] [D`1[[T]],A,1.2.3.4]._f")),
                actual.UniqueAsmFields);
            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), actual.UniqueAssemblies);
        }

        [Test]
        public void PropRef()
        {
            var b1 = CreateContextWithSSTAndMethodBody(
                RefStmt(
                    new PropertyReference
                    {
                        PropertyName = Names.Property(
                            "get [T`1[[T -> p:double]],P] [D`1[[T -> p:double]],A,1.2.3.4].P()")
                    }));
            var b2 = CreateContextWithSSTAndMethodBody(
                RefStmt(
                    new PropertyReference
                    {
                        PropertyName = Names.Property("get [T`1[[T -> p:float]],P] [D`1[[T -> p:float]],A,1.2.3.4].P()")
                    }
                ));

            var actual = Extract(b1, b2);

            Assert.AreEqual(0, actual.NumAsmCalls);
            Assert.AreEqual(
                Sets.NewHashSet(Names.Property("get [T`1[[T]],P] [D`1[[T]],A,1.2.3.4].P()")),
                actual.UniqueAsmProperties);
            Assert.AreEqual(Sets.NewHashSet(Names.Assembly("A,1.2.3.4")), actual.UniqueAssemblies);
        }

        [Test]
        public void ContextInfo()
        {
            const string elem1 = "[G`1[[B -> p:int]],P] [T,P].M()";
            const string elem2 = "[G`1[[B]],P] [T,P].M()";

            var actual = Extract(
                CreateContextWithMethodDeclarationAndHierarchy(elem1, "[p:void] [S,A,1.2.3.4].M()", null));

            Assert.AreEqual(Sets.NewHashSet(Names.Method(elem2)), actual.UniqueMethodDeclsOverrideOrImplementAsm);
        }

        private static IStatement RefStmt(IReference reference)
        {
            return Stmt(new ReferenceExpression {Reference = reference});
        }
    }
}