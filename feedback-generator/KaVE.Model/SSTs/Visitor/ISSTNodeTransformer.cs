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
 *    - 
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
    public interface ISSTNodeTransformer<out TContext>
    {
        TContext Visit(ISST sst);

        // declarations
        TContext Visit(IDelegateDeclaration stmt);
        TContext Visit(IEventDeclaration stmt);
        TContext Visit(IFieldDeclaration stmt);
        TContext Visit(IMethodDeclaration stmt);
        TContext Visit(IPropertyDeclaration stmt);
        TContext Visit(IVariableDeclaration stmt);

        // ambiguous entities
        TContext Visit(ICompletion entity);
        TContext Visit(IInvocation entity);

        // statements
        TContext Visit(IAssignment stmt);
        TContext Visit(IBreakStatement stmt);
        TContext Visit(IContinueStatement stmt);
        TContext Visit(IGotoStatement stmt);
        TContext Visit(ILabelledStatement stmt);
        TContext Visit(ILockStatement stmt);
        TContext Visit(IReturnStatement stmt);
        TContext Visit(IThrowStatement stmt);

        // blocks
        TContext Visit(ICaseBlock block);
        TContext Visit(ICatchBlock block);
        TContext Visit(IDoLoop block);
        TContext Visit(IForEachLoop block);
        TContext Visit(IForLoop block);
        TContext Visit(IIfElseBlock block);
        TContext Visit(ISwitchBlock block);
        TContext Visit(ITryBlock block);
        TContext Visit(IUncheckedBlock block);
        TContext Visit(IUnsafeBlock block);
        TContext Visit(IUsingBlock block);
        TContext Visit(IWhileLoop block);

        // Expressions
        TContext Visit(IComposedExpression expr);
        TContext Visit(IIfElseExpression expr);
        TContext Visit(ILambdaExpression expr);
        TContext Visit(ILoopHeaderBlockExpression expr);
        TContext Visit(IConstantValueExpression expr);
        TContext Visit(INullExpression expr);
        TContext Visit(IReferenceExpression expr);

        // References
        TContext Visit(IEventReference eventRef);
        TContext Visit(IFieldReference fieldRef);
        TContext Visit(IMethodReference methodRef);
        TContext Visit(IPropertyReference methodRef);
        TContext Visit(IVariableReference varRef);
    }
}