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

using KaVE.JetBrains.Annotations;

namespace KaVE.Model.Names.CSharp
{
    public class ArrayTypeName : TypeName
    {
        internal static bool IsArrayTypeIdentifier(string identifier)
        {
            return GetRawFullName(identifier).Contains("[");
        }

        [UsedImplicitly]
        internal new static ITypeName Get(string identifier)
        {
            return TypeName.Get(identifier);
        }

        internal ArrayTypeName(string identifier) : base(identifier) {}

        public override bool IsArrayType
        {
            get { return true; }
        }

        public override ITypeName ArrayBaseType
        {
            get
            {
                if (!IsArrayType)
                {
                    return null;
                }

                var fullName = FullName;
                var rawFullName = RawFullName;
                var startOfArrayBraces = rawFullName.IndexOf('[');
                if (startOfArrayBraces > -1)
                {
                    var start = rawFullName.Substring(0, startOfArrayBraces);
                    var end = fullName.Substring(rawFullName.Length);
                    fullName = start + end;
                }
                if (Assembly.Identifier.Length > 0)
                {
                    fullName += ", " + Assembly;
                }
                return TypeName.Get(fullName);
            }
        }
    }
}