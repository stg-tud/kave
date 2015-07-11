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

using System.Linq;
using System.Text.RegularExpressions;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Names.CSharp
{
    public class ArrayTypeName : TypeName
    {
        private static readonly Regex ArrayTypeNameSuffix = new Regex("(\\[[,]*\\])([^()]*)$");

        internal static bool IsArrayTypeIdentifier(string identifier)
        {
            return ArrayTypeNameSuffix.IsMatch(identifier);
        }

        /// <summary>
        ///     Derives an array-type name from this type name.
        /// </summary>
        /// <param name="baseType">The array's base type</param>
        /// <param name="rank">the rank of the array; must be greater than 0</param>
        public static ArrayTypeName From(ITypeName baseType, int rank)
        {
            Asserts.That(rank > 0, "rank smaller than 1");

            return (ArrayTypeName) Get(DeriveArrayTypeNameIdentifier(baseType, rank));
        }

        private static string DeriveArrayTypeNameIdentifier(ITypeName baseType, int rank)
        {
            if (baseType.IsArrayType)
            {
                rank += GetArrayRank(baseType);
                baseType = baseType.ArrayBaseType;
            }

            var identifier = baseType.Identifier;
            var arrayMarker = CreateArrayMarker(rank);

            string derivedIdentifier;
            if (baseType.IsDelegateType)
            {
                derivedIdentifier = identifier + arrayMarker;
            }
            else
            {
                derivedIdentifier = InsertMarkerAfterRawName(identifier, arrayMarker);
            }
            return derivedIdentifier;
        }

        private static int GetArrayRank(ITypeName arrayTypeName)
        {
            return ArrayTypeNameSuffix.Match(arrayTypeName.Identifier).Groups[1].Value.Count(c => c == ',') + 1;
        }

        private static string CreateArrayMarker(int rank)
        {
            return string.Format("[{0}]", new string(',', rank - 1));
        }

        private static string InsertMarkerAfterRawName(string identifier, string arrayMarker)
        {
            var endOfRawName = identifier.IndexOf('[');
            if (endOfRawName < 0)
            {
                endOfRawName = identifier.IndexOf(',');
            }
            if (endOfRawName < 0)
            {
                endOfRawName = identifier.Length;
            }
            return identifier.Insert(endOfRawName, arrayMarker);
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
            get { return TypeName.Get(ArrayTypeNameSuffix.Replace(Identifier, "$2")); }
        }

        public int Rank
        {
            get { return GetArrayRank(this); }
        }

        public override string FullName
        {
            get { return ArrayBaseType.FullName + CreateArrayMarker(Rank); }
        }

        public override IAssemblyName Assembly
        {
            get { return ArrayBaseType.Assembly; }
        }
    }
}