using System.Text;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;

namespace KaVE.Commons.Model.SSTs.Impl.Visitor
{
    internal class CompletionPrefixVisitor : AbstractNodeVisitor<StringBuilder>
    {
        public override void Visit(ICompletionExpression entity, StringBuilder context)
        {
            context.Append(entity.Token);
        }
    }
}