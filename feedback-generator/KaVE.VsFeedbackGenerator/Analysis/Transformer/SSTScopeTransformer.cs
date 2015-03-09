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
 *    - Dennis Albrecht
 *    - Uli Fahrer
 */

using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Blocks;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.References;
using KaVE.VsFeedbackGenerator.Analysis.Transformer.Context;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class SSTScopeTransformer : BaseSSTTransformer<ScopeTransformerContext>
    {
        public override void VisitBlock(IBlock blockParam, ScopeTransformerContext context)
        {
            blockParam.Statements.ForEach(s => s.Accept(this, context));
        }

        public override void VisitCSharpExpression(ICSharpExpression cSharpExpressionParam,
            ScopeTransformerContext context)
        {
            cSharpExpressionParam.CollectSideEffects(context);
        }

        public override void VisitDeclarationStatement(IDeclarationStatement declarationStatementParam,
            ScopeTransformerContext context)
        {
            declarationStatementParam.Declaration.Accept(this, context);
        }

        public override void VisitDoStatement(IDoStatement doStatementParam, ScopeTransformerContext context)
        {
            var scopedReferences = doStatementParam.Condition.GetScopedReferences(context);
            // TODO
            var doLoop = new DoLoop {Condition = null};
            doLoop.Body.AddRange(doStatementParam.Body.GetScope(context).Body);
            context.Scope.Body.Add(doLoop);
        }

        public override void VisitExpressionStatement(IExpressionStatement expressionStatementParam,
            ScopeTransformerContext context)
        {
            expressionStatementParam.Expression.Accept(this, context);
        }

        public override void VisitForeachStatement(IForeachStatement foreachStatementParam,
            ScopeTransformerContext context)
        {
            var foreachLoop = new ForEachLoop
            {
                Declaration =
                    SSTUtil.Declare(
                        foreachStatementParam.IteratorName,
                        foreachStatementParam.IteratorDeclaration.DeclaredElement.Type.GetName()),
                LoopedReference = Ref(foreachStatementParam.Collection.GetReference(context))
            };
            foreachLoop.Body.AddRange(foreachStatementParam.Body.GetScope(context).Body);
            context.Scope.Body.Add(foreachLoop);
        }

        private static IVariableReference Ref(string id)
        {
            return new VariableReference {Identifier = id};
        }

        public override void VisitForInitializer(IForInitializer forInitializerParam, ScopeTransformerContext context)
        {
            if (forInitializerParam.Declaration != null)
            {
                forInitializerParam.Declaration.Accept(this, context);
            }
            forInitializerParam.Expressions.ForEach(e => e.Accept(this, context));
        }

        public override void VisitForIterator(IForIterator forIteratorParam, ScopeTransformerContext context)
        {
            forIteratorParam.Expressions.ForEach(e => e.Accept(this, context));
        }

        public override void VisitForStatement(IForStatement forStatementParam, ScopeTransformerContext context)
        {
            var forLoop = new ForLoop();
            context.Scope.Body.Add(forLoop);
            if (forStatementParam.Condition != null)
            {
                var scopedReferences = forStatementParam.Condition.GetScopedReferences(context);
                // TODO
                forLoop.Condition = null;
            }
            if (forStatementParam.Initializer != null)
            {
                forLoop.Init.AddRange(forStatementParam.Initializer.GetScope(context).Body);
            }
            if (forStatementParam.Iterators != null)
            {
                forLoop.Step.AddRange(forStatementParam.Iterators.GetScope(context).Body);
            }
            if (forStatementParam.Body != null)
            {
                forLoop.Body.AddRange(forStatementParam.Body.GetScope(context).Body);
            }
        }

        public override void VisitIfStatement(IIfStatement ifStatementParam, ScopeTransformerContext context)
        {
            if (ifStatementParam.Condition != null)
            {
                var ifBlock = new IfElseBlock();
                if (ifStatementParam.Condition != null)
                {
                    var assignableExpression = ifStatementParam.Condition.GetReferences(context);
                    // TODO
                    ifBlock.Condition = null;
                }
                if (ifStatementParam.Then != null)
                {
                    ifBlock.Then.AddRange(ifStatementParam.Then.GetScope(context).Body);
                }
                if (ifStatementParam.Else != null)
                {
                    ifBlock.Else.AddRange(ifStatementParam.Else.GetScope(context).Body);
                }
                context.Scope.Body.Add(ifBlock);
            }
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam,
            ScopeTransformerContext context)
        {
            HandleInvocationExpression(
                invocationExpressionParam,
                context,
                (callee, method, args, retType) =>
                {
                    var basicExpressions =
                        args.Select<string, ISimpleExpression>(
                            id => new ReferenceExpression {Reference = new VariableReference {Identifier = id}})
                            .AsArray();
                    context.Scope.Body.Add(new ExpressionStatement{Expression = SSTUtil.InvocationExpression(method, basicExpressions)});
                });
        }

        public override void VisitLocalVariableDeclaration(ILocalVariableDeclaration localVariableDeclarationParam,
            ScopeTransformerContext context)
        {
            base.VisitLocalVariableDeclaration(localVariableDeclarationParam, context);

            if (localVariableDeclarationParam.Initializer is IExpressionInitializer)
            {
                var expression = (localVariableDeclarationParam.Initializer as IExpressionInitializer).Value;
                var dest = localVariableDeclarationParam.NameIdentifier.Name;
                expression.ProcessAssignment(context, dest);
            }
        }

        public override void VisitMethodDeclaration(IMethodDeclaration methodDeclarationParam,
            ScopeTransformerContext context)
        {
            if (methodDeclarationParam.Body != null)
            {
                methodDeclarationParam.Body.Accept(this, context);
            }
        }

        public override void VisitMultipleDeclarationMember(IMultipleDeclarationMember multipleDeclarationMemberParam,
            ScopeTransformerContext context)
        {
            var name = multipleDeclarationMemberParam.NameIdentifier.Name;
            var type = multipleDeclarationMemberParam.Type.GetName();
            context.Scope.Body.Add(SSTUtil.Declare(name, type));
        }

        public override void VisitMultipleLocalVariableDeclaration(
            IMultipleLocalVariableDeclaration multipleLocalVariableDeclarationParam,
            ScopeTransformerContext context)
        {
            multipleLocalVariableDeclarationParam.Declarators.ForEach(d => d.Accept(this, context));
        }

        public override void VisitReturnStatement(IReturnStatement returnStatementParam, ScopeTransformerContext context)
        {
            var references = returnStatementParam.Value.GetReferences(context);
            // TODO
            context.Scope.Body.Add(new ReturnStatement {Expression = null});
        }

        public override void VisitThrowStatement(IThrowStatement throwStatementParam, ScopeTransformerContext context)
        {
            context.Scope.Body.Add(new ThrowStatement {Exception = throwStatementParam.Exception.Type().GetName()});
        }

        public override void VisitTryStatement(ITryStatement tryStatementParam, ScopeTransformerContext context)
        {
            var tryBlock = new TryBlock();
            tryBlock.Body.AddRange(tryStatementParam.Try.GetScope(context).Body);
            tryBlock.CatchBlocks.AddRange(tryStatementParam.Catches.Select(c => c.GetCatchBlock(context)));
            var finallyBlock = tryStatementParam.FinallyBlock;
            if (finallyBlock != null)
            {
                tryBlock.Finally.AddRange(finallyBlock.GetScope(context).Body);
                context.Scope.Body.Add(tryBlock);
            }
        }

        public override void VisitUsingStatement(IUsingStatement usingStatementParam, ScopeTransformerContext context)
        {
            var usingBlock = new UsingBlock();
            if (usingStatementParam.Expressions.Count > 0)
            {
                usingBlock.Reference = Ref(usingStatementParam.Expressions.GetReference(context));
            }
            usingBlock.Body.AddRange(usingStatementParam.Body.GetScope(context).Body);
            context.Scope.Body.Add(usingBlock);
        }

        public override void VisitWhileStatement(IWhileStatement whileStatementParam, ScopeTransformerContext context)
        {
            var whileLoop = new WhileLoop();
            context.Scope.Body.Add(whileLoop);
            if (whileStatementParam.Condition != null)
            {
                var scopedReferences = whileStatementParam.Condition.GetScopedReferences(context);
                // TODO
                whileLoop.Condition = null;
            }
            if (whileStatementParam.Body != null)
            {
                whileLoop.Body.AddRange(whileStatementParam.Body.GetScope(context).Body);
            }
        }

        #region Redirections

        public override void VisitOperatorExpression(IOperatorExpression operatorExpressionParam,
            ScopeTransformerContext context)
        {
            VisitCSharpExpression(operatorExpressionParam, context);
        }

        public override void VisitAssignmentExpression(IAssignmentExpression assignmentExpressionParam,
            ScopeTransformerContext context)
        {
            VisitOperatorExpression(assignmentExpressionParam, context);
        }

        public override void VisitPostfixOperatorExpression(IPostfixOperatorExpression postfixOperatorExpressionParam,
            ScopeTransformerContext context)
        {
            VisitOperatorExpression(postfixOperatorExpressionParam, context);
        }

        public override void VisitPrefixOperatorExpression(IPrefixOperatorExpression prefixOperatorExpressionParam,
            ScopeTransformerContext context)
        {
            VisitOperatorExpression(prefixOperatorExpressionParam, context);
        }

        #endregion
    }
}