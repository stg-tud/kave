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
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Model.Naming.Impl.v0.Types
{
    public class ArrayTypeName : BaseTypeName, IArrayTypeName
    {
        internal ArrayTypeName(string identifier) : base(identifier)
        {
            Asserts.Not(TypeUtils.IsUnknownTypeIdentifier(identifier));
        }

        public ITypeName ArrayBaseType
        {
            get
            {
                // can not be TypeParameter)

                var id = Identifier;
                string newId;

                if (id.StartsWith("d:")) // base is delegate
                {
                    newId = id.Substring(0, id.LastIndexOf(')') + 1);
                    return TypeUtils.CreateTypeName(newId);
                }

                var openArr = FindArrayMarkerIndex(id);
                Asserts.Not(openArr == -1);
                var closeArr = id.FindCorrespondingCloseBracket(openArr);
                Asserts.Not(closeArr == -1);

                newId = id.Remove(openArr, closeArr - openArr + 1);
                return TypeUtils.CreateTypeName(newId);
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

        internal static bool IsArrayTypeNameIdentifier(string id)
        {
            if (id.StartsWith("d:"))
            {
                var idx = id.LastIndexOf(')');
                if (id.FindNext(idx, '[') != -1)
                {
                    return true;
                }
            }
            else if (TypeParameterName.IsTypeParameterIdentifier(id))
            {
                return false;
            }
            else
            {
                return FindArrayMarkerIndex(id) != -1;
            }

            return false;
        }

        public static int GetArrayRank(ITypeName typeName)
        {
            var id = typeName.Identifier;
            var arrOpen = FindArrayMarkerIndex(id);
            if (arrOpen == -1)
            {
                return 0;
            }
            var arrClose = id.FindCorrespondingCloseBracket(arrOpen);
            return arrClose - arrOpen;
        }

        private static int FindArrayMarkerIndex(string id)
        {
            var closeBracket = id.LastIndexOf(']');
            if (closeBracket == -1)
            {
                return -1;
            }
            var cur = closeBracket;

            // regular (multi-dimensional) array
            while (cur - 1 > 0 && id[--cur] == ',') {}
            if (id[cur] == '[')
            {
                return cur;
            }

            // generic
            var openGeneric = id.FindCorrespondingOpenBracket(closeBracket);
            var tick = id.FindPrevious(openGeneric, '`');
            var openArr = id.FindNext(tick, '[');
            if (openArr == openGeneric)
            {
                return -1;
            }
            return openArr;
        }

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
            var realBase = baseType.IsArray ? baseType.AsArrayTypeName.ArrayBaseType : baseType;
            var realRank = baseType.IsArray ? baseType.AsArrayTypeName.Rank + rank : rank;
            var arrMarker = CreateArrayMarker(realRank);

            if (realBase.IsTypeParameter)
            {
                if (realBase.AsTypeParameterName.IsBound)
                {
                    var paramType = baseType.AsTypeParameterName.TypeParameterType;
                    return "{0}{1} -> {2}".FormatEx(realBase.Name, arrMarker, paramType.Identifier);
                }
                return "{0}{1}".FormatEx(realBase.Name, arrMarker);
            }

            if (realBase.IsDelegateType)
            {
                return realBase.Identifier + arrMarker;
            }

            return InsertMarkerAfterRawName(realBase, arrMarker);
        }

        private static string CreateArrayMarker(int rank)
        {
            return String.Format("[{0}]", new string(',', rank - 1));
        }

        private static string InsertMarkerAfterRawName(ITypeName baseType, string arrayMarker)
        {
            Asserts.Not(baseType.IsArray);
            Asserts.Not(baseType.IsDelegateType);
            Asserts.Not(baseType.IsTypeParameter);

            var id = baseType.Identifier;
            var arrIdx = -1;

            if (baseType.HasTypeParameters)
            {
                var closeGeneric = id.LastIndexOf("]", StringComparison.Ordinal);
                arrIdx = id.FindCorrespondingOpenBracket(closeGeneric);
            }
            else
            {
                var beforeAssemble = id.Length - baseType.Assembly.Identifier.Length;
                var comma = id.FindPrevious(beforeAssemble, ',');
                arrIdx = comma;
            }

            Asserts.Not(arrIdx == -1);
            return id.Insert(arrIdx, arrayMarker);
        }

        #endregion
    }
}