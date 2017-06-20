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

using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.Statements;

namespace KaVE.FeedbackProcessor.EditLocation
{
    public class RelativeEditLocationAnalysis
    {
        private readonly RelativeEditLocationVisitor _visitor = new RelativeEditLocationVisitor();

        public RelativeEditLocation Analyze(ISST sst)
        {
            foreach (var md in sst.Methods)
            {
                var res = new RelativeEditLocation();
                foreach (var stmt in md.Body)
                {
                    stmt.Accept(_visitor, res);
                }

                if (res.HasEditLocation)
                {
                    return res;
                }
            }

            return new RelativeEditLocation();
        }

        private class RelativeEditLocationVisitor : AbstractNodeVisitor<RelativeEditLocation>
        {
            public override void Visit(ICompletionExpression expr, RelativeEditLocation loc)
            {
                loc.Location = loc.Size;
            }

            // other expressions are automatically traversed
            // (e.g., see default impl of lambda expressions in abstract visitor)

            #region statements

            public override void Visit(IAssignment stmt, RelativeEditLocation loc)
            {
                loc.Size++;
                stmt.Expression.Accept(this, loc);
            }

            public override void Visit(IBreakStatement stmt, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            public override void Visit(IContinueStatement stmt, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            public override void Visit(IEventSubscriptionStatement stmt, RelativeEditLocation loc)
            {
                loc.Size++;
                stmt.Expression.Accept(this, loc);
            }

            public override void Visit(IExpressionStatement stmt, RelativeEditLocation loc)
            {
                loc.Size++;
                stmt.Expression.Accept(this, loc);
            }

            public override void Visit(IGotoStatement stmt, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            public override void Visit(ILabelledStatement stmt, RelativeEditLocation loc)
            {
                stmt.Statement.Accept(this, loc);
            }

            public override void Visit(IReturnStatement stmt, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            public override void Visit(IThrowStatement stmt, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            public override void Visit(IUnknownStatement stmt, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            public override void Visit(IVariableDeclaration stmt, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            #endregion

            #region blocks

            public override void Visit(IDoLoop block, RelativeEditLocation loc)
            {
                loc.Size++;
                block.Condition.Accept(this, loc);
                Visit(block.Body, loc);
            }

            public override void Visit(IForEachLoop block, RelativeEditLocation loc)
            {
                loc.Size++;
                Visit(block.Body, loc);
            }

            public override void Visit(IForLoop block, RelativeEditLocation loc)
            {
                loc.Size++;
                Visit(block.Init, loc);
                block.Condition.Accept(this, loc);
                Visit(block.Step, loc);
                Visit(block.Body, loc);
            }

            public override void Visit(IIfElseBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                block.Condition.Accept(this, loc);
                Visit(block.Then, loc);
                Visit(block.Else, loc);
            }

            public override void Visit(ILockBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                Visit(block.Body, loc);
            }

            public override void Visit(ISwitchBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                Visit(block.DefaultSection, loc);
                foreach (var caseBlock in block.Sections)
                {
                    Visit(caseBlock.Body, loc);
                }
            }

            public override void Visit(ITryBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                Visit(block.Body, loc);
                foreach (var catchBlock in block.CatchBlocks)
                {
                    Visit(catchBlock.Body, loc);
                }
                Visit(block.Finally, loc);
            }

            public override void Visit(IUncheckedBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                Visit(block.Body, loc);
            }

            public override void Visit(IUnsafeBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            public override void Visit(IUsingBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                Visit(block.Body, loc);
            }

            public override void Visit(IWhileLoop block, RelativeEditLocation loc)
            {
                loc.Size++;
                block.Condition.Accept(this, loc);
                Visit(block.Body, loc);
            }

            #endregion
        }
    }
}