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
 *    - Sven Amann
 */

using System.Linq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Utils
{
    [TestFixture]
    internal class LookupItemUtilsTest : BaseTest
    {
        private void ThenProposalCollectionContains(params string[] proposalNameIdentifiers)
        {
            foreach (var proposalNameIdentifier in proposalNameIdentifiers)
            {
                ThenProposalCollectionContains(proposalNameIdentifier);
            }
        }

        private void ThenProposalCollectionContains(string proposalNameIdentifier)
        {
            CollectionAssert.Contains(
                ResultProposalCollection.Proposals.Select(prop => prop.Name.Identifier),
                proposalNameIdentifier);
        }

        [Test]
        public void ShouldTranslateMethodParameterProposal()
        {
            CompleteInClass(@"
                public void M(int param)
                {
                    par$
                }");

            ThenProposalCollectionContains("[System.Int32, mscorlib, 4.0.0.0] param");
        }

        [Test]
        public void ShouldTranslateLocalVariableProposal()
        {
            CompleteInMethod(@"
                var loc = ""test"";
                loc$
            ");

            ThenProposalCollectionContains("[System.String, mscorlib, 4.0.0.0] loc");
        }

        [Test]
        public void ShouldTranslateLoopCounterProposal()
        {
            CompleteInMethod(@"
                for (var counter = 0; counter < 10; counter++)
                {
                    count$
                }");

            ThenProposalCollectionContains("[System.Int32, mscorlib, 4.0.0.0] counter");
        }

        [Test]
        public void ShouldTranslateCatchExceptionProposal()
        {
            CompleteInMethod(@"
                try
                {
                    throw new Exception();
                }
                catch(Exception exception)
                {
                    exc$
                }");

            ThenProposalCollectionContains("[System.Exception, mscorlib, 4.0.0.0] exception");
        }

        [Test]
        public void ShouldTranslateUsingVariableProposal()
        {
            CompleteInMethod(@"
                using (var usingVar = new MemoryStream())
                {
                    usi$
                }");

            ThenProposalCollectionContains("[System.IO.MemoryStream, mscorlib, 4.0.0.0] usingVar");
        }

        [Test]
        public void ShouldTranslateFieldProposals()
        {
            CompleteInFile(@"
                class C
                {
                    Object field;

                    void M()
                    {
                        field$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Object, mscorlib, 4.0.0.0] [C, TestProject].field");
        }

        [Test]
        public void ShouldTranslateMethodProposal()
        {
            CompleteInFile(@"
                class C
                {
                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M()");
        }

        [Test]
        public void ShouldTranslateMethodWithReturnTypeProposal()
        {
            CompleteInFile(@"
                class C
                {
                    public Object M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Object, mscorlib, 4.0.0.0] [C, TestProject].M()");
        }

        [Test]
        public void ShouldTranslateMethodWithAliasedReturnTypeProposal()
        {
            CompleteInFile(@"
                class C
                {
                    public int M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Int32, mscorlib, 4.0.0.0] [C, TestProject].M()");
        }

        [Test]
        public void ShouldTranslateMethodWithParameterProposal()
        {
            CompleteInFile(@"
                class C
                {
                    public void M(Object p)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M([System.Object, mscorlib, 4.0.0.0] p)");
        }

        [Test]
        public void ShouldTranslateMethodWithRefParameterProposal()
        {
            CompleteInFile(@"
                class C
                {
                    public void M(ref int i)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M(ref [System.Int32, mscorlib, 4.0.0.0] i)");
        }

        [Test]
        public void ShouldTranslateMethodWithOutParameterProposal()
        {
            CompleteInFile(@"
                class C
                {
                    public void M(out bool b)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M(out [System.Boolean, mscorlib, 4.0.0.0] b)");
        }

        [Test]
        public void ShouldTranslateMethodVarArgsParameterProposal()
        {
            CompleteInFile(@"
                class C
                {
                    public void M(params Object[] objs)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M(params [System.Object[], mscorlib, 4.0.0.0] objs)");
        }

        [Test]
        public void ShouldTranslateMethodWithOptionalProposals()
        {
            CompleteInFile(@"
                class C
                {
                    public void M(Object obj = null)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M(opt [System.Object, mscorlib, 4.0.0.0] obj)");
        }

        [Test]
        public void ShouldTranslateEventProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public delegate void Delegate(object obj);
                    public event Delegate Event;

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains("[d:C+Delegate, TestProject] [C, TestProject].Event");
        }

        [Test]
        public void ShouldTranslatePropertyWithGetterAndSetterProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public int Property { get; set; }

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "set get [System.Int32, mscorlib, 4.0.0.0] [C, TestProject].Property()");
        }

        [Test]
        public void ShouldTranslatePropertyWithGetterProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public int Property { get { return 0; } }

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "get [System.Int32, mscorlib, 4.0.0.0] [C, TestProject].Property()");
        }

        [Test]
        public void ShouldTranslatePropertyWithSetterProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public int Property { set {} }

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "set [System.Int32, mscorlib, 4.0.0.0] [C, TestProject].Property()");
        }

        [Test]
        public void ShouldTranslateIndexerProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public object this[int i]
                    {
                        get { return this; }
                    }

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "get [System.Object, mscorlib, 4.0.0.0] [C, TestProject].Item([System.Int32, mscorlib, 4.0.0.0] i)");
        }

        [Test]
        public void ShouldTranslateMethodWithOwnTypeParameter()
        {
            CompleteInFile(@"
                class C
                {
                    public void M<T>(T p)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains("[System.Void, mscorlib, 4.0.0.0] [C, TestProject].M[[T -> T]]([T] p)");
        }

        [Test]
        public void ShouldTranslateTypeWithNestedTypeParameters()
        {
            CompleteInFile(@"
                class C<T>
                {
                    void M(IList<C<IList<string>>> l)
                    {
                        l$
                    }
                }");

            ThenProposalCollectionContains(
                "[i:System.Collections.Generic.IList`1[[T -> C`1[[T -> i:System.Collections.Generic.IList`1[[T -> System.String, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0]], TestProject]], mscorlib, 4.0.0.0] l");
        }

        [Test]
        public void ShouldTranslateTypeWithMultipleTypeParameters()
        {
            CompleteInMethod(@"
                IDictionary<string, object> d;
                d$
            ");

            ThenProposalCollectionContains(
                "[i:System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, 4.0.0.0],[TValue -> System.Object, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0] d");
        }

        [Test]
        public void ShouldTranslateNullableTypeAlias()
        {
            CompleteInClass(@"
                void M(int? i)
                {
                    i$
                }");

            ThenProposalCollectionContains(
                "[System.Nullable`1[[T -> System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0] i");
        }

        [Test]
        public void ShouldTranslateGenericType()
        {
            CompleteInMethod(@"
                IList<string> l;
                l$
            ");

            ThenProposalCollectionContains("[i:System.Collections.Generic.IList`1[[T -> System.String, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0] l");
        }

        [Test]
        public void ShouldTranslateMethodWithTypeParameterFromEnclosingType()
        {
            CompleteInFile(@"
                class C<T>
                {
                    private T GetT()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[T] [C`1[[T -> T]], TestProject].GetT()");
        }

        [Test]
        public void ShouldTranslateProposalWithInstantiatedTypeParameters()
        {
            CompleteInMethod(@"
                IDictionary<string, object> dict;
                dict.$
            ");

            ThenProposalCollectionContains(
                "[System.Void, mscorlib, 4.0.0.0] [i:System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, 4.0.0.0],[TValue -> System.Object, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0].Add([TKey] key, [TValue] value)",
                "get [System.Int32, mscorlib, 4.0.0.0] [i:System.Collections.Generic.ICollection`1[[T -> s:System.Collections.Generic.KeyValuePair`2[[TKey -> System.String, mscorlib, 4.0.0.0],[TValue -> System.Object, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0].Count()",
                "get [i:System.Collections.Generic.ICollection`1[[T -> TKey]], mscorlib, 4.0.0.0] [i:System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, 4.0.0.0],[TValue -> System.Object, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0].Keys()",
                "get [i:System.Collections.Generic.ICollection`1[[T -> TValue]], mscorlib, 4.0.0.0] [i:System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, 4.0.0.0],[TValue -> System.Object, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0].Values()",
                "set get [TValue] [i:System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, 4.0.0.0],[TValue -> System.Object, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0].Item([TKey] key)");
        }

        [Test]
        public void ShouldTranslateStaticConstantProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public const string Constant = ""constant"";

                    public void M()
                    {
                        C.$
                    }
                }");

            ThenProposalCollectionContains("static [System.String, mscorlib, 4.0.0.0] [C, TestProject].Constant");
        }

        [Test]
        public void ShouldTranslateNestedTypeProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public class N { }

                    public void M()
                    {
                        C.$
                    }
                }");

            ThenProposalCollectionContains("C+N, TestProject");
        }

        [Test]
        public void ShouldTranslateStaticMethodProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public static bool M()
                    {
                        C.$
                    }
                }");

            ThenProposalCollectionContains("static [System.Boolean, mscorlib, 4.0.0.0] [C, TestProject].M()");
        }

        [Test]
        public void ShouldTranslateStaticFieldProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    static string _field;

                    public static bool M()
                    {
                        C.$
                    }
                }");

            ThenProposalCollectionContains("static [System.String, mscorlib, 4.0.0.0] [C, TestProject]._field");
        }

        [Test]
        public void ShouldTranslateNamespaceProposals()
        {
            CompleteInFile(@"
                namespace N
                {
                    public class C
                    {
                        public void M()
                        {
                            N.$
                        }
                    }

                    namespace M
                    {
                    }
                }");

            ThenProposalCollectionContains("N.M");
        }

        [Test]
        public void ShouldTranslateConstructorWithoutArgsProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public C() {}

                    public void M()
                    {
                        new C$
                    }
                }");

            ThenProposalCollectionContains("[C, TestProject] [C, TestProject]..ctor()");
        }

        [Test]
        public void ShouldTranslateConstructorWithArgsProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public C(object o) {}

                    public void M()
                    {
                        new C$
                    }
                }");

            ThenProposalCollectionContains("[C, TestProject] [C, TestProject]..ctor()");
        }

        [Test]
        public void ShouldTranslateImplicitConstructorProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public void M()
                    {
                        new C$
                    }
                }");

            ThenProposalCollectionContains("[C, TestProject] [C, TestProject]..ctor()");
        }

        [Test]
        public void ShouldTranslateConstructorOfGenericTypeProposal1()
        {
            CompleteInFile(@"
                public class MyTestClass<T>
                {
                    public void M()
                    {
                        new MyTestC$
                    }
                }");

            // completion proposes the generic type instead of the constructor in this case
            ThenProposalCollectionContains("MyTestClass`1[[T -> T]], TestProject");
        }

        [Test]
        public void ShouldTranslateConstructorOfGenericTypeProposal2()
        {
            CompleteInFile(@"
                public class MyTestClass<T>
                {
                    public void M()
                    {
                        new MyTestClass<int>$
                    }
                }");

            // completion proposes the generic type instead of the constructor in this case
            ThenProposalCollectionContains("MyTestClass`1[[T -> T]], TestProject");
        }

        [Test]
        public void ShouldTranslateStructConstructorProposal()
        {
            CompleteInFile(@"
                public class C
                {
                    public void M()
                    {
                        new SomeS$
                    }
                }

                public struct SomeStruct
                {
                    public SomeStruct() {}
                }
            ");

            ThenProposalCollectionContains(
                "[s:SomeStruct, TestProject] [s:SomeStruct, TestProject]..ctor()");
        }

        [Test]
        public void ShouldTranslateNewArrayInstanceProposals()
        {
            CompleteInFile(@"
                public class C
                {
                    public void M()
                    {
                        C[] props = new$
                    }
                }");

            ThenProposalCollectionContains(
                "CombinedLookupItem:new C[]",
                "CombinedLookupItem:new [] { }");
        }

        [Test]
        public void ShouldTranslateArrayTypeProposals()
        {
            CompleteInFile(@"
                public class C
                {
                    private string[] myStringArray;
                    private object[,,] myMultidimensionalArray;
                    private object[][][] myJaggedArray;

                    private void myMethod<R>(R[] p) {}

                    public void TriggerCompletionHerein()
                    {
                        this.my$
                    }
                }");

            ThenProposalCollectionContains(
                "[System.Object[][][], mscorlib, 4.0.0.0] [C, TestProject].myJaggedArray",
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].myMethod[[R -> R]]([R[]] p)",
                "[System.Object[,,], mscorlib, 4.0.0.0] [C, TestProject].myMultidimensionalArray",
                "[System.String[], mscorlib, 4.0.0.0] [C, TestProject].myStringArray");
        }

        [Test]
        public void ShouldTranslateProposalsForOverloadedMethod()
        {
            CompleteInFile(@"
                public class C
                {
                    public void M()
                    {
                        this.MyMeth$
                    }

                    private void MyMethod(int i) { }
                    private void MyMethod() {}
                    private void MyMethod(string s) {}
                }");

            // Actually, for overloaded methods only one proposal shows up in the completion list, which is
            // by default showing the "first" overloading. When it is selected, one can cycle through the
            // overloads.
            ThenProposalCollectionContains(
                "[System.Void, mscorlib, 4.0.0.0] [C, TestProject].MyMethod([System.Int32, mscorlib, 4.0.0.0] i)");
        }

        [Test]
        public void ShouldTranslateOverrideProposals()
        {
            CompleteInClass(@"
                Equa$
            ");

            ThenProposalCollectionContains("CombinedLookupItem:public override Equals(object) { ... }");
        }

        [Test]
        public void ShouldTranslateMethodWithTypeParametersProposals()
        {
            CompleteInFile(@"namespace N
                {
                    class C
                    {
                        public void M1<T>(T t)
                        {
                            $
                        }
                    }
                }");

            ThenProposalCollectionContains("[System.Void, mscorlib, 4.0.0.0] [N.C, TestProject].M1[[T -> T]]([T] t)");
        }

        [Test]
        public void ShouldTranlateUnresolvableFieldReference()
        {
            CompleteInMethod(@"
                v = $
            ");

            ThenProposalCollectionContains("[?] [?].v");
        }

        [Test]
        public void ShouldFlagEnumTypesAsSuch()
        {
            CompleteInFile(@"
                enum MyEnum
                {
                    EnumValue
                }
                
                class C
                {
                    void M()
                    {
                        MyEnum.$
                    }
                }");

            ThenProposalCollectionContains("static [e:MyEnum, TestProject] [e:MyEnum, TestProject].EnumValue");
        }

        [Test]
        public void ShouldFlagInterfaceTypesAsSuch()
        {
            CompleteInFile(@"
                interface MyInterface
                {
                    public void M();
                }
                
                class C
                {
                    void M(MyInterface i)
                    {
                        i.$
                    }
                }");

            ThenProposalCollectionContains("[System.Void, mscorlib, 4.0.0.0] [i:MyInterface, TestProject].M()");
        }

        [Test]
        public void ShouldFlagCustomStructAsSuch()
        {
            CompleteInFile(@"
                struct MyStruct
                {
                    public int i;
                }
                
                class C
                {
                    void M(MyStruct s)
                    {
                        s.$
                    }
                }");

            ThenProposalCollectionContains("[System.Int32, mscorlib, 4.0.0.0] [s:MyStruct, TestProject].i");
        }

        [Test]
        public void ShouldFlagDelegateAsSuch()
        {
            CompleteInFile(@"
                class C
                {
                    delegate void D(int i);
                    
                    void M(D d)
                    {
                        d.$
                    }
                }");

            ThenProposalCollectionContains("[System.Void, mscorlib, 4.0.0.0] [d:C+D, TestProject].Invoke([System.Int32, mscorlib, 4.0.0.0] i)");
        }

        // Test cases for keywords (e.g., private), templates (e.g., for), and
        // combined proposals (e.g., return true) donnot seem possible, as such
        // proposals are not made by the test environment's completion engine.
    }
}