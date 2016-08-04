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

using System.Collections.Generic;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Utils;
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
         TestCase("N.C1`1[[T1]]+C2[,,][[T2]],P", "N.C1`1[[T1]]+C2`1[,,][[T2]],P"),
        // artifiacial example to make sure that a '.' is used in the regexp and not a wildcard
         TestCase("!C[[T0]], P", "!C[[T0]], P"),
         TestCase("n.C[[T0]], P", "n.C`1[[T0]], P"),
         TestCase("C[[T0]], P", "C`1[[T0]], P")]
        public void FixesMissingGenericTicks(string legacy, string corrected)
        {
            var actual = legacy.FixLegacyFormats();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }


        [TestCase("n.T1`1+T2`1[[G1],[G2]], P", "n.T1`1[[G1]]+T2`1[[G2]], P"),
         TestCase("n.T1`1+T2[[G1]], P", "n.T1`1[[G1]]+T2, P"),
         TestCase("n.T1`1+T2`2+T3`1[[G1 -> P1,P],[G2 -> P2,P],[G3 -> P3, P],[G4 -> P4, P]], P",
             "n.T1`1[[G1 -> P1,P]]+T2`2[[G2 -> P2,P],[G3 -> P3, P]]+T3`1[[G4 -> P4, P]], P")]
        public void FixesLegacyTypeParameterLists(string legacy, string corrected)
        {
            var actual = legacy.FixLegacyFormats();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }

        [TestCase("T`1,P", "T`1,P"), // in general, invalid names are recognized and ignored
         TestCase("T`1!],P", "T`1!],P"), // artificial (invalid) example to test robustness
         TestCase("System.Collections.Generic.Dictionary`2+KeyCollection, mscorlib, 4.0.0.0",
             "System.Collections.Generic.Dictionary`2[[TKey],[TValue]]+KeyCollection, mscorlib, 4.0.0.0"),
        // should not be hit/changed...
         TestCase("{661F-8B...} SomeClass`1.cs", "{661F-8B...} SomeClass`1.cs"),
         TestCase("vsWindowTypeDocument SomeClass`2.cs", "vsWindowTypeDocument SomeClass`2.cs"),
         TestCase("CSharp SomeClass`2.cs", "CSharp SomeClass`2.cs")
        ]
        public void FixesLegacyTypeParameterLists_SomeInvalidsAreHardcodedRestGetsIgnored(string legacy,
            string corrected)
        {
            var actual = legacy.FixLegacyFormats();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }

        [TestCase("A[], B", "A[], B"),
         TestCase("A[][], B", "A[,], B"),
         TestCase("A[][][], B", "A[,,], B"),
         TestCase("A[,][,], B", "A[,,,], B"),
         TestCase("A[,][][,][,], B", "A[,,,,,,], B")]
        public void FixesLegacyArrayFormat(string legacy, string corrected)
        {
            var actual = legacy.FixLegacyFormats();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }

        [Ignore, TestCase("[R,P] [D,P]..ctor()", "[System.Void, mscorlib, 4.0.0.0] [D,P]..ctor()"),
         TestCase("[R,P] [D,P]..ctor()", "[System.Void, mscorlib, 4.0.0.0] [D,P]..cctor()"),
         TestCase("[R,P] [D,P].M()", "[R,P] [D,P].M()"),
        // resilient to parsing issues
         TestCase("[xxx)", "[xxx)"),
         TestCase("[R,P]xxx)", "[R,P]xxx)"),
         TestCase("[?] [xxx)", "[?] [xxx)"),
         TestCase("[?] [?].)", "[?] [?].)"),
         TestCase("[?] [?].()", "[?] [?].()"),
         TestCase("[?] [?].M()", "[?] [?].M()")]
        public void FixesCtorsWithNonVoidReturn(string legacy, string corrected)
        {
            var actual = legacy.FixLegacyFormats();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource("PredefinedTypesSource")]
        public void ShouldFixPredefinedNames(string strIn, string expected)
        {
            var actual = strIn.FixLegacyFormats();
            Assert.AreEqual(expected, actual);
        }

        public IEnumerable<string[]> PredefinedTypesSource()
        {
            // pType --> former type
            var newToOld = new Dictionary<string, string>
            {
                {"p:sbyte", "System.SByte"},
                {"p:byte", "System.Byte"},
                {"p:short", "System.Int16"},
                {"p:ushort", "System.UInt16"},
                {"p:int", "System.Int32"},
                {"p:uint", "System.UInt32"},
                {"p:long", "System.Int64"},
                {"p:ulong", "System.UInt64"},
                {"p:char", "System.Char"},
                {"p:float", "System.Single"},
                {"p:double", "System.Double"},
                {"p:bool", "System.Boolean"},
                {"p:decimal", "System.Decimal"},
                //
                {"p:void", "System.Void"},
                //
                {"p:object", "System.Object"},
                {"p:string", "System.String"}
            };


            var cases = Lists.NewList<string[]>();
            foreach (var newId in newToOld.Keys)
            {
                var oldId = "{0}, mscorlib, 1.2.3.4".FormatEx(newToOld[newId]);

                foreach (var caseStr in new[] {"{0}", "T`1[[G -> {0}]],P", "G -> {0}", "[{0}] [?].M()"})
                {
                    var oldStr = caseStr.FormatEx(oldId);
                    var newStr = caseStr.FormatEx(newId);

                    cases.Add(new[] {oldStr, newStr});
                }

                foreach (var arrPart in new[] {"[]", "[,]"})
                {
                    var oldArrId = "{0}{1}, mscorlib, 1.2.3.4".FormatEx(newToOld[newId], arrPart);
                    var newArrId = "{0}{1}".FormatEx(newId, arrPart);
                    cases.Add(new[] {oldArrId, newArrId});
                }
            }

            const string old1 = "[System.Int32, mscorlib, 1.2.3.4] [System.Single, mscorlib, 2.3.4.5].M()";
            const string new1 = "[p:int] [p:float].M()";
            cases.Add(new[] {old1, new1});

            return cases;
        }
    }
}