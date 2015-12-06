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
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.SSTs.Impl.Visitor
{
    public abstract class AbstractNodeVisitor<TContext> : ISSTNodeVisitor<TContext>
    {
        public virtual void Visit(ISST sst, TContext context)
        {
            foreach (var d in sst.Delegates)
            {
                d.Accept(this, context);
            }
            foreach (var e in sst.Events)
            {
                e.Accept(this, context);
            }
            foreach (var f in sst.Fields)
            {
                f.Accept(this, context);
            }
            foreach (var m in sst.Methods)
            {
                m.Accept(this, context);
            }
            foreach (var p in sst.Properties)
            {
                p.Accept(this, context);
            }
        }

        public void Visit(IKaVEList<IStatement> stmts, TContext context)
        {
            foreach (var stmt in stmts)
            {
                stmt.Accept(this, context);
            }
        }

        public virtual void Visit(IDelegateDeclaration stmt, TContext context) {}

        public virtual void Visit(IEventDeclaration stmt, TContext context) {}

        public virtual void Visit(IFieldDeclaration stmt, TContext context) {}

        public virtual void Visit(IMethodDeclaration stmt, TContext context)
        {
            Visit(stmt.Body, context);
        }

        public virtual void Visit(IPropertyDeclaration stmt, TContext context)
        {
            Visit(stmt.Get, context);
            Visit(stmt.Set, context);
        }

        public virtual void Visit(IVariableDeclaration stmt, TContext context)
        {
            stmt.Reference.Accept(this, context);
        }

        public virtual void Visit(IAssignment stmt, TContext context)
        {
            stmt.Reference.Accept(this, context);
            stmt.Expression.Accept(this, context);
        }

        public virtual void Visit(IBreakStatement stmt, TContext context) {}

        public virtual void Visit(IContinueStatement stmt, TContext context) {}

        public virtual void Visit(IEventSubscriptionStatement stmt, TContext context)
        {
            stmt.Reference.Accept(this, context);
            stmt.Expression.Accept(this, context);
        }

        public virtual void Visit(IExpressionStatement stmt, TContext context)
        {
            stmt.Expression.Accept(this, context);
        }

        public virtual void Visit(IGotoStatement stmt, TContext context) {}

        public virtual void Visit(ILabelledStatement stmt, TContext context)
        {
            stmt.Statement.Accept(this, context);
        }

        public virtual void Visit(IReturnStatement stmt, TContext context)
        {
            stmt.Expression.Accept(this, context);
        }

        public virtual void Visit(IThrowStatement stmt, TContext context) {}

        public virtual void Visit(IDoLoop block, TContext context)
        {
            block.Condition.Accept(this, context);
            Visit(block.Body, context);
        }

        public virtual void Visit(IForEachLoop block, TContext context)
        {
            block.Declaration.Accept(this, context);
            block.LoopedReference.Accept(this, context);
            Visit(block.Body, context);
        }

        public virtual void Visit(IForLoop block, TContext context)
        {
            Visit(block.Init, context);
            block.Condition.Accept(this, context);
            Visit(block.Step, context);
            Visit(block.Body, context);
        }

        public virtual void Visit(IIfElseBlock block, TContext context)
        {
            block.Condition.Accept(this, context);
            Visit(block.Then, context);
            Visit(block.Else, context);
        }

        public virtual void Visit(ILockBlock block, TContext context)
        {
            block.Reference.Accept(this, context);
            Visit(block.Body, context);
        }

        public virtual void Visit(ISwitchBlock block, TContext context)
        {
            block.Reference.Accept(this, context);
            foreach (var caseBlock in block.Sections)
            {
                caseBlock.Label.Accept(this, context);
                Visit(caseBlock.Body, context);
            }
            Visit(block.DefaultSection, context);
        }

        public virtual void Visit(ITryBlock block, TContext context)
        {
            Visit(block.Body, context);
            foreach (var catchBlock in block.CatchBlocks)
            {
                Visit(catchBlock.Body, context);
            }
            Visit(block.Finally, context);
        }

        public virtual void Visit(IUncheckedBlock block, TContext context)
        {
            Visit(block.Body, context);
        }

        public virtual void Visit(IUnsafeBlock block, TContext context) {}

        public virtual void Visit(IUsingBlock block, TContext context)
        {
            block.Reference.Accept(this, context);
            Visit(block.Body, context);
        }

        public virtual void Visit(IWhileLoop block, TContext context)
        {
            block.Condition.Accept(this, context);
            Visit(block.Body, context);
        }

        public virtual void Visit(ICompletionExpression entity, TContext context) {}

        public virtual void Visit(IComposedExpression expr, TContext context)
        {
            foreach (var r in expr.References)
            {
                r.Accept(this, context);
            }
        }

        public virtual void Visit(IIfElseExpression expr, TContext context)
        {
            expr.Condition.Accept(this, context);
            expr.ThenExpression.Accept(this, context);
            expr.ElseExpression.Accept(this, context);
        }

        public virtual void Visit(IInvocationExpression entity, TContext context)
        {
            entity.Reference.Accept(this, context);
            foreach (var p in entity.Parameters)
            {
                p.Accept(this, context);
            }
        }

        public virtual void Visit(ILambdaExpression expr, TContext context)
        {
            Visit(expr.Body, context);
        }

        public virtual void Visit(ILoopHeaderBlockExpression expr, TContext context)
        {
            Visit(expr.Body, context);
        }

        public virtual void Visit(IConstantValueExpression expr, TContext context) {}

        public virtual void Visit(INullExpression expr, TContext context) {}

        public virtual void Visit(IReferenceExpression expr, TContext context)
        {
            expr.Reference.Accept(this, context);
        }

        public virtual void Visit(ICastExpression expr, TContext context)
        {
            expr.Reference.Accept(this, context);
        }

        public virtual void Visit(IIndexAccessExpression expr, TContext context)
        {
            expr.Reference.Accept(this, context);
        }

        public virtual void Visit(ITypeCheckExpression expr, TContext context)
        {
            expr.Reference.Accept(this, context);
        }

        public virtual void Visit(IUnaryExpression expr, TContext context)
        {
            expr.Operand.Accept(this, context);
        }

        public virtual void Visit(IBinaryExpression expr, TContext context)
        {
            expr.LeftOperand.Accept(this, context);
            expr.RightOperand.Accept(this, context);
        }

        public virtual void Visit(IEventReference eventRef, TContext context)
        {
            eventRef.Reference.Accept(this, context);
        }

        public virtual void Visit(IFieldReference fieldRef, TContext context)
        {
            fieldRef.Reference.Accept(this, context);
        }

        public virtual void Visit(IMethodReference methodRef, TContext context)
        {
            methodRef.Reference.Accept(this, context);
        }

        public virtual void Visit(IPropertyReference propertyRef, TContext context)
        {
            propertyRef.Reference.Accept(this, context);
        }

        public virtual void Visit(IVariableReference varRef, TContext context) {}

        public virtual void Visit(IIndexAccessReference indexAccessRef, TContext context)
        {
            indexAccessRef.Expression.Accept(this, context);
        }

        public virtual void Visit(IUnknownReference unknownRef, TContext context) {}

        public virtual void Visit(IUnknownExpression unknownExpr, TContext context) {}

        public virtual void Visit(IUnknownStatement unknownStmt, TContext context) {}
    }

    public abstract class AbstractNodeVisitor<TContext, TReturn> : ISSTNodeVisitor<TContext, TReturn>
    {
        public virtual TReturn Visit(ISST sst, TContext context)
        {
            foreach (var d in sst.Delegates)
            {
                d.Accept(this, context);
            }
            foreach (var e in sst.Events)
            {
                e.Accept(this, context);
            }
            foreach (var f in sst.Fields)
            {
                f.Accept(this, context);
            }
            foreach (var m in sst.Methods)
            {
                m.Accept(this, context);
            }
            foreach (var p in sst.Properties)
            {
                p.Accept(this, context);
            }
            return default(TReturn);
        }

        private void Visit(IKaVEList<IStatement> stmts, TContext context)
        {
            foreach (var stmt in stmts)
            {
                stmt.Accept(this, context);
            }
        }

        public virtual TReturn Visit(IDelegateDeclaration stmt, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IEventDeclaration stmt, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IFieldDeclaration stmt, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IMethodDeclaration stmt, TContext context)
        {
            Visit(stmt.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IPropertyDeclaration stmt, TContext context)
        {
            Visit(stmt.Get, context);
            Visit(stmt.Set, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IVariableDeclaration stmt, TContext context)
        {
            stmt.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IAssignment stmt, TContext context)
        {
            stmt.Reference.Accept(this, context);
            stmt.Expression.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IBreakStatement stmt, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IContinueStatement stmt, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IEventSubscriptionStatement stmt, TContext context)
        {
            stmt.Reference.Accept(this, context);
            stmt.Expression.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IExpressionStatement stmt, TContext context)
        {
            stmt.Expression.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IGotoStatement stmt, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(ILabelledStatement stmt, TContext context)
        {
            stmt.Statement.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IReturnStatement stmt, TContext context)
        {
            stmt.Expression.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IThrowStatement stmt, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IDoLoop block, TContext context)
        {
            block.Condition.Accept(this, context);
            Visit(block.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IForEachLoop block, TContext context)
        {
            block.Declaration.Accept(this, context);
            block.LoopedReference.Accept(this, context);
            Visit(block.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IForLoop block, TContext context)
        {
            Visit(block.Init, context);
            block.Condition.Accept(this, context);
            Visit(block.Step, context);
            Visit(block.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IIfElseBlock block, TContext context)
        {
            block.Condition.Accept(this, context);
            Visit(block.Then, context);
            Visit(block.Else, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(ILockBlock block, TContext context)
        {
            block.Reference.Accept(this, context);
            Visit(block.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(ISwitchBlock block, TContext context)
        {
            block.Reference.Accept(this, context);
            foreach (var caseBlock in block.Sections)
            {
                caseBlock.Label.Accept(this, context);
                Visit(caseBlock.Body, context);
            }
            Visit(block.DefaultSection, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(ITryBlock block, TContext context)
        {
            Visit(block.Body, context);
            foreach (var catchBlock in block.CatchBlocks)
            {
                Visit(catchBlock.Body, context);
            }
            Visit(block.Finally, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IUncheckedBlock block, TContext context)
        {
            Visit(block.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IUnsafeBlock block, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IUsingBlock block, TContext context)
        {
            block.Reference.Accept(this, context);
            Visit(block.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IWhileLoop block, TContext context)
        {
            block.Condition.Accept(this, context);
            Visit(block.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(ICompletionExpression entity, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IComposedExpression expr, TContext context)
        {
            foreach (var r in expr.References)
            {
                r.Accept(this, context);
            }
            return default(TReturn);
        }

        public virtual TReturn Visit(IIfElseExpression expr, TContext context)
        {
            expr.Condition.Accept(this, context);
            expr.ThenExpression.Accept(this, context);
            expr.ElseExpression.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IInvocationExpression entity, TContext context)
        {
            entity.Reference.Accept(this, context);
            foreach (var p in entity.Parameters)
            {
                p.Accept(this, context);
            }
            return default(TReturn);
        }

        public virtual TReturn Visit(ILambdaExpression expr, TContext context)
        {
            Visit(expr.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(ILoopHeaderBlockExpression expr, TContext context)
        {
            Visit(expr.Body, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IConstantValueExpression expr, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(INullExpression expr, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IReferenceExpression expr, TContext context)
        {
            expr.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(ICastExpression expr, TContext context)
        {
            expr.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(ITypeCheckExpression expr, TContext context)
        {
            expr.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IUnaryExpression expr, TContext context)
        {
            expr.Operand.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IBinaryExpression expr, TContext context)
        {
            expr.LeftOperand.Accept(this, context);
            expr.RightOperand.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IIndexAccessExpression expr, TContext context)
        {
            expr.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IEventReference eventRef, TContext context)
        {
            eventRef.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IFieldReference fieldRef, TContext context)
        {
            fieldRef.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IMethodReference methodRef, TContext context)
        {
            methodRef.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IPropertyReference propertyRef, TContext context)
        {
            propertyRef.Reference.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IVariableReference varRef, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IIndexAccessReference indexAccessRef, TContext context)
        {
            indexAccessRef.Expression.Accept(this, context);
            return default(TReturn);
        }

        public virtual TReturn Visit(IUnknownReference unknownRef, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IUnknownExpression unknownExpr, TContext context)
        {
            return default(TReturn);
        }

        public virtual TReturn Visit(IUnknownStatement unknownStmt, TContext context)
        {
            return default(TReturn);
        }
    }
}