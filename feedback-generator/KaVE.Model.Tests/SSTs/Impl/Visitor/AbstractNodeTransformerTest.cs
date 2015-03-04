/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"));
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
 *    - 
 */

using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Expressions.LoopHeader;
using KaVE.Model.SSTs.Expressions.Simple;
using KaVE.Model.SSTs.Impl.Visitor;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Impl.Visitor
{
    internal class AbstractNodeTransformerTest
    {
        [Test]
        public void AllVisitsAreImplemented()
        {
            var sut = new AbstractNodeTransformer<object>();
            Assert.Null(sut.Visit((ISST) null));

            // declarations
            Assert.Null(sut.Visit((IDelegateDeclaration) null));
            Assert.Null(sut.Visit((IEventDeclaration) null));
            Assert.Null(sut.Visit((IFieldDeclaration) null));
            Assert.Null(sut.Visit((IMethodDeclaration) null));
            Assert.Null(sut.Visit((IPropertyDeclaration) null));
            Assert.Null(sut.Visit((IVariableDeclaration) null));

            // ambiguous entities
            Assert.Null(sut.Visit((ICompletion) null));
            Assert.Null(sut.Visit((IInvocation) null));

            // statements
            Assert.Null(sut.Visit((IAssignment) null));
            Assert.Null(sut.Visit((IBreakStatement) null));
            Assert.Null(sut.Visit((IContinueStatement) null));
            Assert.Null(sut.Visit((IGotoStatement) null));
            Assert.Null(sut.Visit((ILabelledStatement) null));
            Assert.Null(sut.Visit((ILockStatement) null));
            Assert.Null(sut.Visit((IReturnStatement) null));
            Assert.Null(sut.Visit((IThrowStatement) null));

            // blocks
            Assert.Null(sut.Visit((ICaseBlock) null));
            Assert.Null(sut.Visit((ICatchBlock) null));
            Assert.Null(sut.Visit((IDoLoop) null));
            Assert.Null(sut.Visit((IForEachLoop) null));
            Assert.Null(sut.Visit((IForLoop) null));
            Assert.Null(sut.Visit((IIfElseBlock) null));
            Assert.Null(sut.Visit((ISwitchBlock) null));
            Assert.Null(sut.Visit((ITryBlock) null));
            Assert.Null(sut.Visit((IUncheckedBlock) null));
            Assert.Null(sut.Visit((IUnsafeBlock) null));
            Assert.Null(sut.Visit((IUsingBlock) null));
            Assert.Null(sut.Visit((IWhileLoop) null));

            // Expressions
            Assert.Null(sut.Visit((IComposedExpression) null));
            Assert.Null(sut.Visit((IIfElseExpression) null));
            Assert.Null(sut.Visit((ILambdaExpression) null));
            Assert.Null(sut.Visit((ILoopHeaderBlockExpression) null));
            Assert.Null(sut.Visit((IConstantValueExpression) null));
            Assert.Null(sut.Visit((INullExpression) null));
            Assert.Null(sut.Visit((IReferenceExpression) null));

            // References
            Assert.Null(sut.Visit((IEventReference) null));
            Assert.Null(sut.Visit((IFieldReference) null));
            Assert.Null(sut.Visit((IMethodReference) null));
            Assert.Null(sut.Visit((IPropertyReference) null));
            Assert.Null(sut.Visit((IVariableReference) null));
        }
    }
}