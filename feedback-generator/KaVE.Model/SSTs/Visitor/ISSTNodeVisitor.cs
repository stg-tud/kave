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

using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Expressions.LoopHeader;
using KaVE.Model.SSTs.Expressions.Simple;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Statements;

namespace KaVE.Model.SSTs.Visitor
{
    public interface ISSTNodeVisitor<in TContext>
    {
        void Visit(ISST sst, TContext context);

        // declarations
        void Visit(IDelegateDeclaration stmt, TContext context);
        void Visit(IEventDeclaration stmt, TContext context);
        void Visit(IFieldDeclaration stmt, TContext context);
        void Visit(IMethodDeclaration stmt, TContext context);
        void Visit(IPropertyDeclaration stmt, TContext context);

        // ambiguous entities
        void Visit(ICompletion entity, TContext context);
        void Visit(IInvocation entity, TContext context);

        // statements
        void Visit(IAssignment stmt, TContext context);
        void Visit(IBreakStatement stmt, TContext context);
        void Visit(IContinueStatement stmt, TContext context);
        void Visit(IGotoStatement stmt, TContext context);
        void Visit(ILabelledStatement stmt, TContext context);
        void Visit(ILockStatement stmt, TContext context);
        void Visit(IReturnStatement stmt, TContext context);
        void Visit(IThrowStatement stmt, TContext context);

        // blocks
        void Visit(ICaseBlock block, TContext context);
        void Visit(ICatchBlock block, TContext context);
        void Visit(IDoLoop block, TContext context);
        void Visit(IForEachLoop block, TContext context);
        void Visit(IForLoop block, TContext context);
        void Visit(IIfElseBlock block, TContext context);
        void Visit(ISwitchBlock block, TContext context);
        void Visit(ITryBlock block, TContext context);
        void Visit(IUnsafeBlock block, TContext context);
        void Visit(IUsingBlock block, TContext context);
        void Visit(IWhileLoop block, TContext context);

        // Expressions
        void Visit(IComposedExpression expr, TContext context);
        void Visit(IIfElseExpression expr, TContext context);
        void Visit(ILambdaExpression expr, TContext context);
        void Visit(ILoopHeaderBlockExpression expr, TContext context);
        void Visit(IConstantValueExpression expr, TContext context);
        void Visit(INullExpression expr, TContext context);
        void Visit(IReferenceExpression expr, TContext context);

        // References
        void Visit(IEventReference eventRef, TContext context);
        void Visit(IFieldReference fieldRef, TContext context);
        void Visit(IMethodReference methodRef, TContext context);
        void Visit(IPropertyReference methodRef, TContext context);
        void Visit(IVariableReference varRef, TContext context);
    }
}