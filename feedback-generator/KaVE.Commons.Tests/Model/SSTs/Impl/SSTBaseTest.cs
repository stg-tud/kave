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

using System;
using System.Linq;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Model.SSTs.Visitor;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl
{
    internal abstract class SSTBaseTest
    {
        protected static void AssertChildren(ISSTNode sut, params ISSTNode[] expecteds)
        {
            var actualsEnum = sut.Children.GetEnumerator();
            var expectedsEnum = expecteds.GetEnumerator();
            while (expectedsEnum.MoveNext())
            {
                Assert.True(actualsEnum.MoveNext());
                // ReSharper disable once PossibleUnintendedReferenceComparison
                Assert.True(expectedsEnum.Current == actualsEnum.Current);
            }
            Assert.False(actualsEnum.MoveNext());
        }

        protected ISimpleExpression Label(string label)
        {
            return new ConstantValueExpression {Value = label};
        }

        protected IVariableDeclaration SomeDeclaration(string type = "T1")
        {
            return new VariableDeclaration {Type = Names.Type(type + ",P1")};
        }

        protected IVariableReference SomeVarRef(string id = "v")
        {
            return new VariableReference {Identifier = id};
        }

        protected IKaVEList<IVariableReference> Refs(params string[] strRefs)
        {
            var refs = strRefs.ToList().Select(SomeVarRef);
            return Lists.NewListFrom(refs);
        }

        protected IMethodName GetMethod(string simpleName)
        {
            var methodName = String.Format("[T1, P1] [T2, P2].{0}()", simpleName);
            return Names.Method(methodName);
        }

        protected IKaVEList<ISimpleExpression> RefExprs(params string[] ids)
        {
            var exprs = ids.Select<string, ISimpleExpression>(
                id => new ReferenceExpression {Reference = new VariableReference {Identifier = id}});
            return Lists.NewListFrom(exprs);
        }

        protected IParameterName SomeParameter()
        {
            return Names.Parameter("[T,P] n");
        }

        protected ILambdaName SomeLambdaName()
        {
            return Names.Lambda("[T,P] ([T2,P2] p)");
        }
    }
}