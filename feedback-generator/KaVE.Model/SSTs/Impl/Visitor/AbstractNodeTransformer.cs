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
 *    - Seabstian Proksch
 */

using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Expressions.LoopHeader;
using KaVE.Model.SSTs.Expressions.Simple;
using KaVE.Model.SSTs.References;
using KaVE.Model.SSTs.Statements;
using KaVE.Model.SSTs.Visitor;

namespace KaVE.Model.SSTs.Impl.Visitor
{
    public class AbstractNodeTransformer<TContext> : ISSTNodeTransformer<TContext> where TContext:class
    {
        public virtual TContext Visit(ISST sst) { return null; }

        public virtual TContext Visit(IDelegateDeclaration stmt) { return null; }
        public virtual TContext Visit(IEventDeclaration stmt) { return null; }
        public virtual TContext Visit(IFieldDeclaration stmt) { return null; }
        public virtual TContext Visit(IMethodDeclaration stmt) { return null; }
        public virtual TContext Visit(IPropertyDeclaration stmt) { return null; }
        public virtual TContext Visit(IVariableDeclaration stmt) { return null; }

        public virtual TContext Visit(ICompletion entity) { return null; }
        public virtual TContext Visit(IInvocation entity) { return null; }

        public virtual TContext Visit(IAssignment stmt) { return null; }
        public virtual TContext Visit(IBreakStatement stmt) { return null; }
        public virtual TContext Visit(IContinueStatement stmt) { return null; }
        public virtual TContext Visit(IGotoStatement stmt) { return null; }
        public virtual TContext Visit(ILabelledStatement stmt) { return null; }
        public virtual TContext Visit(ILockStatement stmt) { return null; }
        public virtual TContext Visit(IReturnStatement stmt) { return null; }
        public virtual TContext Visit(IThrowStatement stmt) { return null; }

        public virtual TContext Visit(ICaseBlock block) { return null; }
        public virtual TContext Visit(ICatchBlock block) { return null; }
        public virtual TContext Visit(IDoLoop block) { return null; }
        public virtual TContext Visit(IForEachLoop block) { return null; }
        public virtual TContext Visit(IForLoop block) { return null; }
        public virtual TContext Visit(IIfElseBlock block) { return null; }
        public virtual TContext Visit(ISwitchBlock block) { return null; }
        public virtual TContext Visit(ITryBlock block) { return null; }
        public virtual TContext Visit(IUncheckedBlock block) { return null; }
        public virtual TContext Visit(IUnsafeBlock block) { return null; }
        public virtual TContext Visit(IUsingBlock block) { return null; }
        public virtual TContext Visit(IWhileLoop block) { return null; }

        public virtual TContext Visit(IComposedExpression expr) { return null; }
        public virtual TContext Visit(IIfElseExpression expr) { return null; }
        public virtual TContext Visit(ILambdaExpression expr) { return null; }
        public virtual TContext Visit(ILoopHeaderBlockExpression expr) { return null; }
        public virtual TContext Visit(IConstantValueExpression expr) { return null; }
        public virtual TContext Visit(INullExpression expr) { return null; }
        public virtual TContext Visit(IReferenceExpression expr) { return null; }

        public virtual TContext Visit(IEventReference eventRef) { return null; }
        public virtual TContext Visit(IFieldReference fieldRef) { return null; }
        public virtual TContext Visit(IMethodReference methodRef) { return null; }
        public virtual TContext Visit(IPropertyReference methodRef) { return null; }
        public virtual TContext Visit(IVariableReference varRef) { return null; }
    }
}