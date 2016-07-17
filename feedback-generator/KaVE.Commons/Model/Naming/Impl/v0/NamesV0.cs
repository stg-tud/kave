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

using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;

namespace KaVE.Commons.Model.Naming.Impl.v0
{
    public class NamesV0
    {
        public static ITypeName Type(string identifier)
        {
            // checked first, because it's a special case
            if (identifier == string.Empty || UnknownTypeName.IsUnknownTypeIdentifier(identifier))
            {
                return UnknownTypeName.Instance;
            }
            // checked second, since type parameters can have any kind of type
            if (TypeParameterName.IsTypeParameterIdentifier(identifier))
            {
                return new TypeParameterName(identifier);
            }
            // checked third, since the array's value type can have any kind of type
            if (ArrayTypeName.IsArrayTypeIdentifier(identifier))
            {
                return new ArrayTypeName(identifier);
            }
            if (InterfaceTypeName.IsInterfaceTypeIdentifier(identifier))
            {
                return new InterfaceTypeName(identifier);
            }
            if (StructTypeName.IsStructTypeIdentifier(identifier))
            {
                return new StructTypeName(identifier);
            }
            if (EnumTypeName.IsEnumTypeIdentifier(identifier))
            {
                return new EnumTypeName(identifier);
            }
            if (DelegateTypeName.IsDelegateTypeIdentifier(identifier))
            {
                return new DelegateTypeName(DelegateTypeName.FixLegacyDelegateNames(identifier));
            }
            return new TypeName(identifier);
        }
    }
}