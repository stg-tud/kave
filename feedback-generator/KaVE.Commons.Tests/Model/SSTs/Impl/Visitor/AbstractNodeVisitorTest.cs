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

using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Visitor
{
    internal class AbstractNodeVisitorTest
    {
        [Test]
        public void AllVisitsAreImplemented()
        {
            var sut = new AbstractNodeVisitor<object>();
            sut.Visit((ISST) null, null);

            // declarations
            sut.Visit((IDelegateDeclaration) null, null);
            sut.Visit((IEventDeclaration) null, null);
            sut.Visit((IFieldDeclaration) null, null);
            sut.Visit((IMethodDeclaration) null, null);
            sut.Visit((IPropertyDeclaration) null, null);
            sut.Visit((IVariableDeclaration) null, null);

            // statements
            sut.Visit((IAssignment) null, null);
            sut.Visit((IBreakStatement) null, null);
            sut.Visit((IContinueStatement) null, null);
            sut.Visit((IExpressionStatement) null, null);
            sut.Visit((IGotoStatement) null, null);
            sut.Visit((ILabelledStatement) null, null);
            sut.Visit((ILockBlock) null, null);
            sut.Visit((IReturnStatement) null, null);
            sut.Visit((IThrowStatement) null, null);

            // blocks
            sut.Visit((IDoLoop) null, null);
            sut.Visit((IForEachLoop) null, null);
            sut.Visit((IForLoop) null, null);
            sut.Visit((IIfElseBlock) null, null);
            sut.Visit((ISwitchBlock) null, null);
            sut.Visit((ITryBlock) null, null);
            sut.Visit((IUncheckedBlock) null, null);
            sut.Visit((IUnsafeBlock) null, null);
            sut.Visit((IUsingBlock) null, null);
            sut.Visit((IWhileLoop) null, null);

            // Expressions
            sut.Visit((ICompletionExpression) null, null);
            sut.Visit((IComposedExpression) null, null);
            sut.Visit((IIfElseExpression) null, null);
            sut.Visit((IInvocationExpression) null, null);
            sut.Visit((ILambdaExpression) null, null);
            sut.Visit((ILoopHeaderBlockExpression) null, null);
            sut.Visit((IConstantValueExpression) null, null);
            sut.Visit((INullExpression) null, null);
            sut.Visit((IReferenceExpression) null, null);

            // References
            sut.Visit((IEventReference) null, null);
            sut.Visit((IFieldReference) null, null);
            sut.Visit((IMethodReference) null, null);
            sut.Visit((IPropertyReference) null, null);
            sut.Visit((IVariableReference) null, null);

            // Unknowns
            sut.Visit((IUnknownReference) null, null);
            sut.Visit((IUnknownExpression) null, null);
            sut.Visit((IUnknownStatement) null, null);
        }

        [Test]
        public void AllVisitsAreImplementedForVisitorWithReturn()
        {
            var sut = new AbstractNodeVisitor<int, object>();
            Assert.Null(sut.Visit((ISST) null, 0));

            // declarations
            Assert.Null(sut.Visit((IDelegateDeclaration) null, 0));
            Assert.Null(sut.Visit((IEventDeclaration) null, 0));
            Assert.Null(sut.Visit((IFieldDeclaration) null, 0));
            Assert.Null(sut.Visit((IMethodDeclaration) null, 0));
            Assert.Null(sut.Visit((IPropertyDeclaration) null, 0));
            Assert.Null(sut.Visit((IVariableDeclaration) null, 0));

            // statements
            Assert.Null(sut.Visit((IAssignment) null, 0));
            Assert.Null(sut.Visit((IBreakStatement) null, 0));
            Assert.Null(sut.Visit((IContinueStatement) null, 0));
            Assert.Null(sut.Visit((IExpressionStatement) null, 0));
            Assert.Null(sut.Visit((IGotoStatement) null, 0));
            Assert.Null(sut.Visit((ILabelledStatement) null, 0));
            Assert.Null(sut.Visit((ILockBlock) null, 0));
            Assert.Null(sut.Visit((IReturnStatement) null, 0));
            Assert.Null(sut.Visit((IThrowStatement) null, 0));

            // blocks
            Assert.Null(sut.Visit((IDoLoop) null, 0));
            Assert.Null(sut.Visit((IForEachLoop) null, 0));
            Assert.Null(sut.Visit((IForLoop) null, 0));
            Assert.Null(sut.Visit((IIfElseBlock) null, 0));
            Assert.Null(sut.Visit((ISwitchBlock) null, 0));
            Assert.Null(sut.Visit((ITryBlock) null, 0));
            Assert.Null(sut.Visit((IUncheckedBlock) null, 0));
            Assert.Null(sut.Visit((IUnsafeBlock) null, 0));
            Assert.Null(sut.Visit((IUsingBlock) null, 0));
            Assert.Null(sut.Visit((IWhileLoop) null, 0));

            // Expressions
            Assert.Null(sut.Visit((ICompletionExpression) null, 0));
            Assert.Null(sut.Visit((IComposedExpression) null, 0));
            Assert.Null(sut.Visit((IIfElseExpression) null, 0));
            Assert.Null(sut.Visit((IInvocationExpression) null, 0));
            Assert.Null(sut.Visit((ILambdaExpression) null, 0));
            Assert.Null(sut.Visit((ILoopHeaderBlockExpression) null, 0));
            Assert.Null(sut.Visit((IConstantValueExpression) null, 0));
            Assert.Null(sut.Visit((INullExpression) null, 0));
            Assert.Null(sut.Visit((IReferenceExpression) null, 0));

            // References
            Assert.Null(sut.Visit((IEventReference) null, 0));
            Assert.Null(sut.Visit((IFieldReference) null, 0));
            Assert.Null(sut.Visit((IMethodReference) null, 0));
            Assert.Null(sut.Visit((IPropertyReference) null, 0));
            Assert.Null(sut.Visit((IVariableReference) null, 0));

            // Unknowns
            Assert.Null(sut.Visit((IUnknownReference) null, 0));
            Assert.Null(sut.Visit((IUnknownExpression) null, 0));
            Assert.Null(sut.Visit((IUnknownStatement) null, 0));
        }
    }
}