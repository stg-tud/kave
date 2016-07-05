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

using JetBrains.Annotations;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.RS.Commons.Utils.Naming
{
    public static class ReSharperDeclarationNameFactory
    {
        [NotNull]
        public static IMethodName GetName([NotNull] this IMethodDeclaration methodDeclaration)
        {
            var declaredElement = methodDeclaration.DeclaredElement;
            Asserts.NotNull(declaredElement, "no declared element in declaration");
            return declaredElement.GetName<IMethodName>();
        }

        [NotNull]
        public static IMethodName GetName([NotNull] this IConstructorDeclaration constructorDeclaration)
        {
            // TODO testing!
            var declaredElement = constructorDeclaration.DeclaredElement;
            Asserts.NotNull(declaredElement, "no declared element in declaration");
            return declaredElement.GetName<IMethodName>();
        }
    }
}