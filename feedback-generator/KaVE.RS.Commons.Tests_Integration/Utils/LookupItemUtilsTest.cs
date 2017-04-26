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

using System;
using System.Linq;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using NUnit.Framework;
using Fix = KaVE.Commons.TestUtils.Model.Naming.NameFixture;

namespace KaVE.RS.Commons.Tests_Integration.Utils
{
    internal class LookupItemUtilsTest : BaseCSharpCodeCompletionTest
    {
        [TearDown]
        public void DisableTestProposalProvider()
        {
            TestProposalProvider.Enabled = false;
        }

        private void ThenProposalCollectionContainsMulti(params string[] proposalNameIdentifiers)
        {
            foreach (var proposalNameIdentifier in proposalNameIdentifiers)
            {
                ThenProposalCollectionContains(proposalNameIdentifier);
            }
        }

        [StringFormatMethod("formatIdentifier")]
        private void ThenProposalCollectionContains(string formatIdentifier, params object[] args)
        {
            var expectedId = args.Length > 0 ? formatIdentifier.FormatEx(args) : formatIdentifier;
            // ReSharper disable once PossibleNullReferenceException
            var proposalIds = ResultProposalCollection.Proposals.Select(prop => prop.Name.Identifier).ToList();
            if (!proposalIds.Contains(expectedId))
            {
                foreach (var p in proposalIds)
                {
                    Console.WriteLine("* {0}", p);
                }
                Assert.Fail("expected identifier was not captured in proposal list");
            }
        }

        [Test]
        public void ShouldTranslateMethodParameterProposal()
        {
            CompleteInClass(@"
                public void M(int param)
                {
                    par$
                }");

            ThenProposalCollectionContains("[{0}] param", Fix.Int);
        }

        [Test]
        public void ShouldTranslateLocalVariableProposal()
        {
            CompleteInMethod(@"
                var loc = ""test"";
                loc$
            ");

            ThenProposalCollectionContains("[{0}] loc", Fix.String);
        }

        [Test]
        public void ShouldTranslateLoopCounterProposal()
        {
            CompleteInMethod(@"
                for (var counter = 0; counter < 10; counter++)
                {
                    count$
                }");

            ThenProposalCollectionContains("[{0}] counter", Fix.Int);
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
            CompleteInCSharpFile(@"
                class C
                {
                    Object field;

                    void M()
                    {
                        field$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject].field", Fix.Object);
        }

        [Test]
        public void ShouldTranslateMethodProposal()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject].M()", Fix.Void);
        }

        [Test]
        public void ShouldTranslateMethodWithReturnTypeProposal()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public Object M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject].M()", Fix.Object);
        }

        [Test]
        public void ShouldTranslateMethodWithAliasedReturnTypeProposal()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public int M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject].M()", Fix.Int);
        }

