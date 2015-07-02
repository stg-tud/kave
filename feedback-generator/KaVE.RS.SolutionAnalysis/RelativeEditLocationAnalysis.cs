using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Blocks;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils.Collections;

namespace KaVE.RS.SolutionAnalysis
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

            public override void Visit(ILoopHeaderBlockExpression expr, RelativeEditLocation loc)
            {
                VisitBody(expr.Body, loc);
            }

            private void VisitBody(IKaVEList<IStatement> body, RelativeEditLocation loc)
            {
                foreach (var stmt in body)
                {
                    stmt.Accept(this, loc);
                }
            }

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

            #endregion

            #region blocks

            public override void Visit(IDoLoop block, RelativeEditLocation loc)
            {
                loc.Size++;
                block.Condition.Accept(this, loc);
                VisitBody(block.Body, loc);
            }

            public override void Visit(IForEachLoop block, RelativeEditLocation loc)
            {
                loc.Size++;
                VisitBody(block.Body, loc);
            }

            public override void Visit(IForLoop block, RelativeEditLocation loc)
            {
                loc.Size++;
                VisitBody(block.Init, loc);
                block.Condition.Accept(this, loc);
                VisitBody(block.Step, loc);
                VisitBody(block.Body, loc);
            }

            public override void Visit(IIfElseBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                block.Condition.Accept(this, loc);
                VisitBody(block.Then, loc);
                VisitBody(block.Else, loc);
            }

            public override void Visit(ILockBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                VisitBody(block.Body, loc);
            }

            public override void Visit(ISwitchBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                VisitBody(block.DefaultSection, loc);
                foreach (var caseBlock in block.Sections)
                {
                    VisitBody(caseBlock.Body, loc);
                }
            }

            public override void Visit(ITryBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                VisitBody(block.Body, loc);
                foreach (var catchBlock in block.CatchBlocks)
                {
                    VisitBody(catchBlock.Body, loc);
                }
                VisitBody(block.Finally, loc);
            }

            public override void Visit(IUncheckedBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                VisitBody(block.Body, loc);
            }

            public override void Visit(IUnsafeBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
            }

            public override void Visit(IUsingBlock block, RelativeEditLocation loc)
            {
                loc.Size++;
                VisitBody(block.Body, loc);
            }

            public override void Visit(IWhileLoop block, RelativeEditLocation loc)
            {
                loc.Size++;
                block.Condition.Accept(this, loc);
                VisitBody(block.Body, loc);
            }

            #endregion
        }
    }
}