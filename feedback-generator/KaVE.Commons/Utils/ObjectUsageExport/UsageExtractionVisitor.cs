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

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Model.SSTs.Statements;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    internal class UsageExtractionVisitor : AbstractNodeVisitor<UsageContext>
    {
        private readonly UsageDefinitionVisitor _defSiteVisitor = new UsageDefinitionVisitor();

        #region blocks

        public override void Visit(IDoLoop block, UsageContext context)
        {
            block.Condition.Accept(this, context);
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IForEachLoop block, UsageContext context)
        {
            block.Declaration.Accept(this, context);

            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IForLoop block, UsageContext context)
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

        public override void Visit(IIfElseBlock block, UsageContext context)
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

        public override void Visit(ILockBlock stmt, UsageContext context)
        {
            foreach (var statement in stmt.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(ISwitchBlock block, UsageContext context)
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

        public override void Visit(ITryBlock block, UsageContext context)
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

        public override void Visit(IUncheckedBlock block, UsageContext context)
        {
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IUsingBlock block, UsageContext context)
        {
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        public override void Visit(IWhileLoop block, UsageContext context)
        {
            block.Condition.Accept(this, context);
            foreach (var statement in block.Body)
            {
                statement.Accept(this, context);
            }
        }

        #endregion

        #region statements

        public override void Visit(IAssignment stmt, UsageContext context)
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

        public override void Visit(ILabelledStatement stmt, UsageContext context)
        {
            stmt.Statement.Accept(this, context);
        }

        public override void Visit(IVariableDeclaration stmt, UsageContext context)
        {
            if (!stmt.IsMissing)
            {
                var id = stmt.Reference.Identifier;
                var type = stmt.Type;
                var def = DefinitionSites.CreateUnknownDefinitionSite();
                context.DefineVariable(id, type, def);
            }
        }

        public override void Visit(IExpressionStatement stmt, UsageContext context)
        {
            stmt.Expression.Accept(this, context);
        }

        #endregion

        #region expressions

        public override void Visit(ILoopHeaderBlockExpression expr, UsageContext context)
        {
            Visit(expr.Body, context);
        }

        public override void Visit(ILambdaExpression expr, UsageContext context)
        {
            context.EnterNewLambdaScope();
            Visit(expr.Body, context);
        }

        public override void Visit(IInvocationExpression expr, UsageContext context)
        {
            if (expr.MethodName.IsConstructor)
            {
                return;
            }

            var id = expr.Reference.Identifier;
            var method = expr.MethodName;
            context.RegisterCallsite(id, method);
        }

        public override void Visit(ICompletionExpression expr, UsageContext context)
        {
            var hasNoVarName = expr.VariableReference == null || expr.VariableReference.IsMissing;
            if (hasNoVarName)
            {
                return;
            }

            var varName = expr.VariableReference.Identifier;

            if (!context.NameResolver.IsExisting(varName))
            {
                return;
            }

            context.TargetType = context.NameResolver.GetStaticType(varName);
        }

        #endregion
    }
}