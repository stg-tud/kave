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
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class ArrayTypeName : BaseTypeName, IArrayTypeName
    {
        private static readonly Regex ArrayTypeNameSuffix = new Regex("(\\[[,]*\\])([^()]*)$");

        internal ArrayTypeName(string identifier) : base(identifier)
        {
            Asserts.Not(TypeUtils.IsUnknownTypeIdentifier(identifier));
        }

        public ITypeName ArrayBaseType
        {
            get
            {
                var id = Identifier;

                var idx = id.StartsWith("d:")
                    ? id.LastIndexOf(')') + 1
                    : id.IndexOf('[');

                var startIdx = idx;
                while (id[++idx] == ',') {}
                var endIdx = idx;

                var newId = id.Substring(0, startIdx) + id.Substring(endIdx + 1);
                return Names.Type(newId);
            }
        }

        public int Rank
        {
            get { return GetArrayRank(this); }
        }

        public override string Name
        {
            get { return ArrayBaseType.Name + CreateArrayMarker(Rank); }
        }

        public override string FullName
        {
            get { return ArrayBaseType.FullName + CreateArrayMarker(Rank); }
        }

        public override INamespaceName Namespace
        {
            get { return ArrayBaseType.Namespace; }
        }

        public override IAssemblyName Assembly
        {
            get { return ArrayBaseType.Assembly; }
        }

        public override bool IsNestedType
        {
            get { return false; }
        }

        public override ITypeName DeclaringType
        {
            get { return null; }
        }

        #region static helpers

        /// <summary>
        ///     Derives an array-type name from this type name.
        /// </summary>
        /// <param name="baseType">The array's base type</param>
        /// <param name="rank">the rank of the array; must be greater than 0</param>
        public static IArrayTypeName From(ITypeName baseType, int rank)
        {
            Asserts.That(rank > 0, "rank smaller than 1");
            return new ArrayTypeName(DeriveArrayTypeNameIdentifier(baseType, rank));
        }

        private static string DeriveArrayTypeNameIdentifier(ITypeName baseType, int rank)
        {
            if (baseType.IsArrayType)
            {
                rank += GetArrayRank(baseType);
                baseType = baseType.AsArrayTypeName.ArrayBaseType;
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

        public static int GetArrayRank(ITypeName arrayTypeName)
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

        #endregion
    }
}