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

using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.Util;
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Declarations;
using KaVE.Model.SSTs.Statements;
using KaVE.VsFeedbackGenerator.Analysis.Util;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class ScopeTransformerContext : ITransformerContext
    {
        public ScopeTransformerContext(ITransformerContext context)
            : this(context.Factory, context.Generator, context.Scope) {}

        public ScopeTransformerContext(ISSTFactory factory,
            ITempVariableGenerator generator,
            IScope scope)
        {
            Generator = generator;
            Factory = factory;
            Scope = scope;
        }

        public ISSTFactory Factory { get; private set; }
        public ITempVariableGenerator Generator { get; private set; }
        public IScope Scope { get; private set; }
    }

    public class SSTScopeTransformer : BaseSSTTransformer<ScopeTransformerContext>
    {
        public override void VisitMethodDeclaration(IMethodDeclaration methodDeclarationParam,
            ScopeTransformerContext context)
        {
            methodDeclarationParam.Body.Accept(this, context);
        }

        public override void VisitBlock(IBlock blockParam, ScopeTransformerContext context)
        {
            blockParam.Statements.ForEach(s => s.Accept(this, context));
        }

        public override void VisitDeclarationStatement(IDeclarationStatement declarationStatementParam,
            ScopeTransformerContext context)
        {
            declarationStatementParam.Declaration.Accept(this, context);
        }

        public override void VisitExpressionStatement(IExpressionStatement expressionStatementParam,
            ScopeTransformerContext context)
        {
            expressionStatementParam.Expression.Accept(this, context);
        }

        public override void VisitIfStatement(IIfStatement ifStatementParam, ScopeTransformerContext context)
        {
            var ifBlock = new IfElseBlock();
            var refCollectorContext = new ReferenceCollectorContext(context);
            ifStatementParam.Condition.Accept(context.Factory.ReferenceCollector(), refCollectorContext);
            ifBlock.Condition = refCollectorContext.References.AsExpression();
            var thenScopeContext = new ScopeTransformerContext(
                context.Factory,
                context.Generator,
                context.Factory.Scope());
            ifStatementParam.Then.Accept(context.Factory.ScopeTransformer(), thenScopeContext);
            ifBlock.Then.AddRange(thenScopeContext.Scope.Body);
            if (ifStatementParam.Else != null)
            {
                var elseScopeContext = new ScopeTransformerContext(
                    context.Factory,
                    context.Generator,
                    context.Factory.Scope());
                ifStatementParam.Else.Accept(context.Factory.ScopeTransformer(), elseScopeContext);
                ifBlock.Else.AddRange(elseScopeContext.Scope.Body);
            }
            context.Scope.Body.Add(ifBlock);
        }

        public override void VisitWhileStatement(IWhileStatement whileStatementParam, ScopeTransformerContext context)
        {
            var whileLoop = new WhileLoop();
            var refCollectorContext = new ReferenceCollectorContext(context);
            whileStatementParam.Condition.Accept(context.Factory.ReferenceCollector(), refCollectorContext);
            whileLoop.Condition = refCollectorContext.References.AsExpression();
            var bodyScopeContext = new ScopeTransformerContext(
                context.Factory,
                context.Generator,
                context.Factory.Scope());
            whileStatementParam.Body.Accept(context.Factory.ScopeTransformer(), bodyScopeContext);
            whileLoop.Body.AddRange(bodyScopeContext.Scope.Body);
            context.Scope.Body.Add(whileLoop);
        }

        public override void VisitDoStatement(IDoStatement doStatementParam, ScopeTransformerContext context)
        {
            var doLoop = new DoLoop();
            var refCollectorContext = new ReferenceCollectorContext(context);
            doStatementParam.Condition.Accept(context.Factory.ReferenceCollector(), refCollectorContext);
            doLoop.Condition = refCollectorContext.References.AsExpression();
            var bodyScopeContext = new ScopeTransformerContext(
                context.Factory,
                context.Generator,
                context.Factory.Scope());
            doStatementParam.Body.Accept(context.Factory.ScopeTransformer(), bodyScopeContext);
            doLoop.Body.AddRange(bodyScopeContext.Scope.Body);
            context.Scope.Body.Add(doLoop);
        }

        public override void VisitReturnStatement(IReturnStatement returnStatementParam, ScopeTransformerContext context)
        {
            var refCollectorContext = new ReferenceCollectorContext(context);
            returnStatementParam.Value.Accept(context.Factory.ReferenceCollector(), refCollectorContext);
            context.Scope.Body.Add(new ReturnStatement{Expression = refCollectorContext.References.AsExpression()});
        }

        public override void VisitMultipleLocalVariableDeclaration(
            IMultipleLocalVariableDeclaration multipleLocalVariableDeclarationParam,
            ScopeTransformerContext context)
        {
            multipleLocalVariableDeclarationParam.Declarators.ForEach(d => d.Accept(this, context));
        }

        public override void VisitMultipleDeclarationMember(IMultipleDeclarationMember multipleDeclarationMemberParam,
            ScopeTransformerContext context)
        {
            var name = multipleDeclarationMemberParam.NameIdentifier.Name;
            var type = multipleDeclarationMemberParam.Type.GetName();
            context.Scope.Body.Add(new VariableDeclaration(name, type));
        }

        public override void VisitLocalVariableDeclaration(ILocalVariableDeclaration localVariableDeclarationParam,
            ScopeTransformerContext context)
        {
            base.VisitLocalVariableDeclaration(localVariableDeclarationParam, new ScopeTransformerContext(context));

            if (localVariableDeclarationParam.Initializer is IExpressionInitializer)
            {
                var expression = (localVariableDeclarationParam.Initializer as IExpressionInitializer).Value;
                var dest = localVariableDeclarationParam.NameIdentifier.Name;
                expression.Accept(
                    context.Factory.AssignmentGenerator(),
                    new AssignmentGeneratorContext(context, dest));
            }
        }

        public override void VisitAssignmentExpression(IAssignmentExpression assignmentExpressionParam,
            ScopeTransformerContext context)
        {
            if (assignmentExpressionParam.Dest is IReferenceExpression)
            {
                var dest = (assignmentExpressionParam.Dest as IReferenceExpression).NameIdentifier.Name;
                assignmentExpressionParam.Source.Accept(
                    context.Factory.AssignmentGenerator(),
                    new AssignmentGeneratorContext(context, dest));
            }
        }

        public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam,
            ScopeTransformerContext context)
        {
            HandleInvocationExpression(
                invocationExpressionParam,
                context,
                (callee, method, args, retType) =>
                    context.Scope.Body.Add(new InvocationStatement(callee, method, args)));
        }
    }
}