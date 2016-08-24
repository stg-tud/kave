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
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0
{
    internal class NameFixesTest
    {
        [ // field
            TestCase("[?] [?]._f", "[?] [?]._f"), TestCase("static [?] [?]._f", "static [?] [?]._f"),
            // get
            TestCase("get [?] [?].P", "get [?] [?].P()"), TestCase("get static [?] [?].P", "get static [?] [?].P()"),
            // set
            TestCase("set [?] [?].P", "set [?] [?].P()"), TestCase("set static [?] [?].P", "set static [?] [?].P()"),
            // get + set
            TestCase("get set [?] [?].P", "get set [?] [?].P()"),
            TestCase("get set static [?] [?].P", "get set static [?] [?].P()"),
            // correct examples
            TestCase("get set [?] [?].P()", "get set [?] [?].P()"),
            TestCase("get set [?] [?].P([p:int] i)", "get set [?] [?].P([p:int] i)")]
        public void FixesMissingParenthesisOfProperties(string legacy, string corrected)
        {
            Assert.AreEqual(corrected, legacy.FixIdentifiers());
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
            Assert.AreEqual(corrected, legacy.FixIdentifiers());
        }

        [TestCase("n.C1`1[[T1]]+C2[[T2]]+C3[[T3]], P", "n.C1`1[[T1]]+C2`1[[T2]]+C3`1[[T3]], P"),
         TestCase("n.C1`1[[T1]]+C2[[T2],[T3]]+C3[[T3]], P", "n.C1`1[[T1]]+C2`2[[T2],[T3]]+C3`1[[T3]], P"),
         TestCase("n.C1`1[[T1]]+C2[[T2] , [T3] ]+C3[[T3]], P", "n.C1`1[[T1]]+C2`2[[T2] , [T3] ]+C3`1[[T3]], P"),
         TestCase("N.C1`1[[T1]]+C2[][[T2]],P", "N.C1`1[[T1]]+C2`1[][[T2]],P"),
         TestCase("N.C1`1[[T1]]+C2[,][[T2]],P", "N.C1`1[[T1]]+C2`1[,][[T2]],P"),
         TestCase("N.C1`1[[T1]]+C2[,,][[T2]],P", "N.C1`1[[T1]]+C2`1[,,][[T2]],P"),
        // artificial example to make sure that a '.' is used in the regexp and not a wildcard
         TestCase("!C[[T0]], P", "!C[[T0]], P"),
         TestCase("n.C[[T0]], P", "n.C`1[[T0]], P"),
         TestCase("C[[T0]], P", "C`1[[T0]], P")]
        public void FixesMissingGenericTicks(string legacy, string corrected)
        {
            var actual = legacy.FixIdentifiers();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }


        [TestCase("n.T1`1+T2`1[[G1],[G2]], P", "n.T1`1[[G1]]+T2`1[[G2]], P"),
         TestCase("n.T1`1+T2[[G1]], P", "n.T1`1[[G1]]+T2, P"),
         TestCase("n.T1`1+T2`2+T3`1[[G1 -> P1,P],[G2 -> P2,P],[G3 -> P3, P],[G4 -> P4, P]], P",
             "n.T1`1[[G1 -> P1,P]]+T2`2[[G2 -> P2,P],[G3 -> P3, P]]+T3`1[[G4 -> P4, P]], P")]
        public void FixesLegacyTypeParameterLists(string legacy, string corrected)
        {
            var actual = legacy.FixIdentifiers();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }

        // 0M:[s:System.Collections.Generic.List`1[][[[T -> T]]]+Enumerator, mscorlib, 4.0.0.0] .GetEnumerator()
        [TestCase("T`1,P", "T`1,P"), // in general, invalid names are recognized and ignored
         TestCase("T`1!],P", "T`1!],P"), // artificial (invalid) example to test robustness
         TestCase("System.Collections.Generic.Dictionary`2+KeyCollection, mscorlib, 4.0.0.0",
             "System.Collections.Generic.Dictionary`2[[TKey],[TValue]]+KeyCollection, mscorlib, 4.0.0.0"),
        // should not be hit/changed...
         TestCase("{661F-8B...} SomeClass`1.cs", "{661F-8B...} SomeClass`1.cs"),
         TestCase("vsWindowTypeDocument SomeClass`2.cs", "vsWindowTypeDocument SomeClass`2.cs"),
         TestCase("CSharp SomeClass`2.cs", "CSharp SomeClass`2.cs")]
        public void FixesLegacyTypeParameterLists_SomeInvalidsAreHardcodedRestGetsIgnored(string legacy,
            string corrected)
        {
            var actual = legacy.FixIdentifiers();
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
            var actual = legacy.FixIdentifiers();
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
            var actual = legacy.FixIdentifiers();
            var expected = corrected;
            Assert.AreEqual(expected, actual);
        }

        [TestCase("System.Nullable`1[[T]]...", "s:System.Nullable`1[[T]]..."),
         TestCase("System.Nullable`1[][[T]]...", "s:System.Nullable`1[][[T]]..."),
         TestCase("...System.Nullable`1[[T]]...", "...s:System.Nullable`1[[T]]..."),
         TestCase("s:System.Nullable`1[[T]]...", "s:System.Nullable`1[[T]]..."),
         TestCase("s:System.Nullable`1[][[T]]...", "s:System.Nullable`1[][[T]]..."),
         TestCase("...s:System.Nullable`1[[T]]...", "...s:System.Nullable`1[[T]]...")]
        public void FixesOldNullableNames(string legacy, string expected)
        {
            var actual = legacy.FixIdentifiers();
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource("PredefinedTypesSource")]
        public void FixesPredefinedNames(string strIn, string expected)
        {
            var actual = strIn.FixIdentifiers();
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


        public static IEnumerable<string[]> BrokenSerializedPropertyNamesSource()
        {
            var cases = Sets.NewHashSet<string[]>();
            var prefixes = new[] {"0P", "CSharp.PropertyName"};
            foreach (var prefix in prefixes)
            {
                cases.Add(new[] {prefix, "[?] [?].P", "set get [?] [?].P()"});
                cases.Add(new[] {prefix, " [?] [?].P", "set get [?] [?].P()"});
                cases.Add(new[] {prefix, "  [?] [?].P", "set get [?] [?].P()"});
                cases.Add(new[] {prefix, "static [?] [?].P", "set get static [?] [?].P()"});
                cases.Add(new[] {prefix, " static [?] [?].P", "set get static [?] [?].P()"});
                cases.Add(new[] {prefix, "  static  [?] [?].P", "set get static [?] [?].P()"});
            }

            return cases;
        }

        [TestCaseSource("BrokenSerializedPropertyNamesSource")]
        public void ShouldFixBrokenPropertyNames(string prefix, string id, string corrected)
        {
            Assert.AreEqual(corrected, id.FixIdentifiers(prefix));
        }

        public static IEnumerable<string[]> ValidIdSource()
        {
            var ids = new List<string[]>();
            var delTypeId = "d:[TR] [C`10[[T1]], P].()";
            // not valid, but counts as a minimal example
            ids.Add(new[] {"[{0}] [{0}]..ctor()".FormatEx(delTypeId), "0M"});
            ids.Add(new[] {"N.C`10[[T]],P", "0T"});
            ids.Add(new[] {"getdatetime.Program, getdatetime", "0T"});
            ids.Add(new[] {"[?] [?].???", "0P"});
            ids.Add(new[] {"[?] [?].???", "0F"});
            ids.Add(new[] {"[?] [?].???", "0E"});
            ids.Add(new[] {"[?] [?].???()", "0M"});
            return ids;
        }

        [TestCaseSource("ValidIdSource")]
        public void FixDoesNotBreakValidIds(string id, string prefix)
        {
            Assert.AreEqual(id, id.FixIdentifiers());
            Assert.AreEqual(id, id.FixIdentifiers(prefix));
        }


        public static IEnumerable<string[]> ManualTypeFixSource()
        {
            var baseIds = new List<string[]>
            {
                new[]
                {
                    "s:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1+Enumerator, EntityFramework, 6.0.0.0",
                    "s:System.Data.Entity.Core.Metadata.Edm.ReadOnlyMetadataCollection`1[[T]]+Enumerator, EntityFramework, 6.0.0.0"
                },
                new[]
                {
                    "System.Collections.Generic.Dictionary`2+KeyCollection, mscorlib, 4.0.0.0",
                    "System.Collections.Generic.Dictionary`2[[TKey],[TValue]]+KeyCollection, mscorlib, 4.0.0.0"
                },
                new[]
                {
                    "System.Collections.Generic.Dictionary`2+ValueCollection, mscorlib, 4.0.0.0",
                    "System.Collections.Generic.Dictionary`2[[TKey],[TValue]]+ValueCollection, mscorlib, 4.0.0.0"
                },
                new[]
                {
                    "System.Collections.ObjectModel.ReadOnlyDictionary`2+KeyCollection, mscorlib, 4.0.0.0",
                    "System.Collections.ObjectModel.ReadOnlyDictionary`2[[TKey],[TValue]]+KeyCollection, mscorlib, 4.0.0.0"
                },
                new[]
                {
                    "System.Collections.ObjectModel.ReadOnlyDictionary`2+ValueCollection, mscorlib, 4.0.0.0",
                    "System.Collections.ObjectModel.ReadOnlyDictionary`2[[TKey],[TValue]]+ValueCollection, mscorlib, 4.0.0.0"
                },
                new[]
                {
                    "s:System.Collections.Generic.Dictionary`2+Enumerator, mscorlib, 4.0.0.0",
                    "s:System.Collections.Generic.Dictionary`2[[TKey],[TValue]]+Enumerator, mscorlib, 4.0.0.0"
                },
                new[]
                {
                    "s:System.Collections.Immutable.ImmutableArray`1+Enumerator, System.Collections.Immutable, 1.1.37.0",
                    "s:System.Collections.Immutable.ImmutableArray`1[[T]]+Enumerator, System.Collections.Immutable, 1.1.37.0"
                },
                new[]
                {
                    "d:[TValue] [System.Runtime.CompilerServices.ConditionalWeakTable`2+CreateValueCallback, mscorlib, 4.0.0.0].([TKey] key)",
                    "d:[TValue] [System.Runtime.CompilerServices.ConditionalWeakTable`2[[TKey],[TValue]]+CreateValueCallback, mscorlib, 4.0.0.0].([TKey] key)"
                },
                new[]
                {
                    "s:System.Collections.Generic.List`1+Enumerator, mscorlib, 4.0.0.0",
                    "s:System.Collections.Generic.List`1[[T]]+Enumerator, mscorlib, 4.0.0.0"
                },
                new[]
                {
                    "System.Collections.Generic.List`1+SynchronizedList, mscorlib, 4.0.0.0",
                    "System.Collections.Generic.List`1[[T]]+SynchronizedList, mscorlib, 4.0.0.0"
                }
            };


            var ids = new List<string[]>();
            foreach (var baseId in baseIds)
            {
                ids.Add(baseId);
                // create nested case
                ids.Add(new[] {"C`1[[{0}]], P".FormatEx(baseId[0]), "C`1[[{0}]], P".FormatEx(baseId[1])});
            }

            return ids;
        }

        [TestCaseSource("ManualTypeFixSource")]
        public void ShouldFixManualParameterLists(string id, string corrected)
        {
            Assert.AreEqual(corrected, id.FixIdentifiers());
        }

        public static IEnumerable<string[]> BrokenFixesSource()
        {
            var cases = Sets.NewHashSet<string[]>();

            var delTypeId =
                "d:[TResult] [System.Func`10[[T1],[T2],[T3],[T4],[T5],[T6],[T7],[T8],[T9],[TResult]], System.Core, 4.0.0.0].([T1] arg1, [T2] arg2, [T3] arg3, [T4] arg4, [T5] arg5, [T6] arg6, [T7] arg7, [T8] arg8, [T9] arg9))";
            cases.Add(
                new[]
                {
                    "0M",
                    "[d:[TResult] [System.Func`10[[T9]][[TResult],[System.Func`10[[T9]][[T1],[T2],[T3],[T4],[T5],[T6],[T7],[T8],[T9],[TResult]], System.Core, 4.0.0.0],[T1],[T2],[T3],[T4],[T5],[T6],[T7],[T8]][[T1],[T2],[T3],[T4],[T5],[T6],[T7],[T8],[T9],[TResult]], System.Core, 4.0.0.0].([T1] arg1, [T2] arg2, [T3] arg3, [T4] arg4, [T5] arg5, [T6] arg6, [T7] arg7, [T8] arg8, [T9] arg9)] ..ctor()",
                    "[{0}] [{0}]..ctor()".FormatEx(delTypeId)
                });
            cases.Add(
                new[]
                {
                    "0M",
                    "[s:System.Collections.Generic.List`1[][[[T -> T]]]+Enumerator, mscorlib, 4.0.0.0] .GetEnumerator()",
                    "[s:System.Collections.Generic.List`1[[T -> T]]+Enumerator, mscorlib, 4.0.0.0] [System.Collections.Generic.List`1[[T -> T]], mscorlib, 4.0.0.0].GetEnumerator()"
                });
            cases.Add(new[] {"0M", "[p:void] ..ctor()", "[?] [?].???()"});
            return cases;
        }

        [TestCaseSource("BrokenFixesSource")]
        public void ShouldFixBrokenFixes(string prefix, string id, string expected)
        {
            Assert.AreEqual(expected, id.FixIdentifiers(prefix));
        }

        [Test]
        public void ShouldNotCreateInvalidIdsWhenFixingTypeParameterLists()
        {
            const string id = "N.C`1+N, P";
            Assert.AreEqual(id, id.FixIdentifiers());
            Assert.AreEqual(id, id.FixIdentifiers("0T"));
        }
    }
}