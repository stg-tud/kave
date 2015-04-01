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

using System;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    internal static class TypeHierarchyAnalysisUtils
    {
        public static MethodHierarchy CollectDeclarationInfo([NotNull] this IMethod method, [NotNull] IMethodName name)
        {
            var decl = new MethodHierarchy(name);
            var declaringType = method.GetContainingType();
            decl.Super = FindMethodInSuperTypes(method, declaringType);
            decl.First = FindFirstMethodInSuperTypes(method, declaringType);

            if (decl.First != null && decl.First.Equals(decl.Super))
            {
                decl.First = null;
            }
            return decl;
        }

        public static IMethodName FindFirstMethodInSuperTypes([NotNull] this IMethod method)
        {
            var containingType = method.GetContainingType();
            return FindFirstMethodInSuperTypes(method, containingType);
        }

        private static IMethodName FindFirstMethodInSuperTypes(IMethod enclosingMethod,
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

        private static IMethodName FindMethodInSuperTypes(IMethod enclosingMethod,
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
            var ps = String.Join(",", method.Parameters.Select(p => p.Type));
            var ret = method.ReturnType;
            return ret + " " + name + "(" + ps + ")";
        }
    }
}