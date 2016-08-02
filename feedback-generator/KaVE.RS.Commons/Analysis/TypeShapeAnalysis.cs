/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections.Generic;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.Commons.Utils.Naming;

namespace KaVE.RS.Commons.Analysis
{
    internal class TypeShapeAnalysis
    {
        private ITypeDeclaration _typeDeclaration;

        public TypeShape Analyze(ITypeDeclaration typeDeclaration)
        {
            _typeDeclaration = typeDeclaration;

            var typeShape = new TypeShape();

            foreach (var m in FindImplementedConstructorsInType())
            {
                var name = m.GetName<IMethodName>();
                typeShape.MethodHierarchies.Add(new MethodHierarchy {Element = name});
            }

            foreach (var m in FindImplementedMethodsInType())
            {
                var name = m.GetName<IMethodName>();
                var declaration = m.CollectDeclarationInfo(name);
                typeShape.MethodHierarchies.Add(declaration);
            }

            typeShape.TypeHierarchy = CreateTypeHierarchy(
                typeDeclaration.DeclaredElement,
                EmptySubstitution.INSTANCE,
                Lists.NewList<ITypeName>());

            return typeShape;
        }

        private IEnumerable<IConstructor> FindImplementedConstructorsInType()
        {
            var ctors = new HashSet<IConstructor>();
            if (_typeDeclaration != null && _typeDeclaration.DeclaredElement != null)
            {
                foreach (var ctor in _typeDeclaration.DeclaredElement.Constructors)
                {
                    if (!ctor.IsImplicit)
                    {
                        ctors.Add(ctor);
                    }
                }
            }
            return ctors;
        }

        private IEnumerable<IMethod> FindImplementedMethodsInType()
        {
            if (_typeDeclaration != null && _typeDeclaration.DeclaredElement != null)
            {
                return _typeDeclaration.DeclaredElement.Methods;
            }
            return new HashSet<IMethod>();
        }


        private static TypeHierarchy CreateTypeHierarchy(ITypeElement type,
            ISubstitution substitution,
            IKaVEList<ITypeName> seenTypes)
        {
            if (type == null || IsRootType(type))
            {
                return null;
            }
            var typeName = type.GetName<ITypeName>(substitution);
            seenTypes.Add(typeName);
            var enclosingClassHierarchy = new TypeHierarchy(typeName.Identifier);
            foreach (var superType in type.GetSuperTypes())
            {
                var resolveResult = superType.Resolve();
                var declElem = resolveResult.DeclaredElement;
                var isUnresolvedAlias = declElem is IUsingAliasDirective;
                if (!resolveResult.IsValid() || declElem == null || isUnresolvedAlias)
                {
                    enclosingClassHierarchy.Implements.Add(new TypeHierarchy());
                    continue;
                }

                var superName = declElem.GetName<ITypeName>(substitution);
                if (seenTypes.Contains(superName))
                {
                    continue;
                }

                var superTypeElement = superType.GetTypeElement();
                var superTypeSubstitution = superType.GetSubstitution();
                var superHierarchy = CreateTypeHierarchy(superTypeElement, superTypeSubstitution, seenTypes);

                if (declElem is IClass || declElem is IStruct)
                {
                    enclosingClassHierarchy.Extends = superHierarchy;
                }
                else if (declElem is IInterface)
                {
                    enclosingClassHierarchy.Implements.Add(superHierarchy);
                }
            }
            return enclosingClassHierarchy;
        }

        private static bool IsRootType(ITypeElement type)
        {
            var fn = type.GetClrName().FullName;
            return "System.Object".Equals(fn) || "System.ValueType".Equals(fn) || "System.Enum".Equals(fn);
        }
    }
}