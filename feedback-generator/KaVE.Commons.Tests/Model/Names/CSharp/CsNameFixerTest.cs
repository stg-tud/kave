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

using KaVE.Commons.Model.Names;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp
{
    public class CsNameFixerTest
    {

        [TestCase("n.T+T1,a", "n:n.T+T1,a"),
         TestCase("n.T+T1+T2,a", "n:n:n.T+T1+T2,a"),
         TestCase("T`1[[T1 -> i:T`1[[T -> T]], a, 4.0.0.0]], a, 4.0.0.0",
             "T'1[[T1 -> i:T'1[[T -> T]], a, 4.0.0.0]], a, 4.0.0.0"),
        TestCase("d:[e:n.T1+T2, a] [e:n.T1+T2, a].d([e:n.T, a] p)", "d:[n:n.e:T1+T2, a] [n:n.e:T1+T2, a].d([n.e:T, a] p)"),
        TestCase("i:T`1[[T -> i:n.T, a, 0.0.0.0]]+T2, a, 0.0.0.0", "n:i:T'1[[T -> n.i:T, a, 0.0.0.0]]+T2, a, 0.0.0.0")]
        public void HandleOldNamesSimpleNested(string input, string expected)
        {
            Assert.AreEqual(expected, CsNameFixer.HandleOldTypeNames(input));
        }

        [TestCase("[System.Void, mscorlib, 4.0.0.0] [?]..ctor([?] a)", "[?] [?]..ctor([?] a)"),
        TestCase("[?] [?].m(out [?] t)", "[?] [?].m(out [?] t)")]
        public void HandleOldMethodNames(string input, string expected)
        {
            Assert.AreEqual(expected, CsNameFixer.HandleOldMethodNames(input));
        }

        [TestCase("e:n.T, a, 0.0.0.0", "n.e:T, a, 0.0.0.0"),
        TestCase("e:n.T+T2, a, 0.0.0.0", "n.e:T+T2, a, 0.0.0.0")]
        public void HandleTypeIdentifier(string input, string expected)
        {
            Assert.AreEqual(expected, CsNameFixer.HandleTypeIdentifier(input));
        }

        [TestCase("e:n.T+T2, a, 0.0.0.0", "n:n.e:T+T2, a, 0.0.0.0"),
        TestCase("e:n.T+T2+T3, a, 0.0.0.0", "n:n:n.e:T+T2+T3, a, 0.0.0.0")]
        public void HandleNestedTypeIdentifier(string input, string expected)
        {
            Assert.AreEqual(expected, CsNameFixer.HandleNestedTypeNames(CsNameFixer.HandleTypeIdentifier(input)));
        }
    }
}