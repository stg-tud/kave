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

            // ambiguous entities
            sut.Visit((ICompletion) null, null);
            sut.Visit((IInvocation) null, null);

            // statements
            sut.Visit((IAssignment) null, null);
            sut.Visit((IBreakStatement) null, null);
            sut.Visit((IContinueStatement) null, null);
            sut.Visit((IGotoStatement) null, null);
            sut.Visit((ILabelledStatement) null, null);
            sut.Visit((ILockStatement) null, null);
            sut.Visit((IReturnStatement) null, null);
            sut.Visit((IThrowStatement) null, null);

            // blocks
            sut.Visit((ICaseBlock) null, null);
            sut.Visit((ICatchBlock) null, null);
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
            sut.Visit((IComposedExpression) null, null);
            sut.Visit((IIfElseExpression) null, null);
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
        }
    }
}