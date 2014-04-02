using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class ContextAnalysis
    {
        private ITreeNode _nodeInFile;
        private Context _context;

        private readonly TypeShapeAnalysis _typeShapeAnalysis = new TypeShapeAnalysis();
        private readonly CalledMethodsAnalysis _calledMethodsAnalysis = new CalledMethodsAnalysis();

        public static Context Analyze(CSharpCodeCompletionContext rsContext)
        {
            return new ContextAnalysis().AnalyzeInternal(rsContext);
        }

        private Context AnalyzeInternal(CSharpCodeCompletionContext rsContext)
        {
            _nodeInFile = rsContext.NodeInFile;
            _context = new Context();

            var typeDeclaration = FindEnclosing<ITypeDeclaration>(_nodeInFile);
            if (typeDeclaration != null)
            {
                _context.TypeShape = _typeShapeAnalysis.Analyze(typeDeclaration);
                _context.TriggerTarget = AnalyzeTargetType(_nodeInFile);

                var methodDeclaration = FindEnclosing<IMethodDeclaration>(_nodeInFile);
                if (methodDeclaration != null)
                {
                    _context.EnclosingMethod = GetName(methodDeclaration);

                    _context.CalledMethods = _calledMethodsAnalysis.Analyze(
                        methodDeclaration,
                        _context.TypeShape.TypeHierarchy.Element);
                }
            }
            return _context;
        }

        private static IName AnalyzeTargetType(ITreeNode targetNode)
        {
            if (targetNode == null)
            {
                return null;
            }
            var tokenNodeType = targetNode.GetTokenType();
            if ((tokenNodeType != null && (tokenNodeType.IsWhitespace || tokenNodeType == CSharpTokenType.DOT || tokenNodeType.IsIdentifier)) || targetNode is IErrorElement)
            {
                return AnalyzeTargetType(targetNode.PrevSibling);
            }
            if (tokenNodeType == CSharpTokenType.SEMICOLON)
            {
                return null;
            }
            var statement = targetNode as IExpressionStatement;
            if (statement != null && statement.Semicolon == null)
            {
                return AnalyzeTargetType(statement.LastChild);
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
                    return AnalyzeTargetType(expression.LastChild);
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
            if (resolvedReference.IsValid())
            {
                var result = resolvedReference.Result;
                return result.DeclaredElement.GetName(result.Substitution);
            }
            return null;
        }

        [CanBeNull]
        private static TIDeclaration FindEnclosing<TIDeclaration>(ITreeNode node)
            where TIDeclaration : class, IDeclaration
        {
            while (node != null)
            {
                var declaration = node as TIDeclaration;
                if (declaration != null)
                {
                    return declaration;
                }
                node = node.Parent;
            }
            return null;
        }


        // TODO move this to a generic helper in the name factory
        private static IMethodName GetName(IMethodDeclaration methodDeclaration)
        {
            var declaredElement = methodDeclaration.DeclaredElement;
            return declaredElement.GetName<IMethodName>();
        }
    }
}