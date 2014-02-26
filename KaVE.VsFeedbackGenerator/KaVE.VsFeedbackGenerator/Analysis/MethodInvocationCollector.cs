using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    class MethodInvocationCollector : TreeNodeVisitor<ISet<IMethodName>>
    {
        public override void VisitInvocationExpression(IInvocationExpression invocation, ISet<IMethodName> context)
        {
            var invocationRef = invocation.Reference;
            if (invocationRef != null)
            {
                var declaredElement = invocationRef.Resolve().DeclaredElement;
                context.Add((IMethodName) declaredElement.GetName(EmptySubstitution.INSTANCE));
            }
            base.VisitInvocationExpression(invocation, context);
        }

        public override void VisitNode(ITreeNode node, ISet<IMethodName> context)
        {
            foreach (var childNode in node.Children<ICSharpTreeNode>())
            {
                childNode.Accept(this, context);
            }
        }
    }
}