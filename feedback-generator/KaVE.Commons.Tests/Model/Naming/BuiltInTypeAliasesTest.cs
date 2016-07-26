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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming
{
    internal class BuiltInTypeAliasesTest
    {
        private static string[][] Aliases()
        {
            string[][] aliases =
            {
                new[] {"sbyte", "System.SByte"},
                new[] {"byte", "System.Byte"},
                new[] {"short", "System.Int16"},
                new[] {"ushort", "System.UInt16"},
                new[] {"int", "System.Int32"},
                new[] {"uint", "System.UInt32"},
                new[] {"long", "System.Int64"},
                new[] {"ulong", "System.UInt64"},
                new[] {"char", "System.Char"},
                new[] {"float", "System.Single"},
                new[] {"double", "System.Double"},
                new[] {"bool", "System.Boolean"},
                new[] {"decimal", "System.Decimal"},
                new[] {"void", "System.Void"},
                new[] {"object", "System.Object"},
                new[] {"string", "System.String"}
            };
            return aliases;
        }

        [TestCaseSource("Aliases")]
        public void ShouldReplaceShortName(string shortName, string fullName)
        {
            var actual = BuiltInTypeAliases.GetFullTypeNameFromTypeAlias(shortName);
            Assert.AreEqual(fullName, actual);
        }

        [TestCaseSource("Aliases")]
        public void ShouldReplaceShortNameNullables(string shortName, string fullName)
        {
            var actual = BuiltInTypeAliases.GetFullTypeNameFromTypeAlias(fullName + "?");
            var expected = "System.Nullable`1[[T -> {0}]]".FormatEx(fullName);
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource("Aliases")]
        public void ShouldReplaceShortNameArrays(string shortName, string fullName)
        {
            var actual = BuiltInTypeAliases.GetFullTypeNameFromTypeAlias(fullName + "[]");
            var expected = "{0}[]".FormatEx(fullName);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldKeepShortNameIfNotFound()
        {
            var actual = BuiltInTypeAliases.GetFullTypeNameFromTypeAlias("x");
            Assert.AreEqual("x", actual);
        }

        [TestCaseSource("Aliases")]
        public void ShouldReplaceFullName(string shortName, string fullName)
        {
            var actual = BuiltInTypeAliases.GetTypeAliasFromFullTypeName(fullName);
            Assert.AreEqual(shortName, actual);
        }

        [Test]
        public void ShouldKeepFullNameIfNotFound()
        {
            var actual = BuiltInTypeAliases.GetTypeAliasFromFullTypeName("x");
            Assert.AreEqual("x", actual);
        }
    }
}