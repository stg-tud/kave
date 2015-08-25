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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using KaVE.RS.Commons.Utils;
using NUnit.Framework;
using JB = JetBrains.ReSharper.Psi.CSharp.Tree;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    internal abstract class BaseSSTAnalysisTest : BaseCSharpCodeCompletionTest
    {
        internal readonly IList<string> Log = new List<string>();

        [SetUp]
        public void RegisterLogger()
        {
            var logger = new KaVE.Commons.TestUtils.Utils.Exceptions.TestLogger(false);
            logger.InfoLogged += Log.Add;
            Registry.RegisterComponent<ILogger>(logger);
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

        protected ISimpleExpression RefExpr(IReference reference)
        {
            return new ReferenceExpression {Reference = reference};
        }

        #region custom asserts

        protected void AssertAllMethods(params IMethodDeclaration[] expectedDecls)
        {
            var ms = ResultSST.Methods;
            Assert.AreEqual(expectedDecls.Length, ms.Count);

            foreach (var expectedDecl in expectedDecls)
            {
                Assert.IsTrue(ms.Contains(expectedDecl));
            }
        }

        protected void AssertBody(IMethodDeclaration method, IKaVEList<IStatement> body)
        {
            if (!body.Equals(method.Body))
            {
                Console.WriteLine("AssertBody failed!");
                Console.WriteLine("\n-- expected body --\n");
                Console.WriteLine(body.ToString());
                Console.WriteLine("\n-- actual body --\n");
                Console.WriteLine(method.Body.ToString());
                Assert.Fail();
            }
        }

        protected void AssertBody(IKaVEList<IStatement> body)
        {
            Assert.AreEqual(1, ResultSST.Methods.Count);
            var m = ResultSST.Methods.First();
            AssertBody(m, body);
        }

        protected void AssertBody(params IStatement[] bodyArr)
        {
            AssertBody(Lists.NewListFrom(bodyArr));
        }

        protected void AssertBody(string methodName, params IStatement[] bodyArr)
        {
            AssertBody(ResultSST.Methods.Single(m => m.Name.Name == methodName), Lists.NewListFrom(bodyArr));
        }

        protected static void AssertNodeIsMethodDeclaration(string simpleMethodName, JB.ICSharpTreeNode node)
        {
            var decl = node as JB.IMethodDeclaration;
            Assert.NotNull(decl);
            Assert.AreEqual(simpleMethodName, decl.NameIdentifier.Name);
        }

        protected static void AssertNodeIsVariableDeclaration(string varName, JB.ICSharpTreeNode node)
        {
            var decl = node as JB.ILocalVariableDeclaration;
            Assert.NotNull(decl);
            Assert.AreEqual(varName, decl.NameIdentifier.Name);
        }

        protected static void AssertNodeIsReference(string refName, JB.ICSharpTreeNode node)
        {
            var expr = node as JB.IReferenceExpression;
            Assert.NotNull(expr);
            Assert.AreEqual(refName, expr.NameIdentifier.Name);
        }

        protected static void AssertNodeIsAssignment(string varName, JB.ICSharpTreeNode node)
        {
            var ass = node as JB.IAssignmentExpression;
            Assert.NotNull(ass);
            var dest = ass.Dest as JB.IReferenceExpression;
            Assert.NotNull(dest);
            Assert.AreEqual(varName, dest.NameIdentifier.Name);
        }

        protected void AssertCompletionCase(CompletionCase expectedCase)
        {
            Assert.AreEqual(expectedCase, LastCompletionMarker.Case);
        }

        protected void AssertNodeIsIf(JB.ICSharpTreeNode node)
        {
            Assert.True(node is JB.IIfStatement);
        }

        protected void AssertNodeIsCall(string expectedName, JB.ICSharpTreeNode node)
        {
            var call = node as JB.IInvocationExpression;
            Assert.NotNull(call);
            var actualName = call.InvocationExpressionReference.GetName();
            Assert.AreEqual(expectedName, actualName);
        }

        protected void AssertCompletionMarker<TNodeType>(CompletionCase expectedCase)
        {
            Assert.That(LastCompletionMarker.AffectedNode is TNodeType);
            Assert.AreEqual(expectedCase, LastCompletionMarker.Case);
        }

        #endregion

        #region instantiation helper

        protected static VariableReference VarRef(string id = "")
        {
            return new VariableReference {Identifier = id};
        }

        protected static PropertyReference PropRef(string name, ITypeName type, string target = "this")
        {
            var propertyName = PropertyName.Get("set get " + MemberName(name, type) + "()");
            return new PropertyReference {Reference = VarRef(target), PropertyName = propertyName};
        }

        protected static FieldReference FieldRef(string name, ITypeName type, string target = "this")
        {
            var fieldName = FieldName.Get(MemberName(name, type));
            return new FieldReference {Reference = VarRef(target), FieldName = fieldName};
        }

        private static string MemberName(string name, ITypeName type)
        {
            return string.Format("[{0}] [N.C, TestProject].{1}", type, name);
        }

        protected static FieldReference FieldRef(IFieldName fieldName, IVariableReference declTypeRef)
        {
            return new FieldReference {FieldName = fieldName, Reference = declTypeRef};
        }

        protected static EventReference EventRef(IEventName eventName, IVariableReference declTypeRef)
        {
            return new EventReference {EventName = eventName, Reference = declTypeRef};
        }

        protected static PropertyReference PropertyRef(IPropertyName propertyName, IVariableReference declTypeRef)
        {
            return new PropertyReference {PropertyName = propertyName, Reference = declTypeRef};
        }

        protected static MethodReference MethodRef(IMethodName methodName, IVariableReference declTypeRef)
        {
            return new MethodReference {MethodName = methodName, Reference = declTypeRef};
        }

        protected IVariableDeclaration VarDecl(string varName, ITypeName type)
        {
            return new VariableDeclaration
            {
                Reference = VarRef(varName),
                Type = type
            };
        }

        protected static Assignment VarAssign(string varName, IAssignableExpression expr)
        {
            return new Assignment {Reference = VarRef(varName), Expression = expr};
        }

        protected static IAssignment Assign(string id, IAssignableExpression expr)
        {
            return new Assignment
            {
                Reference = VarRef(id),
                Expression = expr
            };
        }

        protected static IAssignment Assign(IAssignableReference reference, IAssignableExpression expr)
        {
            return new Assignment {Reference = reference, Expression = expr};
        }

        protected static IStatement InvokeStmt(string id, IMethodName methodName, params ISimpleExpression[] parameters)
        {
            Asserts.That(!methodName.IsStatic);
            return ExprStmt(
                new InvocationExpression
                {
                    Reference = VarRef(id),
                    MethodName = methodName,
                    Parameters = Lists.NewList(parameters)
                });
        }

        protected static IStatement InvokeStaticStmt(IMethodName methodName, params ISimpleExpression[] parameters)
        {
            Asserts.That(methodName.IsStatic);
            return ExprStmt(
                new InvocationExpression
                {
                    MethodName = methodName,
                    Parameters = Lists.NewList(parameters)
                });
        }

        protected static IExpressionStatement ExprStmt(IAssignableExpression expr)
        {
            return new ExpressionStatement
            {
                Expression = expr
            };
        }

        public static InvocationExpression Invoke(string id,
            IMethodName methodName,
            params ISimpleExpression[] parameters)
        {
            Assert.False(methodName.IsStatic);
            return new InvocationExpression
            {
                Reference = VarRef(id),
                MethodName = methodName,
                Parameters = Lists.NewList(parameters)
            };
        }

        public static InvocationExpression InvokeStatic(IMethodName methodName, params ISimpleExpression[] parameters)
        {
            Assert.True(methodName.IsStatic);
            return new InvocationExpression
            {
                MethodName = methodName,
                Parameters = Lists.NewList(parameters)
            };
        }

        protected InvocationExpression InvokeCtor(IMethodName methodName, params ISimpleExpression[] parameters)
        {
            Assert.That(methodName.IsConstructor);
            return new InvocationExpression
            {
                MethodName = methodName,
                Parameters = Lists.NewList(parameters)
            };
        }

        protected static IMethodName Method(string methodDef, params object[] args)
        {
            return MethodName.Get(string.Format(methodDef, args));
        }

        protected static ITypeName Type(string shortName)
        {
            return TypeName.Get("N." + shortName + ", TestProject");
        }

        #endregion
    }
}