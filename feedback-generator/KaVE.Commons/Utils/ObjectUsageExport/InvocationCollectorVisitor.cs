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
 *    - Roman Fojtik
 *    - Sebastian Proksch
 */

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    internal class InvocationCollectorVisitor : AbstractNodeVisitor<QueryContext>
    {
        private readonly DefinitionSiteEvaluatorVisitor _defSiteVisitor = new DefinitionSiteEvaluatorVisitor();

        public override void Visit(IForEachLoop block, QueryContext context)
        {
            block.Declaration.Accept(this, context);

            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IForLoop block, QueryContext context)
        {
            foreach (var statement in block.Init)
            {
                statement.Accept(this, context);
            }

            block.Condition.Accept(this, context);

            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }

            foreach (var statement in block.Step)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IIfElseBlock block, QueryContext context)
        {
            foreach (var statement in block.Then)
            {
                statement.Accept(this, context);
            }
            foreach (var statement in block.Else)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ILockBlock stmt, QueryContext context)
        {
            foreach (var statement in stmt.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ISwitchBlock block, QueryContext context)
        {
            foreach (var caseBlock in block.Sections)
            {
                foreach (var statement in caseBlock.Body)
                {
                    statement.Accept(this, context);
                }
            }
            foreach (var statement in block.DefaultSection)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ITryBlock block, QueryContext context)
        {
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
            foreach (var catchBlock in block.CatchBlocks)
            {
                context.EnterNewScope();

                var id = catchBlock.Parameter.Name;
                var type = catchBlock.Parameter.ValueType;
                var def = DefinitionSites.CreateUnknownDefinitionSite();
                context.DefineVariable(id, type, def);

                foreach (var statement in catchBlock.Body)
                {
                    statement.Accept(this, context);
                }

                context.LeaveCurrentScope();
            }
            foreach (var statement in block.Finally)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IUncheckedBlock block, QueryContext context)
        {
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IUsingBlock block, QueryContext context)
        {
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ILabelledStatement stmt, QueryContext context)
        {
            stmt.Statement.Accept(this, context);
        }

        public override void Visit(IWhileLoop block, QueryContext context)
        {
            block.Condition.Accept(this, context);
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IDoLoop block, QueryContext context)
        {
            block.Condition.Accept(this, context);
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ILoopHeaderBlockExpression expr, QueryContext context)
        {
            foreach (var statement in expr.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IVariableDeclaration stmt, QueryContext context)
        {
            if (!stmt.IsMissing)
            {
                var id = stmt.Reference.Identifier;
                var type = stmt.Type;
                var def = DefinitionSites.CreateUnknownDefinitionSite();
                context.DefineVariable(id, type, def);
            }
        }

        public override void Visit(IAssignment stmt, QueryContext context)
        {
            stmt.Expression.Accept(this, context);

            var varRef = stmt.Reference as IVariableReference;
            if (varRef != null)
            {
                var id = varRef.Identifier;
                var def = stmt.Expression.Accept(_defSiteVisitor, context);
                context.RegisterDefinition(id, def ?? DefinitionSites.CreateUnknownDefinitionSite());
            }

            // TODO @seb: field refs, etc.
        }

        public override void Visit(IExpressionStatement stmt, QueryContext context)
        {
            stmt.Expression.Accept(this, context);
        }

        public override void Visit(IInvocationExpression expr, QueryContext context)
        {
            if (expr.MethodName.IsConstructor)
            {
                return;
            }

            var id = expr.Reference.Identifier;
            var method = expr.MethodName;
            context.RegisterCallsite(id, method);
        }
    }
}