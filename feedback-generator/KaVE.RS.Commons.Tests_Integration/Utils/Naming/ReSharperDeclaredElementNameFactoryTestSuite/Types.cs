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

using JetBrains.Util;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.Statements;
using KaVE.Commons.Utils;
using NUnit.Framework;
using Fix = KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.RS.Commons.Tests_Integration.Utils.Naming.ReSharperDeclaredElementNameFactoryTestSuite
{
    internal class Types : NameFactoryBaseTest
    {
        #region basic cases

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
        public void ShouldParseShortNamesOfSimpleTypes(string shortName, string fullName, string typeId)
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(" + shortName + @" p) { $ }
                }
            ");

            var actual = AssertSingleParameter().ValueType;
            var expected = new PredefinedTypeName(typeId);
            Assert.AreEqual(expected, actual);
        }

        [TestCaseSource("PredefinedTypeSource")]
        public void ShouldParseFullNamesOfSimpleTypes(string shortName, string fullName, string typeId)
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(" + fullName + @" p) { $ }
                }
            ");

            var actual = AssertSingleParameter().ValueType;
            var expected = new PredefinedTypeName(typeId);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Unknown()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(XXX ns) { $ }
                }
            ");

            AssertParameterTypes(Fix.Unknown.Identifier);
        }

        [Test]
        public void Unknown_Array()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(XXX[] ns) { $ }
                }
            ");

            AssertParameterTypes(Fix.Unknown.Identifier + "[]");
        }

        [Test]
        public void Arrays()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(int[] ns) { $ }
                }
            ");

            AssertParameterTypes(Fix.IntArray.Identifier);
        }

        [Test]
        public void Delegates()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public delegate void D(int i);
                    public void M(D d) { $ }
                }
            ");

            var delType = "d:[{0}] [N.C+D, TestProject].([{1}] i)".FormatEx(Fix.Void, Fix.Int);
            AssertParameterTypes(delType);
        }

        [Test]
        public void Delegates_BuiltIn()
        {
            CompleteInNamespace(@"
                public class C
                {
                    public void M(Action<int> a) { $ }
                }
            ");

            var id = "d:[{0}] [System.Action`1[[T -> {1}]], mscorlib, 4.0.0.0].([T] obj)".FormatEx(Fix.Void, Fix.Int);
            AssertParameterTypes(id);
        }

        [Test]
        public void Basic_Enums()
        {
            CompleteInNamespace(@"
                public enum E {}
                public class C
                {
                    public void M(E d) { $ }
                }
            ");

            AssertParameterTypes("e:N.E, TestProject");
        }

        [Test]
        public void Basic_Interfaces()
        {
            CompleteInNamespace(@"
                public interface I {}
                public class C
                {
                    public void M(I i) { $ }
                }
            ");

            AssertParameterTypes("i:N.I, TestProject");
        }

        [Test]
        public void Basic_Structs()
        {
            CompleteInNamespace(@"
                public struct S {}
                public class C
                {
                    public void M(S s) { $ }
                }
            ");

            AssertParameterTypes("s:N.S, TestProject");
        }

        #endregion

        #region generics

        [Test]
        public void Generics_Free()
        {
            CompleteInNamespace(@"
                class C<G1>
                {
                    public void M()
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C`1[[G1]], TestProject].M()", Fix.Void);
        }

        [Test]
        public void Generics_Bound_Type()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C
                {
                    public void M(G<int> p)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C, TestProject].M([N.G`1[[G1 -> {1}]], TestProject] p)", Fix.Void, Fix.Int);
        }

        [Test]
        public void Generics_Bound_Unknown()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C
                {
                    public void M(G<XYZ> p)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C, TestProject].M([N.G`1[[G1 -> ?]], TestProject] p)", Fix.Void);
        }

        [Test]
        public void Generics_Bound_TypeParameter()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C<G2>
                {
                    public void M(G<G2> p)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C`1[[G2]], TestProject].M([N.G`1[[G1 -> G2]], TestProject] p)", Fix.Void);
        }

        [Test]
        public void Generics_Bound_TypeParameterWithSameName()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C<G1>
                {
                    public void M(G<G1> p)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName("[{0}] [N.C`1[[G1]], TestProject].M([N.G`1[[G1 -> G1]], TestProject] p)", Fix.Void);
        }

        [Test]
        public void Generics_Combination()
        {
            CompleteInNamespace(@"
                class G<G1> {}
                class C<G2>
                {
                    public void M(G2 p1, G<G2> p2, G<int> p3)
                    {
                        $
                    }
                }
            ");

            AssertSingleMethodName(
                "[{0}] [N.C`1[[G2]], TestProject].M([G2] p1, [N.G`1[[G1 -> G2]], TestProject] p2, [N.G`1[[G1 -> {1}]], TestProject] p3)",
                Fix.Void,
                Fix.Int);
        }

        [Test]
        public void Regression_WildCombination()
        {
            CompleteInNamespace(@"
                class Outer<T0> {
                    public class C1<T1>
                    {
                        public class C2<T2> {}
                    }

                    public class C
                    {
                        public void M(C1<int>.C2<int> p)
                        {
                            $
                        }
                    }
                }
            ");

            AssertParameterTypes("N.Outer`1[[T0]]+C1`1[[T1 -> {0}]]+C2`1[[T2 -> {0}]], TestProject".FormatEx(Fix.Int));
        }

        [Test]
        public void RecursiveDefinition1()
        {
            CompleteInNamespace(@"
                class C {
                    public delegate D D();
                    void M(D d){ $ }
                }   
            ");

            const string delType = "N.C+D, TestProject";
            AssertParameterTypes("d:[{0}] [{0}].()".FormatEx(delType));
        }

        [Test]
        public void RecursiveDefinition2()
        {
            CompleteInNamespace(@"
                class C {
                    public delegate IEnumerable<D> D();
                    void M(D d){ $ }
                }   
            ");

            const string delType = "N.C+D, TestProject";
            AssertParameterTypes(
                "d:[i:System.Collections.Generic.IEnumerable`1[[T -> {0}]], mscorlib, 4.0.0.0] [{0}].()".FormatEx(
                    delType));
        }

        [Test]
        public void RecursiveDefinition3()
        {
            CompleteInNamespace(@"
                class C1 {
                    public delegate IEnumerable<D> D();
                }
                class C2 {
                    void M(C1.D d){ $ }
                }   
            ");

            const string delType = "N.C1+D, TestProject";
            AssertParameterTypes(
                "d:[i:System.Collections.Generic.IEnumerable`1[[T -> {0}]], mscorlib, 4.0.0.0] [{0}].()".FormatEx(
                    delType));
        }

        [Test]
        public void Right_NewHandler()
        {
            CompleteInClass(@"
                private event Handler E;
                private delegate void Handler(int i);
                private void Listener(int i) { }

                public void M()
                {
                    E += new Handler(Listener);
                    $
                }
            ");

            var en = ResultSST.Methods.GetEnumerator();
            en.MoveNext(); // .Listener
            en.MoveNext(); // .M
            Assert.NotNull(en.Current);
            var stmt = en.Current.Body.FirstOrDefault(new EventSubscriptionStatement());
            var esStmt = stmt as IEventSubscriptionStatement;
            Assert.NotNull(esStmt);
            var inv = esStmt.Expression as IInvocationExpression;
            Assert.NotNull(inv);
            var actual = inv.MethodName;

            var delegateType = Names.Type("d:[{0}] [N.C+Handler, TestProject].([{1}] i)", Fix.Void, Fix.Int);
            var parameter = Names.Parameter("[{0}] target", delegateType);
            var ctor = Fix.Ctor(delegateType, parameter);

            Assert.AreEqual(ctor, actual);
        }

        [Test]
        public void Nullable()
        {
            CompleteInNamespace(@"
                class C {
                    void M(int? i){ $ }
                }   
            ");
            AssertParameterTypes("System.Nullable`1[[T -> {0}]], mscorlib, 4.0.0.0".FormatEx(Fix.Int));
        }

        [Test]
        public void GlobalNamespace()
        {
            CompleteInCSharpFile(@"
                class C {
                    void M(){ $ }
                }   
            ");
            var type = ResultSST.EnclosingType;
            Assert.AreEqual(new NamespaceName(""), type.Namespace);
            Assert.IsTrue(type.Namespace.IsGlobalNamespace);
        }

        #endregion
    }
}