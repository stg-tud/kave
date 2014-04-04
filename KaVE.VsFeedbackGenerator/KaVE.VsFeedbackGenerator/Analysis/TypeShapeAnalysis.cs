using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal class TypeShapeAnalysis
    {
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
                    var declaration = m.CollectDeclarationInfo(name);
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