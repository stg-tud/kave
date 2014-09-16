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
 *    - Dennis Albrecht
 */

using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.ObjectUsage;
using NUnit.Framework;

namespace KaVE.Model.Tests.ObjectUsage
{
    [TestFixture]
    internal class CoReNameUtilTest
    {
        private static readonly TestCaseData[] TestCases =
        {
            // TODO: How to represent the disabled examples
            new TestCaseData(TypeName.Get("TType, A, 1.0.0.0"), new CoReTypeName("LTType")),
            new TestCaseData(TypeName.Get("System.Void, mscore, 4.0.0.0"), new CoReTypeName("LSystem/Void")),
            new TestCaseData(TypeName.Get("Coll.List`1[T -> T], mscore, 4.0.0.0"), new CoReTypeName("LColl/List")),
            //new TestCaseData(TypeName.Get("Name.Outer+Inner, Assembly, 1.2.3.4"), new CoReTypeName("???")),
            new TestCaseData(
                MethodName.Get("[Sys.V, A, 1.0.0.0] [N.T, A, 1.0.0.0].m([N.P, A, 1.0.0.0] arg)"),
                new CoReMethodName("LN/T.m(LN/P;)LSys/V;")),
            new TestCaseData(
                MethodName.Get("[Sys.V, A, 1.0.0.0] [N.T, A, 1.0.0.0].m`1[G -> G]([G, A, 1.0.0.0] arg)"),
                new CoReMethodName("LN/T.m(LG;)LSys/V;")),
            //new TestCaseData(MethodName.Get("[N.T, A, 1.0.0.0] [N.T, A, 1.0.0.0]..ctor()"), new CoReMethodName("LN/T..ctor()LN/T;"))
            new TestCaseData(FieldName.Get("[N.S, A, 1.0.0.0] [N.T, A, 1.0.0.0].f"), new CoReFieldName("LN/T.f;LN/S")),
            new TestCaseData(AssemblyName.Get("mscore, 4.0.0.0"), null)
            // TODO: Some more test-cases that aren't convertable?
        };

        [TestCaseSource("TestCases")]
        public void ShouldConvertNames(IName input, CoReName expected)
        {
            var actual = input.ToCoReName();

            Assert.AreEqual(expected, actual);
        }
    }
}