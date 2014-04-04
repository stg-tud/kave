using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class TypeShapeAnalysis {
        private ITypeDeclaration _typeDeclaration;

        public TypeShape Analyze(ITypeDeclaration typeDeclaration)
        {
            _typeDeclaration = typeDeclaration;

            var typeShape = new TypeShape();
            foreach (var m in FindImplementedMethodsInType())
            {
                var name = m.GetName<IMethodName>();
                if (name != null)
                {
                    var declaration = new MethodHierarchy(name);
                    CollectDeclarationInfo(declaration, m, typeDeclaration.DeclaredElement);
                    typeShape.MethodHierarchies.Add(declaration);
                }
            }

            typeShape.TypeHierarchy = CreateTypeHierarchy(
                typeDeclaration.DeclaredElement,
                EmptySubstitution.INSTANCE);

            return typeShape;
        }

        private IEnumerable<IMethod> FindImplementedMethodsInType()
        {
            if (_typeDeclaration != null && _typeDeclaration.DeclaredElement != null)
            {
                return _typeDeclaration.DeclaredElement.Methods;
            }
            return new HashSet<IMethod>();
        }

        private void CollectDeclarationInfo(MethodHierarchy decl,
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
            // TODO use MethodName.Signature
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

        private static string GetSimpleName(IMethod method)
        {
            var name = method.ShortName;
            var ps = string.Join(",", method.Parameters.Select(p => p.Type));
            var ret = method.ReturnType;
            return ret + " " + name + "(" + ps + ")";
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
    }
}