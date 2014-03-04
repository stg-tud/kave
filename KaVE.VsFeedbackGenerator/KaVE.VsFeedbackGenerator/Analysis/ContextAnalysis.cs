using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class ContextAnalysis
    {
        private ITreeNode _nodeInFile;
        private Context _context;

        private IMethodDeclaration _methodDeclaration;
        private ITypeDeclaration _typeDeclaration;

        // TODO review: this class *NEEDS* to be refactored!
        // TODO discuss is lock necessary?
        //lock (GetType()) {}
        public Context Analyze(CSharpCodeCompletionContext rsContext)
        {
            _nodeInFile = rsContext.NodeInFile;
            _context = new Context();

            FindEnclMethodDeclaration();
            FindEnclTypeDeclaration();


            if (_methodDeclaration != null)
            {
                var methodName = GetName(_methodDeclaration);
                _context.EnclosingMethodDeclaration = new MethodDeclaration(methodName);
                CollectDeclarationInfo(
                    _context.EnclosingMethodDeclaration,
                    _methodDeclaration.DeclaredElement,
                    _typeDeclaration.DeclaredElement);
            }

            if (_typeDeclaration != null)
            {
                foreach (var m in FindImplementedMethodsInType())
                {
                    var name = m.GetName() as IMethodName;
                    if (name != null)
                    {
                        var declaration = new MethodDeclaration(name);
                        CollectDeclarationInfo(declaration, m, _typeDeclaration.DeclaredElement);
                        _context.TypeShapeMethods.Add(declaration);
                    }
                }

                _context.EnclosingClassHierarchy = CreateTypeHierarchy(
                    _typeDeclaration.DeclaredElement,
                    EmptySubstitution.INSTANCE);
            }

            if (_methodDeclaration != null && _typeDeclaration != null)
            {
                _context.CalledMethods = FindAllCalledMethods(
                    _methodDeclaration,
                    _context.EnclosingClassHierarchy.Element);
            }
            return _context;
        }

        private IEnumerable<IMethod> FindImplementedMethodsInType()
        {
            if (_typeDeclaration != null && _typeDeclaration.DeclaredElement != null)
            {
                return _typeDeclaration.DeclaredElement.Methods;
            }
            return new HashSet<IMethod>();
        }

        private void FindEnclMethodDeclaration()
        {
            _methodDeclaration = FindEnclosing<IMethodDeclaration>(_nodeInFile);
        }

        private void FindEnclTypeDeclaration()
        {
            if (_methodDeclaration != null)
            {
                _typeDeclaration = _methodDeclaration.GetContainingTypeDeclaration();
            }
            else
            {
                _typeDeclaration = FindEnclosing<ITypeDeclaration>(_nodeInFile);
            }
        }

        private void CollectDeclarationInfo(MethodDeclaration decl,
            IMethod declaredElement,
            ITypeElement typeElement)
        {
            decl.Super = FindMethodInSuperTypes(
                declaredElement,
                typeElement);
            decl.First = FindFirstMethodInSuperTypes(
                declaredElement,
                typeElement);

            if (decl.First != null)
            {
                if (decl.First.Equals(decl.Super))
                {
                    decl.First = null;
                }
            }
        }

        private IMethodName FindFirstMethodInSuperTypes(IMethod enclosingMethod,
            ITypeElement typeDeclaration)
        {
            // TODO implement GetSignature for MethodName
            var encName = GetSimpleName(enclosingMethod);

            foreach (var superType in typeDeclaration.GetSuperTypes())
            {
                var superTypeElement = superType.GetTypeElement();
                Asserts.NotNull(superTypeElement);

                if (superType.IsInterfaceType() || enclosingMethod.IsOverride)
                {
                    var superName = FindFirstMethodInSuperTypes(enclosingMethod, superType.GetTypeElement());
                    if (superName != null)
                    {
                        return superName;
                    }
                }
            }

            foreach (var method in typeDeclaration.Methods)
            {
                var name = GetSimpleName(method);

                if (name.Equals(encName))
                {
                    if (method.Equals(enclosingMethod))
                    {
                        return null;
                    }
                    return (IMethodName) method.GetName(method.IdSubstitution);
                }
            }

            return null;
        }

        private IMethodName FindMethodInSuperTypes(IMethod enclosingMethod,
            ITypeElement typeDeclaration)
        {
            if (!enclosingMethod.IsOverride)
            {
                return null;
            }

            var encName = GetSimpleName(enclosingMethod);

            foreach (var superType in typeDeclaration.GetSuperTypes())
            {
                var superTypeElement = superType.GetTypeElement();
                Asserts.NotNull(superTypeElement);

                if (superType.IsClassType())
                {
                    foreach (var method in superTypeElement.Methods)
                    {
                        if (!method.IsAbstract)
                        {
                            var name = GetSimpleName(method);

                            if (name.Equals(encName))
                            {
                                return (IMethodName) method.GetName(method.IdSubstitution);
                            }
                        }
                    }

                    var superName = FindMethodInSuperTypes(enclosingMethod, superType.GetTypeElement());
                    if (superName != null)
                    {
                        return superName;
                    }
                }
            }

            return null;
        }

        // TODO discuss
        private string GetSimpleName(IMethod method)
        {
            var name = method.ShortName;
            var ps = string.Join(",", method.Parameters.Select(p => p.Type));
            var ret = method.ReturnType;
            return ret + " " + name + "(" + ps + ")";
        }

        private ISet<IMethodName> FindAllCalledMethods(IMethodDeclaration methodDeclaration, ITypeName enclosingType)
        {
            return MethodInvocationCollector.FindCalledMethodsIn(methodDeclaration, enclosingType);
        }

        private static TypeHierarchy CreateTypeHierarchy(ITypeElement type, ISubstitution substitution)
        {
            if (type == null || HasTypeSystemObject(type))
            {
                return null;
            }
            var typeName = type.GetName(substitution);
            var enclosingClassHierarchy = new TypeHierarchy(typeName.Identifier);

            foreach (var superType in type.GetSuperTypes())
            {
                var resolvedSuperType = superType.Resolve().DeclaredElement;

                var superTypeSubstitution = superType.GetSubstitution();

                var aClass = resolvedSuperType as IClass;
                if (aClass != null)
                {
                    enclosingClassHierarchy.Extends = CreateTypeHierarchy(aClass, superTypeSubstitution);
                }
                var anInterface = resolvedSuperType as IInterface;
                if (anInterface != null)
                {
                    enclosingClassHierarchy.Implements.Add(CreateTypeHierarchy(anInterface, superTypeSubstitution));
                }
            }
            return enclosingClassHierarchy;
        }

        private static bool HasTypeSystemObject(ITypeElement type)
        {
            return "System.Object".Equals(type.GetClrName().FullName);
        }


        private static IMethodName GetName(IMethodDeclaration methodDeclaration)
        {
            var declaredElement = methodDeclaration.DeclaredElement;
            return declaredElement.GetName<IMethodName>();
        }

        [CanBeNull]
        private static TIDeclaration FindEnclosing<TIDeclaration>(ITreeNode node)
            where TIDeclaration : class, IDeclaration
        {
            if (node == null)
            {
                return null;
            }

            var methodDeclaration = node as TIDeclaration;
            return methodDeclaration ?? FindEnclosing<TIDeclaration>(node.Parent);
        }
    }
}