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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
    internal class PredefinedTypeNameTest
    {
        private static string[][] PredefinedTypeSource()
        {
            return new[]
            {
                new[] {"sbyte", "System.SByte", "p:sbyte"},
                new[] {"byte", "System.Byte", "p:byte"},
                new[] {"short", "System.Int16", "p:short"},
                new[] {"ushort", "System.UInt16", "p:ushort"},
                new[] {"int", "System.Int32", "p:int"},
                new[] {"uint", "System.UInt32", "p:uint"},
                new[] {"long", "System.Int64", "p:long"},
                new[] {"ulong", "System.UInt64", "p:ulong"},
                new[] {"char", "System.Char", "p:char"},
                new[] {"float", "System.Single", "p:float"},
                new[] {"double", "System.Double", "p:double"},
                new[] {"bool", "System.Boolean", "p:bool"},
                new[] {"decimal", "System.Decimal", "p:decimal"},
                new[] {"void", "System.Void", "p:void"},
                new[] {"object", "System.Object", "p:object"},
                new[] {"string", "System.String", "p:string"}
            };
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldRecognizeIdentifier(string shortName, string fullName, string id)
        {
            Assert.IsTrue(PredefinedTypeName.IsPredefinedTypeNameIdentifier(id));
            Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier(id));
            Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(id));
            Assert.IsFalse(TypeParameterName.IsTypeParameterNameIdentifier(id));
            Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(id));
        }
    }
}