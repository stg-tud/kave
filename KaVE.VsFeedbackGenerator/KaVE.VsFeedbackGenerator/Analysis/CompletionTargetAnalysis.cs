using JetBrains.ReSharper.Psi.CSharp.Parsing;
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

        // TODO decide which implementation is cleaner
        // ReSharper disable once UnusedMember.Local
        private IName Analyze2(ITreeNode targetNode) {
            if (targetNode == null)
            {
                return null;
            }
            var tokenNodeType = targetNode.GetTokenType();
            if ((tokenNodeType != null && (tokenNodeType.IsWhitespace || tokenNodeType == CSharpTokenType.DOT || tokenNodeType.IsIdentifier)) || targetNode is IErrorElement)
            {
                return Analyze(targetNode.PrevSibling);
            }
            var statement = targetNode as IExpressionStatement;
            if (statement != null && statement.Semicolon == null)
            {
                return Analyze(statement.LastChild);
            }
            var predefinedTypeExpression = targetNode as IPredefinedTypeExpression;
            if (predefinedTypeExpression != null)
            {
                var predefinedTypeReference = predefinedTypeExpression.PredefinedTypeName;
                return GetName(predefinedTypeReference.Reference);
            }
            var expression = targetNode as IExpression;
            if (expression != null)
            {
                if (expression.LastChild is IErrorElement)
                {
                    return Analyze(expression.LastChild);
                }
                var type = expression.Type();
                if (!type.IsUnknown)
                {
                    return type.GetName();
                }
                var referenceExpression = expression as IReferenceExpression;
                if (referenceExpression != null)
                {
                    return GetName(referenceExpression.Reference);
                }
                return type.GetName();
            }
            return null;
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
