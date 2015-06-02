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
using System.Linq;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.VsFeedbackGenerator.Analysis.CompletionTarget;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;
using RS = JetBrains.ReSharper.Psi.CSharp.Tree;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal abstract class BaseSSTAnalysisTest : BaseCSharpCodeCompletionTest
    {
        internal readonly IList<string> Log = new List<string>();

        [SetUp]
        public void RegisterLogger()
        {
            var logger = new Commons.TestUtils.Utils.Exceptions.TestLogger(false);
            logger.InfoLogged += Log.Add;
            Registry.RegisterComponent<ILogger>(logger);
        }

        protected SST NewSST()
        {
            return new SST
            {
                EnclosingType = TypeName.Get("N.C, TestProject")
            };
        }

        protected MethodDeclaration NewMethodDeclaration(ITypeName returnType, string simpleName)
        {
            return NewMethodDeclaration(returnType, simpleName, new string[0]);
        }

        protected MethodDeclaration NewMethodDeclaration(ITypeName returnType, string simpleName, params string[] args)
        {
            const string package = "N.C, TestProject";
            var identifier = string.Format(
                "[{0}] [{1}].{2}({3})",
                returnType,
                package,
                simpleName,
                string.Join(", ", args));
            return new MethodDeclaration
            {
                Name = MethodName.Get(identifier),
                IsEntryPoint = true
            };
        }

        protected void AssertResult(SST expected)
        {
            Assert.AreEqual(expected, ResultSST);
        }

        protected void AssertMethod(IMethodDeclaration expected)
        {
            if (Enumerable.Contains(ResultSST.Methods, expected))
            {
                return;
            }
            Assert.Fail("method not found");
        }


        protected IEnumerable<ISimpleExpression> RefExprs(params string[] ids)
        {
            return Lists.NewListFrom(ids.Select(RefExpr));
        }

        protected ISimpleExpression RefExpr(string id)
        {
            return new ReferenceExpression {Reference = new VariableReference {Identifier = id}};
        }

        protected IVariableReference Ref(string id)
        {
            return new VariableReference {Identifier = id};
        }

        protected void AssertAllMethods(params IMethodDeclaration[] expectedDecls)
        {
            var ms = ResultSST.Methods;
            Assert.AreEqual(expectedDecls.Length, ms.Count);

            foreach (var expectedDecl in expectedDecls)
            {
                Assert.IsTrue(ms.Contains(expectedDecl));
            }
        }

        protected void AssertBody(IList<IStatement> body)
        {
            Assert.AreEqual(1, ResultSST.Methods.Count);
            var m = ResultSST.Methods.First();
            Assert.AreEqual(body, m.Body);
        }


        protected void AssertBody(params IStatement[] bodyArr)
        {
            AssertBody(Lists.NewListFrom(bodyArr));
        }


        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
            var logCopy = new List<string>(Log);
            Log.Clear();
            Assert.IsEmpty(logCopy);
            TestAnalysisTrigger.IsPrintingType = false;
        }

        protected static VariableReference VarRef(string id)
        {
            return new VariableReference {Identifier = id};
        }

        protected IVariableDeclaration VarDecl(string varName, ITypeName type)
        {
            return new VariableDeclaration
            {
                Reference = VarRef(varName),
                Type = type
            };
        }

        protected static void AssertNodeIsMethodDeclaration(string simpleMethodName, RS.ICSharpTreeNode node)
        {
            var decl = node as RS.IMethodDeclaration;
            Assert.NotNull(decl);
            Assert.AreEqual(simpleMethodName, decl.NameIdentifier.Name);
        }

        protected static void AssertNodeIsVariableDeclaration(string varName, RS.ICSharpTreeNode node)
        {
            var decl = node as RS.ILocalVariableDeclaration;
            Assert.NotNull(decl);
            Assert.AreEqual(varName, decl.NameIdentifier.Name);
        }

        protected static void AssertNodeIsReference(string refName, RS.ICSharpTreeNode node)
        {
            var expr = node as RS.IReferenceExpression;
            Assert.NotNull(expr);
            Assert.AreEqual(refName, expr.NameIdentifier.Name);
        }

        protected static void AssertNodeIsAssignment(string varName, RS.ICSharpTreeNode node)
        {
            var ass = node as RS.IAssignmentExpression;
            Assert.NotNull(ass);
            var dest = ass.Dest as RS.IReferenceExpression;
            Assert.NotNull(dest);
            Assert.AreEqual(varName, dest.NameIdentifier.Name);
        }

        protected void AssertCompletionCase(CompletionCase expectedCase)
        {
            Assert.AreEqual(expectedCase, LastCompletionMarker.Case);
        }

        protected void AssertNodeIsIf(RS.ICSharpTreeNode node)
        {
            Assert.True(node is RS.IIfStatement);
        }

        protected void AssertNodeIsCall(string expectedName, RS.ICSharpTreeNode node)
        {
            var call = node as RS.IInvocationExpression;
            Assert.NotNull(call);
            var actualName = call.InvocationExpressionReference.GetName();
            Assert.AreEqual(expectedName, actualName);
        }

        protected void AssertCompletionMarker<TNodeType>(CompletionCase expectedCase)
        {
            Assert.That(LastCompletionMarker.AffectedNode is TNodeType);
            Assert.AreEqual(expectedCase, LastCompletionMarker.Case);
        }

        protected static Assignment VarAssign(string varName, IAssignableExpression expr)
        {
            return new Assignment {Reference = VarRef(varName), Expression = expr};
        }
    }
}