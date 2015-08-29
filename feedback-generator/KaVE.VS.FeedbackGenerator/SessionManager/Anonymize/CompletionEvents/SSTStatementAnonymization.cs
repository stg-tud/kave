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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Anonymize.CompletionEvents
{
    public class SSTStatementAnonymization : AbstractNodeVisitor<int, IStatement>
    {
        private readonly SSTExpressionAnonymization _expr;
        private readonly SSTReferenceAnonymization _ref;

        public SSTStatementAnonymization(SSTExpressionAnonymization exprAnon, SSTReferenceAnonymization refAnon)
        {
            _expr = exprAnon;
            _ref = refAnon;
        }

        #region blocks

        public override IStatement Visit(IDoLoop stmt, int context)
        {
            return new DoLoop
            {
                Condition = Anonymize(stmt.Condition),
                Body = Anonymize(stmt.Body)
            };
        }

        public override IStatement Visit(IForEachLoop stmt, int context)
        {
            return new ForEachLoop
            {
                Declaration = _ref.Anonymize(stmt.Declaration),
                LoopedReference = _ref.Anonymize(stmt.LoopedReference),
                Body = Anonymize(stmt.Body)
            };
        }

        public override IStatement Visit(IForLoop stmt, int context)
        {
            return new ForLoop
            {
                Init = Anonymize(stmt.Init),
                Condition = Anonymize(stmt.Condition),
                Step = Anonymize(stmt.Step),
                Body = Anonymize(stmt.Body)
            };
        }

        public override IStatement Visit(IIfElseBlock stmt, int context)
        {
            return new IfElseBlock
            {
                Condition = Anonymize(stmt.Condition),
                Then = Anonymize(stmt.Then),
                Else = Anonymize(stmt.Else)
            };
        }

        public override IStatement Visit(ILockBlock stmt, int context)
        {
            return new LockBlock
            {
                Reference = _ref.Anonymize(stmt.Reference),
                Body = Anonymize(stmt.Body)
            };
        }

        public override IStatement Visit(ISwitchBlock stmt, int context)
        {
            return new SwitchBlock
            {
                Reference = _ref.Anonymize(stmt.Reference),
                Sections = Anonymize(stmt.Sections),
                DefaultSection = Anonymize(stmt.DefaultSection)
            };
        }

        private IKaVEList<ICaseBlock> Anonymize(IEnumerable<ICaseBlock> caseBlocks)
        {
            return Lists.NewListFrom(caseBlocks.Select(Anonymize));
        }

        private ICaseBlock Anonymize(ICaseBlock caseBlock)
        {
            return new CaseBlock
            {
                Label = caseBlock.Label,
                Body = Anonymize(caseBlock.Body)
            };
        }

        public override IStatement Visit(ITryBlock stmt, int context)
        {
            return new TryBlock
            {
                Body = Anonymize(stmt.Body),
                CatchBlocks = Anonymize(stmt.CatchBlocks),
                Finally = Anonymize(stmt.Finally)
            };
        }

        private IKaVEList<ICatchBlock> Anonymize(IEnumerable<ICatchBlock> catchBlocks)
        {
            return Lists.NewListFrom(catchBlocks.Select(Anonymize));
        }

        private ICatchBlock Anonymize(ICatchBlock stmt)
        {
            return new CatchBlock
            {
                Parameter = stmt.Parameter.ToAnonymousName(),
                Body = Anonymize(stmt.Body)
            };
        }

        public override IStatement Visit(IUncheckedBlock stmt, int context)
        {
            return new UncheckedBlock
            {
                Body = Anonymize(stmt.Body)
            };
        }

        public override IStatement Visit(IUnsafeBlock stmt, int context)
        {
            return new UnsafeBlock();
        }

        public override IStatement Visit(IUsingBlock stmt, int context)
        {
            return new UsingBlock
            {
                Reference = _ref.Anonymize(stmt.Reference),
                Body = Anonymize(stmt.Body)
            };
        }

        public override IStatement Visit(IWhileLoop stmt, int context)
        {
            return new WhileLoop
            {
                Condition = Anonymize(stmt.Condition),
                Body = Anonymize(stmt.Body)
            };
        }

        #endregion

        #region statements

        public override IStatement Visit(IAssignment stmt, int context)
        {
            return new Assignment
            {
                Reference = _ref.Anonymize(stmt.Reference),
                Expression = Anonymize(stmt.Expression)
            };
        }

        public override IStatement Visit(IBreakStatement stmt, int context)
        {
            return new BreakStatement();
        }

        public override IStatement Visit(IContinueStatement stmt, int context)
        {
            return new ContinueStatement();
        }

        public override IStatement Visit(IEventSubscriptionStatement stmt, int context)
        {
            return new EventSubscriptionStatement
            {
                Reference = _ref.Anonymize(stmt.Reference),
                Operation = stmt.Operation,
                Expression = Anonymize(stmt.Expression)
            };
        }

        public override IStatement Visit(IExpressionStatement stmt, int context)
        {
            return new ExpressionStatement
            {
                Expression = Anonymize(stmt.Expression)
            };
        }

        public override IStatement Visit(IGotoStatement stmt, int context)
        {
            return new GotoStatement {Label = stmt.Label};
        }

        public override IStatement Visit(ILabelledStatement stmt, int context)
        {
            return new LabelledStatement
            {
                Label = stmt.Label,
                Statement = Anonymize(stmt.Statement)
            };
        }

        public override IStatement Visit(IReturnStatement stmt, int context)
        {
            return new ReturnStatement
            {
                Expression = Anonymize(stmt.Expression)
            };
        }

        public override IStatement Visit(IThrowStatement stmt, int context)
        {
            return new ThrowStatement
            {
                Exception = stmt.Exception.ToAnonymousName()
            };
        }

        public override IStatement Visit(IVariableDeclaration stmt, int context)
        {
            return new VariableDeclaration
            {
                Reference = _ref.Anonymize(stmt.Reference)
            };
        }

        public override IStatement Visit(IUnknownStatement stmt, int context)
        {
            return new UnknownStatement();
        }

        #endregion

        public virtual ISimpleExpression Anonymize([NotNull] ISimpleExpression expr)
        {
            return (ISimpleExpression) expr.Accept(_expr, 0);
        }

        private ILoopHeaderExpression Anonymize(ILoopHeaderExpression expr)
        {
            var block = expr as ILoopHeaderBlockExpression;
            if (block != null)
            {
                return new LoopHeaderBlockExpression
                {
                    Body = Anonymize(block.Body)
                };
            }
            return expr == null ? null : (ILoopHeaderExpression) expr.Accept(_expr, 0);
        }

        private IAssignableExpression Anonymize([NotNull] IAssignableExpression expr)
        {
            var lambda = expr as ILambdaExpression;
            if (lambda != null)
            {
                return new LambdaExpression
                {
                    Name = lambda.Name.ToAnonymousName(),
                    Body = Anonymize(lambda.Body)
                };
            }
            return (IAssignableExpression) expr.Accept(_expr, 0);
        }

        public IKaVEList<IStatement> Anonymize(IEnumerable<IStatement> body)
        {
            return Lists.NewListFrom(body.Select(Anonymize));
        }

        private IStatement Anonymize([NotNull] IStatement stmt)
        {
            return stmt.Accept(this, 0);
        }
    }
}