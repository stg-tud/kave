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
using KaVE.Commons.Model.ObjectUsage;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.ObjectUsage
{
    internal class CoReNameUtilTest
    {
        [TestCase("TType, A, 1.0.0.0", "LTType"),
         TestCase("System.Void, mscore, 4.0.0.0", "LSystem/Void"),
         TestCase("Coll.List`1[[T -> T]], mscore, 4.0.0.0", "LColl/List"),
         TestCase("N.Type[], A, 1.0.0.0", "[LN/Type"),
         TestCase("N.Type[,,], A, 1.0.0.0", "[LN/Type"),
         TestCase("Name.Outer+Inner, Assembly, 1.2.3.4", "LName/Outer$Inner"),
         TestCase("?", "LUnknown")]
        public void ShouldConvertTypeNames(string iName, string coReName)
        {
            var expected = new CoReTypeName(coReName);
            var original = Names.Type(iName);

            var actual = original.ToCoReName();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("[Sys.V, A, 1.0.0.0] [N.T, A, 1.0.0.0].m([N.P, A, 1.0.0.0] arg)", "LN/T.m(LN/P;)LSys/V;"),
         TestCase("[Sys.V, A, 1.0.0.0] [N.T, A, 1.0.0.0].m`1[[G]]([G] arg)", "LN/T.m(LSystem/Object;)LSys/V;"),
         TestCase("[System.Void, mscorlib, 4.0.0.0] [N.T, A, 1.0.0.0]..ctor()", "LN/T.<init>()LSystem/Void;"),
         TestCase("[?] [?].???()", "LUnknown.unknown()LUnknown;")]
        public void ShouldConvertMethodNames(string iName, string coReName)
        {
            var expected = new CoReMethodName(coReName);
            var original = Names.Method(iName);

            var actual = original.ToCoReName();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("[N.S, A, 1.0.0.0] [N.T, A, 1.0.0.0].f", "LN/T.f;LN/S"),
         TestCase("[?] [?].???", "LUnknown.unknown;LUnknown")]
        public void ShouldConvertFieldNames(string iName, string coReName)
        {
            var expected = new CoReFieldName(coReName);
            var original = Names.Field(iName);

            var actual = original.ToCoReName();

            Assert.AreEqual(expected, actual);
        }
    }
}