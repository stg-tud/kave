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
using KaVE.Commons.Model.SSTs.Visitor;

namespace KaVE.Commons.Model.SSTs.Impl.Visitor
{
    public class AbstractNodeVisitor<TContext> : ISSTNodeVisitor<TContext>
    {
        public virtual void Visit(ISST sst, TContext context) {}

        // declarations
        public virtual void Visit(IDelegateDeclaration stmt, TContext context) {}
        public virtual void Visit(IEventDeclaration stmt, TContext context) {}
        public virtual void Visit(IFieldDeclaration stmt, TContext context) {}
        public virtual void Visit(IMethodDeclaration stmt, TContext context) {}
        public virtual void Visit(IPropertyDeclaration stmt, TContext context) {}
        public virtual void Visit(IVariableDeclaration stmt, TContext context) {}

        // statements
        public virtual void Visit(IAssignment stmt, TContext context) {}
        public virtual void Visit(IBreakStatement stmt, TContext context) {}
        public virtual void Visit(IContinueStatement stmt, TContext context) {}
        public virtual void Visit(IExpressionStatement stmt, TContext context) {}
        public virtual void Visit(IGotoStatement stmt, TContext context) {}
        public virtual void Visit(ILabelledStatement stmt, TContext context) {}
        public virtual void Visit(IReturnStatement stmt, TContext context) {}
        public virtual void Visit(IThrowStatement stmt, TContext context) {}

        // blocks
        public virtual void Visit(IDoLoop block, TContext context) {}
        public virtual void Visit(IForEachLoop block, TContext context) {}
        public virtual void Visit(IForLoop block, TContext context) {}
        public virtual void Visit(IIfElseBlock block, TContext context) {}
        public virtual void Visit(ILockBlock stmt, TContext context) {}
        public virtual void Visit(ISwitchBlock block, TContext context) {}
        public virtual void Visit(ITryBlock block, TContext context) {}
        public virtual void Visit(IUncheckedBlock block, TContext context) {}
        public virtual void Visit(IUnsafeBlock block, TContext context) {}
        public virtual void Visit(IUsingBlock block, TContext context) {}
        public virtual void Visit(IWhileLoop block, TContext context) {}

        // Expressions
        public virtual void Visit(ICompletionExpression entity, TContext context) {}
        public virtual void Visit(IComposedExpression expr, TContext context) {}
        public virtual void Visit(IIfElseExpression expr, TContext context) {}
        public virtual void Visit(IInvocationExpression entity, TContext context) {}
        public virtual void Visit(ILambdaExpression expr, TContext context) {}
        public virtual void Visit(ILoopHeaderBlockExpression expr, TContext context) {}
        public virtual void Visit(IConstantValueExpression expr, TContext context) {}
        public virtual void Visit(INullExpression expr, TContext context) {}
        public virtual void Visit(IReferenceExpression expr, TContext context) {}

        // References
        public virtual void Visit(IEventReference eventRef, TContext context) {}
        public virtual void Visit(IFieldReference fieldRef, TContext context) {}
        public virtual void Visit(IMethodReference methodRef, TContext context) {}
        public virtual void Visit(IPropertyReference methodRef, TContext context) {}
        public virtual void Visit(IVariableReference varRef, TContext context) {}

        // References
        public virtual void Visit(IUnknownReference unknownRef, TContext context) {}
        public virtual void Visit(IUnknownExpression unknownExpr, TContext context) {}
        public virtual void Visit(IUnknownStatement unknownStmt, TContext context) {}
    }

    public class AbstractNodeVisitor<TContext, TReturn> : ISSTNodeVisitor<TContext, TReturn>
        where TReturn : class
    {
        public virtual TReturn Visit(ISST sst, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IDelegateDeclaration stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IEventDeclaration stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IFieldDeclaration stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IMethodDeclaration stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IPropertyDeclaration stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IVariableDeclaration stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IAssignment stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IBreakStatement stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IContinueStatement stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IExpressionStatement stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IGotoStatement stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(ILabelledStatement stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IReturnStatement stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IThrowStatement stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IDoLoop block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IForEachLoop block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IForLoop block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IIfElseBlock block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(ILockBlock stmt, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(ISwitchBlock block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(ITryBlock block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IUncheckedBlock block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IUnsafeBlock block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IUsingBlock block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IWhileLoop block, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(ICompletionExpression entity, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IComposedExpression expr, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IIfElseExpression expr, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IInvocationExpression entity, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(ILambdaExpression expr, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(ILoopHeaderBlockExpression expr, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IConstantValueExpression expr, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(INullExpression expr, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IReferenceExpression expr, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IEventReference eventRef, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IFieldReference fieldRef, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IMethodReference methodRef, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IPropertyReference methodRef, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IVariableReference varRef, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IUnknownReference unknownRef, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IUnknownExpression unknownExpr, TContext context)
        {
            return null;
        }

        public virtual TReturn Visit(IUnknownStatement unknownStmt, TContext context)
        {
            return null;
        }
    }
}