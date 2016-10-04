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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.Commons.Utils.Naming;

namespace KaVE.RS.Commons.Analysis
{
    public class TypeShapeAnalysis
    {
        private ITypeElement _typeElement;

        public TypeShape Analyze(ITypeDeclaration typeDeclaration)
        {
            return Analyze(typeDeclaration.DeclaredElement);
        }

        public TypeShape Analyze(ITypeElement typeElement)
        {
            _typeElement = typeElement;
            return AnalyzeInternal(typeElement);
        }

        private TypeShape AnalyzeInternal(ITypeElement typeElement)
        {
            var typeShape = new TypeShape();

            AddEventHierarchies(typeShape);

            AddDelegates(typeShape);

            AddFields(typeShape);

            AddMethodHierarchies(typeShape);

            AddNestedTypes(typeShape);

            AddPropertyHierarchies(typeShape);

            typeShape.TypeHierarchy = CreateTypeHierarchy(
                typeElement,
                EmptySubstitution.INSTANCE,
                Lists.NewList<ITypeName>());

            return typeShape;
        }

        private void AddEventHierarchies(ITypeShape typeShape)
        {
            foreach (var m in FindImplementedEventsInType())
            {
                var name = m.GetName<IEventName>();
                var declaration = m.CollectDeclarationInfo<IEventName, EventHierarchy>(name);
                typeShape.EventHierarchies.Add(declaration);
            }
        }

        private void AddDelegates(ITypeShape typeShape)
        {
            var typeMembers = _typeElement.GetMembers();
            var delegateTypeNames =
                typeMembers.OfType<IDelegate>()
                           .Select(d => d.GetName<IDelegateTypeName>());
            typeShape.Delegates.AddAll(delegateTypeNames);
        }

        private void AddFields(ITypeShape typeShape)
        {
            var typeMembers = _typeElement.GetMembers();
            foreach (var typeMember in typeMembers)
            {
                var field = typeMember as IField;
                if (field != null)
                {
                    if (field.IsAutoPropertyBackingField())
                    {
                        continue;
                    }
                    var fieldName = field.GetName<IFieldName>();
                    if (field.IsEnumMember)
                    {
                        var shortName = field.ShortName;
                        fieldName = Names.Field("[{0}] [{0}].{1}", fieldName.DeclaringType, shortName);
                    }
                    typeShape.Fields.Add(fieldName);
                }
            }
        }

        private void AddNestedTypes(ITypeShape typeShape)
        {
            var nestedTypes = _typeElement.NestedTypes;
            foreach (var typeElement in nestedTypes)
            {
                if (typeElement is IDelegate)
                {
                    continue;
                }
                var typeHierarchy = CreateTypeHierarchy(
                    typeElement,
                    EmptySubstitution.INSTANCE,
                    Lists.NewList<ITypeName>());
                typeShape.NestedTypes.Add(typeHierarchy);
            }
        }

        private void AddMethodHierarchies(ITypeShape typeShape)
        {
            foreach (var m in FindImplementedConstructorsInType())
            {
                var name = m.GetName<IMethodName>();
                typeShape.MethodHierarchies.Add(new MethodHierarchy {Element = name});
            }

            foreach (var m in FindImplementedMethodsInType())
            {
                var name = m.GetName<IMethodName>();
                var declaration = m.CollectDeclarationInfo<IMethodName, MethodHierarchy>(name);
                typeShape.MethodHierarchies.Add(declaration);
            }
        }

        private void AddPropertyHierarchies(ITypeShape typeShape)
        {
            foreach (var m in FindImplementedPropertiesInType())
            {
                var name = m.GetName<IPropertyName>();
                var declaration = m.CollectDeclarationInfo<IPropertyName, PropertyHierarchy>(name);
                typeShape.PropertyHierarchies.Add(declaration);
            }
        }

        private IEnumerable<IConstructor> FindImplementedConstructorsInType()
        {
            var ctors = new HashSet<IConstructor>();
            if (_typeElement != null)
            {
                foreach (var ctor in _typeElement.Constructors)
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
            return _typeElement != null ? _typeElement.Methods : new HashSet<IMethod>();
        }

        private IEnumerable<IEvent> FindImplementedEventsInType()
        {
            return _typeElement != null ? _typeElement.Events : new HashSet<IEvent>();
        }

        private IEnumerable<IProperty> FindImplementedPropertiesInType()
        {
            return _typeElement != null ? _typeElement.Properties : new HashSet<IProperty>();
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
                // TODO NameUpdate: "isUnknownOrUnResolvedUntested" required by one analyzed solution, still untested
                var isUnknownOrUnResolvedUntested = superType.IsUnknown || !superType.IsResolved;
                if (!resolveResult.IsValid() || declElem == null || isUnresolvedAlias || isUnknownOrUnResolvedUntested)
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