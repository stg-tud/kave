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
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Utils.Assertion;
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

        [ExpectedException(typeof(AssertException)), TestCase(null, 1, 2), TestCase("", 0, 0), TestCase("a]", 0, 1),
         TestCase("[b", 0, 1), TestCase("[]", -1, 1), TestCase("[]", 0, 2), TestCase("][", 1, 0)]
        public void IncorrectCasesAreHandledForParseTypeParameterlist(string id, int startIdx, int endIdx)
        {
            id.ParseTypeParameterList(startIdx, endIdx);
        }

        [Test]
        public void DoesNotBreakNonGenericNames()
        {
            var actual = new MethodName("[T,P] [T,P].Add()").RemoveGenerics();
            var expected = new MethodName("[T,P] [T,P].Add()");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanRemoveGenericInformationFromTypes()
        {
            var actual = new TypeName("G`1[[T -> U,P]], P").RemoveGenerics();
            var expected = new TypeName("G`1[[T]], P");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanRemoveGenericInformationFromMethods()
        {
            var actual = new MethodName("[T,P] [G`1[[T -> U,P]], P].Add([T] item)").RemoveGenerics();
            var expected = new MethodName("[T,P] [G`1[[T]], P].Add([T] item)");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanRemoveGenericInformationFromFields()
        {
            var actual = new FieldName("[T,P] [G`1[[T -> U,P]], P]._f").RemoveGenerics();
            var expected = new FieldName("[T,P] [G`1[[T]], P]._f");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanRemoveGenericInformationFromProperties()
        {
            var actual = new PropertyName("get [T,P] [G`1[[T -> U,P]], P].P()").RemoveGenerics();
            var expected = new PropertyName("get [T,P] [G`1[[T]], P].P()");
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

        [TestCase(
             "Demo.G1`1[][[T -> System.Tuple`2[[T1 -> p:int],[T2 -> p:bool]], mscorlib, 4.0.0.0]], Demo",
             "Demo.G1`1[][[T]], Demo"),
         TestCase(
             "Demo.G1`2[][[S],[T -> System.Tuple`2[[T1 -> p:int],[T2 -> p:bool]], mscorlib, 4.0.0.0]], Demo",
             "Demo.G1`2[][[S],[T]], Demo"),
         TestCase(
             "Demo.G1`2[][][[S],[T -> System.Tuple`2[[T1 -> p:int],[T2 -> p:bool]], mscorlib, 4.0.0.0]], Demo",
             "Demo.G1`2[][][[S],[T]], Demo"),
         TestCase(
             "Demo.G1`2[,][[S],[T -> System.Tuple`2[[T1 -> p:int],[T2 -> p:bool]], mscorlib, 4.0.0.0]], Demo",
             "Demo.G1`2[,][[S],[T]], Demo"),
         TestCase(
             "Demo.G1`2[][[S],[T -> System.Tuple`2[[T1],[T2 -> p:bool]], mscorlib, 4.0.0.0]], Demo",
             "Demo.G1`2[][[S],[T]], Demo")]
        public void RemoveBoundGenericsFromArray(string boundId, string expected)
        {
            var actual = new TypeName(boundId).RemoveGenerics();
            Assert.AreEqual(new TypeName(expected), actual);
        }


        [TestCase(
             "[p:double] [i:Accord.Math.Distances.IDistance`2[[T -> System.Collections.BitArray, mscorlib, 4.0.0.0],[U -> System.Collections.BitArray, mscorlib, 4.0.0.0]], Accord.Math].Distance([T] x, [U] y)",
             "[p:double] [i:Accord.Math.Distances.IDistance`2[[T],[U]], Accord.Math].Distance([T] x, [U] y)"),
         TestCase(
             "[p:double] [i:Accord.Math.Distances.IDistance`2[[T -> System.Tuple`2[[T1 -> p:double],[T2 -> p:double]], mscorlib, 4.0.0.0],[U -> System.Tuple`2[[T1 -> p:double],[T2 -> p:double]], mscorlib, 4.0.0.0]], Accord.Math].Distance([T] x, [U] y)",
             "[p:double] [i:Accord.Math.Distances.IDistance`2[[T],[U]], Accord.Math].Distance([T] x, [U] y)"),
         TestCase(
             "[p:double] [i:Accord.Math.Distances.IDistance`2[[T],[U -> System.Tuple`2[[T1],[U1]], mscorlib, 4.0.0.0]], Accord.Math].Distance([T] x, [U] y)",
             "[p:double] [i:Accord.Math.Distances.IDistance`2[[T],[U]], Accord.Math].Distance([T] x, [U] y)"),
         TestCase(
             "[p:double] [Accord.Math.ADistance`2[[A -> T],[B -> System.Tuple`2[[T1 -> p:int],[T2 -> p:bool]], mscorlib, 4.0.0.0]], Demo].Distance([A] x, [B] y)",
             "[p:double] [Accord.Math.ADistance`2[[A],[B]], Demo].Distance([A] x, [B] y)")]
        public void RemoveGenericsFromMethods(string boundId, string expectedId)
        {
            var actual = new MethodName(boundId).RemoveGenerics();
            var expected = new MethodName(expectedId);
            Assert.AreEqual(expected, actual);
        }
    }
}