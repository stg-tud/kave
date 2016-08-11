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

using KaVE.Commons.Model.Naming.Types;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public static class TypeUtils
    {
        private static readonly ITypeName UnknownTypeInstance = new TypeName();

        public static ITypeName CreateTypeName([NotNull] string identifier)
        {
            // checked first, because it's a special case
            if (IsUnknownTypeIdentifier(identifier))
            {
                return UnknownTypeInstance;
            }
            if (PredefinedTypeName.IsPredefinedTypeNameIdentifier(identifier))
            {
                return new PredefinedTypeName(identifier);
            }
            if (TypeParameterName.IsTypeParameterNameIdentifier(identifier))
            {
                return new TypeParameterName(identifier);
            }
            if (ArrayTypeName.IsArrayTypeNameIdentifier(identifier))
            {
                return new ArrayTypeName(identifier);
            }
            if (DelegateTypeName.IsDelegateTypeNameIdentifier(identifier))
            {
                return new DelegateTypeName(identifier);
            }
            if (TypeName.IsTypeNameIdentifier(identifier))
            {
                return new TypeName(identifier);
            }

            return UnknownTypeInstance;
        }

        public static bool IsUnknownTypeIdentifier([NotNull] string identifier)
        {
            return string.IsNullOrEmpty(identifier) || BaseTypeName.UnknownTypeIdentifier.Equals(identifier);
        }
    }
}