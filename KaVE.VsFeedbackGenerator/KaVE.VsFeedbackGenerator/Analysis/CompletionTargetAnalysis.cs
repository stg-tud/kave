using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class CompletionTargetAnalysis
    {
        public IName Analyze(ITreeNode targetNode)
        {
            var finder = new TargetFinder();
            ((ICSharpTreeNode) targetNode).Accept(finder);
            return finder.Result;
        }

        private static IName GetName(IReference reference)
        {
            var resolvedReference = reference.Resolve();
            var result = resolvedReference.Result;
            var declaredElement = result.DeclaredElement;
            return declaredElement != null ? declaredElement.GetName(result.Substitution) : null;
        }

        private class TargetFinder : TreeNodeVisitor
        {
            public IName Result { get; private set; }

            public override void VisitNode(ITreeNode node)
            {
                var prevSibling = node.PrevSibling;
                var cSharpTreeNode = prevSibling as ICSharpTreeNode;
                if (cSharpTreeNode != null)
                {
                    cSharpTreeNode.Accept(this);
                }
                else if (prevSibling != null)
                {
                    VisitNode(prevSibling);
                }
            }

            public override void VisitExpressionStatement(IExpressionStatement expressionStatementParam)
            {
                if (expressionStatementParam.Semicolon == null)
                {
                    var lastChild = expressionStatementParam.LastChild;
                    var lastChildNode = lastChild as ICSharpTreeNode;
                    if (lastChildNode != null)
                    {
                        lastChildNode.Accept(this);
                    }
                    var errorElement = lastChild as IErrorElement;
                    if (errorElement != null)
                    {
                        VisitNode(errorElement);
                    }
                }
            }

            public override void VisitPredefinedTypeExpression(IPredefinedTypeExpression predefinedTypeExpressionParam)
            {
                predefinedTypeExpressionParam.PredefinedTypeName.Accept(this);
            }

            public override void VisitPredefinedTypeReference(IPredefinedTypeReference predefinedTypeReferenceParam)
            {
                Result = GetName(predefinedTypeReferenceParam.Reference);
            }

            public override void VisitReferenceExpression(IReferenceExpression referenceExpressionParam)
            {
                var errorElement = referenceExpressionParam.LastChild as IErrorElement;
                if (errorElement != null)
                {
                    VisitNode(errorElement);
                }
                else
                {
                    Result = GetExpressionType(referenceExpressionParam) ?? GetName(referenceExpressionParam.Reference);
                }
            }

            public override void VisitInvocationExpression(IInvocationExpression invocationExpressionParam)
            {
                VisitCSharpExpression(invocationExpressionParam);
            }

            public override void VisitCSharpExpression(ICSharpExpression cSharpExpressionParam)
            {
                Result = GetExpressionType(cSharpExpressionParam);
            }

            private ITypeName GetExpressionType(ICSharpExpression cSharpExpressionParam)
            {
                var type = cSharpExpressionParam.Type();
                if (!type.IsUnknown)
                {
                    return type.GetName();
                }
                return null;
            }
        }
    }
}
