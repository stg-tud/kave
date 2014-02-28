using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    class MethodInvocationCollector : TreeNodeVisitor<ISet<IMethodName>>
    {
        private readonly ITypeName _enclosingType;

        public MethodInvocationCollector(ITypeName enclosingType)
        {
            _enclosingType = enclosingType;
        }

        public override void VisitInvocationExpression(IInvocationExpression invocation, ISet<IMethodName> context)
        {
            var invocationRef = invocation.Reference;
            if (invocationRef != null)
            {
                var resolvedRef = invocationRef.Resolve();
                var declaredElement = resolvedRef.DeclaredElement;
                var method = (IMethod) declaredElement;
                var methodName = method.GetName<IMethodName>(resolvedRef.Result.Substitution);
                if (IsLocalHelper(methodName))
                {
                    var declaration = (IMethodDeclaration) method.GetDeclarations().First();
                    declaration.Body.Accept(this, context);
                }
                else
                {
                    context.Add(methodName);
                }
            }
            base.VisitInvocationExpression(invocation, context);
        }

        private bool IsLocalHelper(IMemberName method)
        {
            return _enclosingType == method.DeclaringType;
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