        [Test]
        public void ShouldTranslateMethodWithParameterProposal()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public void M(Object p)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject].M([{1}] p)", Fix.Void, Fix.Object);
        }

        [Test]
        public void ShouldTranslateMethodWithRefParameterProposal()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public void M(ref int i)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[{0}] [C, TestProject].M(ref [{1}] i)",
                Fix.Void,
                Fix.Int);
        }

        [Test]
        public void ShouldTranslateMethodWithOutParameterProposal()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public void M(out bool b)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[{0}] [C, TestProject].M(out [{1}] b)",
                Fix.Void,
                Fix.Bool);
        }

        [Test]
        public void ShouldTranslateMethodVarArgsParameterProposal()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public void M(params Object[] objs)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[{0}] [C, TestProject].M(params [{1}] objs)",
                Fix.Void,
                Fix.ObjectArr(1));
        }

        [Test]
        public void ShouldTranslateMethodWithOptionalProposals()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public void M(Object obj = null)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[{0}] [C, TestProject].M(opt [{1}] obj)",
                Fix.Void,
                Fix.Object);
        }

        [Test]
        public void ShouldTranslateEventProposal()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    public delegate void Delegate(object obj);
                    public event Delegate Event;

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[d:[{0}] [C+Delegate, TestProject].([{1}] obj)] [C, TestProject].Event",
                Fix.Void,
                Fix.Object);
        }

        [Test]
        public void ShouldTranslatePropertyWithGetterAndSetterProposal()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    public int Property { get; set; }

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "set get [{0}] [C, TestProject].Property()",
                Fix.Int);
        }

        [Test]
        public void ShouldTranslatePropertyWithGetterProposal()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    public int Property { get { return 0; } }

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "get [{0}] [C, TestProject].Property()",
                Fix.Int);
        }

        [Test]
        public void ShouldTranslatePropertyWithSetterProposal()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    public int Property { set {} }

                    public void M()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "set [{0}] [C, TestProject].Property()",
                Fix.Int);
        }

        [Test]
        public void ShouldTranslateIndexerProposal()
        {
            CompleteInCSharpFile(@"
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
                "get [{0}] [C, TestProject].Item([{1}] i)",
                Fix.Object,
                Fix.Int);
        }

        [Test]
        public void ShouldTranslateMethodWithOwnTypeParameter()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    public void M<T>(T p)
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject].M`1[[T]]([T] p)", Fix.Void);
        }

        [Test]
        public void ShouldTranslateTypeWithNestedTypeParameters()
        {
            CompleteInCSharpFile(@"
                class C<T>
                {
                    void M(C<C<string>> ccs)
                    {
                        cc$
                    }
                }");

            ThenProposalCollectionContains(
                "[C`1[[T -> C`1[[T -> {0}]], TestProject]], TestProject] ccs",
                Fix.String);
        }

        [Test]
        public void ShouldCaptureMultipleSubstitutionsForSameType()
        {
            CompleteInCSharpFile(@"
                interface I<T> {}

                class C
                {
                    void M(I<string> ls, I<int> li)
                    {
                        M$
                    }
                }");

            ThenProposalCollectionContains(
                "[{0}] [C, TestProject].M([i:I`1[[T -> {1}]], TestProject] ls, [i:I`1[[T -> {2}]], TestProject] li)",
                Fix.Void,
                Fix.String,
                Fix.Int);
        }

        [Test]
        public void ShouldTranslateTypeWithMultipleTypeParameters()
        {
            CompleteInMethod(@"
                IDictionary<string, object> d;
                d$
            ");

            ThenProposalCollectionContains(
                "[i:System.Collections.Generic.IDictionary`2[[TKey -> {0}],[TValue -> {1}]], mscorlib, 4.0.0.0] d",
                Fix.String,
                Fix.Object);
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
                "[s:System.Nullable`1[[T -> {0}]], mscorlib, 4.0.0.0] i",
                Fix.Int);
        }

        [Test]
        public void ShouldTranslateGenericType()
        {
            CompleteInMethod(@"
                IList<string> l;
                l$
            ");

            ThenProposalCollectionContains(
                "[i:System.Collections.Generic.IList`1[[T -> {0}]], mscorlib, 4.0.0.0] l",
                Fix.String);
        }

        [Test]
        public void ShouldTranslateMethodWithTypeParameterFromEnclosingType()
        {
            CompleteInCSharpFile(@"
                class C<T>
                {
                    private T GetT()
                    {
                        this.$
                    }
                }");

            ThenProposalCollectionContains(
                "[T] [C`1[[T]], TestProject].GetT()");
        }

        [Test]
        public void ShouldTranslateProposalWithInstantiatedTypeParameters()
        {
            CompleteInMethod(@"
                IDictionary<string, object> dict;
                dict.$
            ");

            ThenProposalCollectionContainsMulti(
                "[{0}] [i:System.Collections.Generic.IDictionary`2[[TKey -> {1}],[TValue -> {2}]], mscorlib, 4.0.0.0].Add([TKey] key, [TValue] value)"
                    .FormatEx(Fix.Void, Fix.String, Fix.Object),
                "get [{0}] [i:System.Collections.Generic.ICollection`1[[T -> s:System.Collections.Generic.KeyValuePair`2[[TKey -> {1}],[TValue -> {2}]], mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0].Count()"
                    .FormatEx(Fix.Int, Fix.String, Fix.Object),
                "get [i:System.Collections.Generic.ICollection`1[[T -> TKey]], mscorlib, 4.0.0.0] [i:System.Collections.Generic.IDictionary`2[[TKey -> {0}],[TValue -> {1}]], mscorlib, 4.0.0.0].Keys()"
                    .FormatEx(Fix.String, Fix.Object),
                "get [i:System.Collections.Generic.ICollection`1[[T -> TValue]], mscorlib, 4.0.0.0] [i:System.Collections.Generic.IDictionary`2[[TKey -> {0}],[TValue -> {1}]], mscorlib, 4.0.0.0].Values()"
                    .FormatEx(Fix.String, Fix.Object),
                // for the indexers R# doesn't give us the type parameter information (Substitution is Identity, instead of map to concrete types)
                "set get [TValue] [i:System.Collections.Generic.IDictionary`2[[TKey],[TValue]], mscorlib, 4.0.0.0].Item([TKey] key)");
        }

        [Test]
        public void ShouldTranslateStaticConstantProposal()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    public const string Constant = ""constant"";

                    public void M()
                    {
                        C.$
                    }
                }");

            ThenProposalCollectionContains("static [{0}] [C, TestProject].Constant", Fix.String);
        }

        [Test]
        public void ShouldTranslateNestedTypeProposal()
        {
            CompleteInCSharpFile(@"
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
            CompleteInCSharpFile(@"
                public class C
                {
                    public static bool M()
                    {
                        C.$
                    }
                }");

            ThenProposalCollectionContains("static [{0}] [C, TestProject].M()", Fix.Bool);
        }

        [Test]
        public void ShouldTranslateStaticFieldProposal()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    static string _field;

                    public static bool M()
                    {
                        C.$
                    }
                }");

            ThenProposalCollectionContains("static [{0}] [C, TestProject]._field", Fix.String);
        }

        [Test]
        public void ShouldTranslateNamespaceProposals()
        {
            CompleteInCSharpFile(@"
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
            CompleteInCSharpFile(@"
                public class C
                {
                    public C() {}

                    public void M()
                    {
                        new C$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject]..ctor()", Fix.Void);
        }

        [Test, Ignore]
        public void ShouldTranslateConstructorWithArgsProposal()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    public C(object o) {}

                    public void M()
                    {
                        new C$
                    }
                }");

            // TODO @Sven: how do we fix this inconsistency of ReSharper?
            // -> even though the UI shows a constructor, the declared element points to a class :/
            ThenProposalCollectionContains(
                "[{0}] [C, TestProject]..ctor([{1}] o)",
                Fix.Void,
                Fix.Object);
        }

        [Test]
        public void ShouldTranslateImplicitConstructorProposal()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    public void M()
                    {
                        new C$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject]..ctor()", Fix.Void);
        }

        [Test]
        public void ShouldTranslateConstructorOfGenericTypeProposal1()
        {
            CompleteInCSharpFile(@"
                public class MyTestClass<T>
                {
                    public void M()
                    {
                        new MyTestC$
                    }
                }");

            // completion proposes a constructor in this case
            const string typeName = "MyTestClass`1[[T]], TestProject";
            ThenProposalCollectionContains("[{0}] [{1}]..ctor()", Fix.Void, typeName);
        }

        [Test]
        public void ShouldTranslateConstructorOfGenericTypeProposal2()
        {
            CompleteInCSharpFile(@"
                public class MyTestClass<T>
                {
                    public void M()
                    {
                        new MyTestClass<int>$
                    }
                }");

            // completion proposes the generic type in this case
            ThenProposalCollectionContains("MyTestClass`1[[T]], TestProject");
        }

        [Test]
        public void ShouldTranslateStructConstructorProposal()
        {
            CompleteInCSharpFile(@"
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
                "[{0}] [s:SomeStruct, TestProject]..ctor()",
                Fix.Void);
        }

        [Test]
        public void ShouldTranslateNewArrayInstanceProposals()
        {
            CompleteInCSharpFile(@"
                public class C
                {
                    public void M()
                    {
                        C[] props = new$
                    }
                }");

            ThenProposalCollectionContainsMulti(
                "CSharpCombinedLookupItem:new C[]",
                "CSharpCombinedLookupItem:new [] { }");
        }

        [Test]
        public void ShouldTranslateArrayTypeProposals()
        {
            CompleteInCSharpFile(@"
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

            ThenProposalCollectionContainsMulti(
                "[{0}] [C, TestProject].myJaggedArray".FormatEx(Fix.ObjectArr(3)),
                "[{0}] [C, TestProject].myMethod`1[[R]]([R[]] p)".FormatEx(Fix.Void),
                "[{0}] [C, TestProject].myMultidimensionalArray".FormatEx(Fix.ObjectArr(3)),
                "[{0}] [C, TestProject].myStringArray".FormatEx(Fix.StringArr(1)));
        }

        [Test]
        public void ShouldTranslateProposalsForOverloadedMethod()
        {
            CompleteInCSharpFile(@"
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
            ThenProposalCollectionContains("[{0}] [C, TestProject].MyMethod([{1}] i)", Fix.Void, Fix.Int);
        }

        [Test]
        public void ShouldTranslateOverrideProposals()
        {
            CompleteInClass(@"
                Equa$
            ");

            // ReSharper disable once FormatStringProblem
            // second parameter is required to not break string.Format
            ThenProposalCollectionContains("CombinedLookupItem:public override Equals(object) {{ ... }}", "");
        }

        [Test]
        public void ShouldTranslateMethodWithTypeParametersProposals()
        {
            CompleteInCSharpFile(@"namespace N
                {
                    class C
                    {
                        public void M1<T>(T t)
                        {
                            $
                        }
                    }
                }");

            ThenProposalCollectionContains("[{0}] [N.C, TestProject].M1`1[[T]]([T] t)", Fix.Void);
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
            CompleteInCSharpFile(@"
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
            CompleteInCSharpFile(@"
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

            ThenProposalCollectionContains("[{0}] [i:MyInterface, TestProject].M()", Fix.Void);
        }

        [Test]
        public void ShouldFlagCustomStructAsSuch()
        {
            CompleteInCSharpFile(@"
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

            ThenProposalCollectionContains("[{0}] [s:MyStruct, TestProject].i", Fix.Int);
        }

        [Test]
        public void ShouldFlagDelegateAsSuch()
        {
            CompleteInCSharpFile(@"
                class C
                {
                    delegate void D(int i);
                    
                    void M(D d)
                    {
                        d.$
                    }
                }");

            ThenProposalCollectionContains(
                "[{0}] [d:[{0}] [C+D, TestProject].([{1}] i)].Invoke([{1}] i)",
                Fix.Void,
                Fix.Int);
        }

        [Test]
        public void ShouldResolveArraysOfGenericTypes()
        {
            CompleteInCSharpFile(@"
                class C {
                    void M<T>(T[] ts) {
                        this.$
                    }
                }");

            ThenProposalCollectionContains("[{0}] [C, TestProject].M`1[[T]]([T[]] ts)", Fix.Void);
        }

        [Test]
        public void ShouldNotCrashOnCompletionInXaml()
        {
            CompleteInFile(@"<Window x:Class=""WpfApplication1.MainWindow""
                                     xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                     xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                     Title=""MainWindow"" Height=""350"" Width=""525"">
               <Grid>
                 $
               </Grid>
            </Window>", "xaml");
        }

        [Test]
        public void ShouldHandleUnknownLookupItemType()
        {
            TestProposalProvider.Enabled = true;

            CompleteInMethod("$");
        }

        [Test]
        public void TranslatesArrayOfGenericType()
        {
            CompleteInFile(@"
                class C
                {
                    public System.Collections.Generic.IList<string>[] M() { return null; }

                    public void N()
                    {
                        $
                    }
                }");

            ThenProposalCollectionContains(
                "[i:System.Collections.Generic.IList`1[][[T -> {0}]], mscorlib, 4.0.0.0] [C, TestProject].M()",
                Fix.String);
        }

        [Test]
        public void ShouldCaptureTextualLookupItems()
        {
            CompleteInMethod("$");
            ThenProposalCollectionContains("text:async");
        }

        [Test]
        public void ShouldFallBackOnGenericNameForUnknownLookupItems()
        {
            CompleteInCSharpFile(@"
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

            ThenProposalCollectionContains("LookupItem`1[[PostfixTemplateInfo]]:notnull");
        }

        [Test]
        public void ShouldCaptureTypeLookupItems()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    class C {}
                    class Test {
                        public void M() {
                            C$
                        }
                    }
                }
            ");
            ThenProposalCollectionContains("N.C, TestProject");
        }

        [Test]
        public void ShouldCaptureDelegateLookupItems()
        {
            CompleteInCSharpFile(@"
                namespace N {
                    delegate int D(double d);
                    class Test {
                        public void M() {
                            D$
                        }
                    }
                }
            ");
            ThenProposalCollectionContains("d:[p:int] [N.D, TestProject].([p:double] d)");
        }
    }
}