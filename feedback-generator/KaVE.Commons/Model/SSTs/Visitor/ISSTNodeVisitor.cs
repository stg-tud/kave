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

using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;

namespace KaVE.Commons.Model.SSTs.Visitor
{
    public interface ISSTNodeVisitor<in TContext>
    {
        void Visit(ISST sst, TContext context);

        // declarations
        void Visit(IDelegateDeclaration decl, TContext context);
        void Visit(IEventDeclaration decl, TContext context);
        void Visit(IFieldDeclaration decl, TContext context);
        void Visit(IMethodDeclaration decl, TContext context);
        void Visit(IPropertyDeclaration decl, TContext context);
        void Visit(IVariableDeclaration decl, TContext context);

        // statements
        void Visit(IAssignment stmt, TContext context);
        void Visit(IBreakStatement stmt, TContext context);
        void Visit(IContinueStatement stmt, TContext context);
        void Visit(IEventSubscriptionStatement stmt, TContext context);
        void Visit(IExpressionStatement stmt, TContext context);
        void Visit(IGotoStatement stmt, TContext context);
        void Visit(ILabelledStatement stmt, TContext context);
        void Visit(IReturnStatement stmt, TContext context);
        void Visit(IThrowStatement stmt, TContext context);

        // blocks
        void Visit(IDoLoop block, TContext context);
        void Visit(IForEachLoop block, TContext context);
        void Visit(IForLoop block, TContext context);
        void Visit(IIfElseBlock block, TContext context);
        void Visit(ILockBlock block, TContext context);
        void Visit(ISwitchBlock block, TContext context);
        void Visit(ITryBlock block, TContext context);
        void Visit(IUncheckedBlock block, TContext context);
        void Visit(IUnsafeBlock block, TContext context);
        void Visit(IUsingBlock block, TContext context);
        void Visit(IWhileLoop block, TContext context);

        // Expressions
        void Visit(ICompletionExpression expr, TContext context);
        void Visit(IComposedExpression expr, TContext context);
        void Visit(IIfElseExpression expr, TContext context);
        void Visit(IInvocationExpression expr, TContext context);
        void Visit(ILambdaExpression expr, TContext context);
        void Visit(ILoopHeaderBlockExpression expr, TContext context);
        void Visit(IConstantValueExpression expr, TContext context);
        void Visit(INullExpression expr, TContext context);
        void Visit(IReferenceExpression expr, TContext context);
        void Visit(ICastExpression expr, TContext context);
        void Visit(IIndexAccessExpression expr, TContext context);
        void Visit(ITypeCheckExpression expr, TContext context);
        void Visit(IUnaryExpression expr, TContext context);
        void Visit(IBinaryExpression expr, TContext context);

        // References
        void Visit(IEventReference eventRef, TContext context);
        void Visit(IFieldReference fieldRef, TContext context);
        void Visit(IMethodReference methodRef, TContext context);
        void Visit(IPropertyReference propertyRef, TContext context);
        void Visit(IVariableReference varRef, TContext context);
        void Visit(IIndexAccessReference indexAccessRef, TContext context);

        // unknowns
        void Visit(IUnknownReference unknownRef, TContext context);
        void Visit(IUnknownExpression unknownExpr, TContext context);
        void Visit(IUnknownStatement unknownStmt, TContext context);
    }

    public interface ISSTNodeVisitor<in TContext, out TReturn>
    {
        TReturn Visit(ISST sst, TContext context);

        // declarations
        TReturn Visit(IDelegateDeclaration decl, TContext context);
        TReturn Visit(IEventDeclaration decl, TContext context);
        TReturn Visit(IFieldDeclaration decl, TContext context);
        TReturn Visit(IMethodDeclaration decl, TContext context);
        TReturn Visit(IPropertyDeclaration decl, TContext context);
        TReturn Visit(IVariableDeclaration decl, TContext context);

        // statements
        TReturn Visit(IAssignment stmt, TContext context);
        TReturn Visit(IBreakStatement stmt, TContext context);
        TReturn Visit(IContinueStatement stmt, TContext context);
        TReturn Visit(IEventSubscriptionStatement stmt, TContext context);
        TReturn Visit(IExpressionStatement stmt, TContext context);
        TReturn Visit(IGotoStatement stmt, TContext context);
        TReturn Visit(ILabelledStatement stmt, TContext context);
        TReturn Visit(IReturnStatement stmt, TContext context);
        TReturn Visit(IThrowStatement stmt, TContext context);

        // blocks
        TReturn Visit(IDoLoop block, TContext context);
        TReturn Visit(IForEachLoop block, TContext context);
        TReturn Visit(IForLoop block, TContext context);
        TReturn Visit(IIfElseBlock block, TContext context);
        TReturn Visit(ILockBlock block, TContext context);
        TReturn Visit(ISwitchBlock block, TContext context);
        TReturn Visit(ITryBlock block, TContext context);
        TReturn Visit(IUncheckedBlock block, TContext context);
        TReturn Visit(IUnsafeBlock block, TContext context);
        TReturn Visit(IUsingBlock block, TContext context);
        TReturn Visit(IWhileLoop block, TContext context);

        // Expressions
        TReturn Visit(ICompletionExpression expr, TContext context);
        TReturn Visit(IComposedExpression expr, TContext context);
        TReturn Visit(IIfElseExpression expr, TContext context);
        TReturn Visit(IInvocationExpression expr, TContext context);
        TReturn Visit(ILambdaExpression expr, TContext context);
        TReturn Visit(ILoopHeaderBlockExpression expr, TContext context);
        TReturn Visit(IConstantValueExpression expr, TContext context);
        TReturn Visit(INullExpression expr, TContext context);
        TReturn Visit(IReferenceExpression expr, TContext context);
        TReturn Visit(ICastExpression expr, TContext context);
        TReturn Visit(IIndexAccessExpression expr, TContext context);
        TReturn Visit(ITypeCheckExpression expr, TContext context);
        TReturn Visit(IUnaryExpression expr, TContext context);
        TReturn Visit(IBinaryExpression expr, TContext context);

        // References
        TReturn Visit(IEventReference eventRef, TContext context);
        TReturn Visit(IFieldReference fieldRef, TContext context);
        TReturn Visit(IMethodReference methodRef, TContext context);
        TReturn Visit(IPropertyReference propertyRef, TContext context);
        TReturn Visit(IVariableReference varRef, TContext context);
        TReturn Visit(IIndexAccessReference indexAccessRef, TContext context);

        // unknowns
        TReturn Visit(IUnknownReference unknownRef, TContext context);
        TReturn Visit(IUnknownExpression unknownExpr, TContext context);
        TReturn Visit(IUnknownStatement unknownStmt, TContext context);
    }
}