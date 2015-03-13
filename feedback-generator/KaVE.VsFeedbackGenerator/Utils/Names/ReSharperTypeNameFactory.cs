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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using JetBrains.ReSharper.Psi;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp.TypeNames;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils.Names
{
    public static class ReSharperTypeNameFactory
    {
        [NotNull]
        public static ITypeName GetName(this IType type)
        {
            return IfTypeIs<IDeclaredType>(type, GetName) ??
                   IfTypeIs<IArrayType>(type, GetName) ??
                   IfTypeIs<IAnonymousType>(type, GetName) ??
                   IfTypeIs<IMultitype>(type, GetName) ??
                   IfTypeIs<IPointerType>(type, GetName) ??
                   Asserts.Fail<ITypeName>("unknown IType case: {0}", type.GetType());
        }

        private static ITypeName IfTypeIs<TE>(IType element, Func<TE, ITypeName> map) where TE : class, IType
        {
            var specificElement = element as TE;
            return specificElement != null ? map(specificElement) : null;
        }

        [NotNull]
        private static ITypeName GetName(this IDeclaredType type)
        {
            var typeElement = type.GetTypeElement();
            // typeElement can be null, for example when resolving the second
            // parameter type in the incomplete method declaration
            // > public void M(int i, )
            return typeElement == null
                ? UnknownTypeName.Instance
                : (ITypeName) typeElement.GetName(type.GetSubstitution());
        }

        [NotNull]
        private static ITypeName GetName(this IArrayType arrayType)
        {
            return arrayType.ElementType.GetName().DeriveArrayTypeName(arrayType.Rank);
        }

        [NotNull]
        private static ITypeName GetName(this IAnonymousType type)
        {
            Asserts.Fail("cannot create name for anonymous type");
            return null;
        }

        [NotNull]
        private static ITypeName GetName(this IMultitype type)
        {
            Asserts.Fail("cannot create name for multitype");
            return null;
        }

        [NotNull]
        private static ITypeName GetName(this IPointerType type)
        {
            return type.ElementType.GetName();
        }
    }
}