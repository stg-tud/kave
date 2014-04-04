using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class ContextAnalysis
    {
        private ITreeNode _nodeInFile;
        private Context _context;

        private readonly TypeShapeAnalysis _typeShapeAnalysis = new TypeShapeAnalysis();
        private readonly CalledMethodsAnalysis _calledMethodsAnalysis = new CalledMethodsAnalysis();
        private readonly CompletionTargetAnalysis _completionTargetAnalysis = new CompletionTargetAnalysis();

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
                _context.TriggerTarget = _completionTargetAnalysis.Analyze(_nodeInFile);

                var methodDeclaration = FindEnclosing<IMethodDeclaration>(_nodeInFile);
                if (methodDeclaration != null)
                {
                    _context.EnclosingMethod = methodDeclaration.GetName();

                    _context.CalledMethods = _calledMethodsAnalysis.Analyze(
                        methodDeclaration,
                        _context.TypeShape.TypeHierarchy.Element);
                }
            }
            return _context;
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
    }
}