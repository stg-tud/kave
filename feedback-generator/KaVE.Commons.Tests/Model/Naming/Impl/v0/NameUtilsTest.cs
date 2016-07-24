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

using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0
{
    internal class NameUtilsTest
    {
        [Test]
        public void ParsesEmptyParameters()
        {
            const string sig = "...()";
            var expected = Lists.NewList<IParameterName>();
            Assert.AreEqual(expected, sig.GetParameterNamesFromSignature(3, 4));
        }

        [TestCase("([A`1[[B, P]], P] p)", "[A`1[[B, P]], P] p"),
         TestCase("([d:[?] [?].()] p)", "[d:[?] [?].()] p"),
         TestCase("(  out [T,P] p   )", "out [T,P] p")]
        public void ParsesParameters(string parameters, string parameterId)
        {
            var sig = "..." + parameters;
            var expected = Lists.NewList(new ParameterName(parameterId));
            var endIdx = parameters.Length - 1 + 3;
            Assert.AreEqual(expected, sig.GetParameterNamesFromSignature(3, endIdx));
        }

        [TestCase("([T1,P] a, [T2,P] b)", "[T1,P] a", "[T2,P] b"),
         TestCase("(out [T,P] p, ref [T,P] q)", "out [T,P] p", "ref [T,P] q")]
        public void ParsesMultipleParameters(string parameters, string paramId1, string paramId2)
        {
            var sig = "..." + parameters;
            var expected = Lists.NewList(new ParameterName(paramId1), new ParameterName(paramId2));
            var endIdx = parameters.Length - 1 + 3;
            Assert.AreEqual(expected, sig.GetParameterNamesFromSignature(3, endIdx));
        }

        [Test]
        public void DoesNotBreakNonGenericNames()
        {
            var actual = new MethodName("[T,P] [T,P].Add()").RemoveGenerics();
            var expected = new MethodName("[T,P] [T,P].Add()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanRemoveGenericInformationFromResolvedTypes()
        {
            var actual = new MethodName("[T,P] [G`1[[T -> U,P]], P].Add([T] item)").RemoveGenerics();
            var expected = new MethodName("[T,P] [G`1[[T]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void StillWorksWithUnresolvedParameters()
        {
            var actual = new MethodName("[T,P] [G`1[[T]], P].Add([T] item)").RemoveGenerics();
            var expected = new MethodName("[T,P] [G`1[[T]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WorksWithNestedParameters()
        {
            var actual = new MethodName("[T,P] [G`1[[T -> G2`2[[A -> T,P],[B]]]], P].Add([T] item)").RemoveGenerics();
            var expected = new MethodName("[T,P] [G`1[[T]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WorksWithMultipleParameters_2()
        {
            var actual = new MethodName("[T,P] [G`2[[T->T2,P],[U->U2,P]], P].Add([T] item)").RemoveGenerics();
            var expected = new MethodName("[T,P] [G`2[[T],[U]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WorksWithMultipleParameters_3()
        {
            var actual = new MethodName("[T,P] [G`3[[T->T2,P],[U->U2,P],[V->V2,P]], P].Add([T] item)").RemoveGenerics();
            var expected = new MethodName("[T,P] [G`3[[T],[U],[V]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MultipleGenericTypes()
        {
            var actual = new MethodName("[T`1[[A->A2,P]],P] [T2`1[[B->B2,P]], P].Add([T] item)").RemoveGenerics();
            var expected = new MethodName("[T`1[[A]],P] [T2`1[[B]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [TestCase("d:n.D,P", "d:[?] [n.D,P].()"),
         TestCase("T -> d:n.D,P", "T -> d:[?] [n.D,P].()"),
         TestCase("C`1[[T -> d:n.D,P]],P", "C`1[[T -> d:[?] [n.D,P].()]],P"),
         TestCase("[?] [d:n.D,P].M([?] p)",
             "[?] [d:[?] [n.D,P].()].M([?] p)"),
         TestCase("C`2[[T -> d:n.D,P],[T -> d:n.D2,P]],P", "C`2[[T -> d:[?] [n.D,P].()],[T -> d:[?] [n.D2,P].()]],P"),
         TestCase("[d:n.D,P] [d:n.D2,P].M([?] p)",
             "[d:[?] [n.D,P].()] [d:[?] [n.D2,P].()].M([?] p)")]
        public void FixesLegacyDelegateTypeNameFormat(string legacy, string corrected)
        {
            Assert.AreEqual(corrected, legacy.FixLegacyFormats());
        }

        [TestCase("n.C1`1[[T1]]+C2[[T2]]+C3[[T3]], P", "n.C1`1[[T1]]+C2`1[[T2]]+C3`1[[T3]], P"),
         TestCase("n.C1`1[[T1]]+C2[[T2],[T3]]+C3[[T3]], P", "n.C1`1[[T1]]+C2`2[[T2],[T3]]+C3`1[[T3]], P"),
         TestCase("n.C1`1[[T1]]+C2[[T2] , [T3] ]+C3[[T3]], P", "n.C1`1[[T1]]+C2`2[[T2] , [T3] ]+C3`1[[T3]], P"),
         TestCase("N.C1`1[[T1]]+C2[][[T2]],P", "N.C1`1[[T1]]+C2`1[][[T2]],P"),
         TestCase("N.C1`1[[T1]]+C2[,][[T2]],P", "N.C1`1[[T1]]+C2`1[,][[T2]],P"),
         TestCase("N.C1`1[[T1]]+C2[,,][[T2]],P", "N.C1`1[[T1]]+C2`1[,,][[T2]],P")]
        public void MissingGenericTicksAreAdded(string legacy, string corrected)
        {
            var actual = legacy.FixLegacyFormats();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }
    }
}