using System.Linq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Utils
{
    [TestFixture]
    internal class LookupItemUtilsTest : KaVEBaseTest
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
        public void ShouldTranslateProposals()
        {
            WhenCodeCompletionIsInvokedInFile("ProofOfConcept");

            ThenProposalCollectionContains(
                "[System.Boolean, mscorlib, Version=4.0.0.0] [System.Object, mscorlib, Version=4.0.0.0].Equals([System.Object, mscorlib, Version=4.0.0.0] obj)",
                "[System.Int32, mscorlib, Version=4.0.0.0] [System.Object, mscorlib, Version=4.0.0.0].GetHashCode()",
                "[System.Type, mscorlib, Version=4.0.0.0] [System.Object, mscorlib, Version=4.0.0.0].GetType()",
                "[System.Object, mscorlib, Version=4.0.0.0] [System.Object, mscorlib, Version=4.0.0.0].MemberwiseClone()",
                "[System.Void, mscorlib, Version=4.0.0.0] [TestTargets.SomeClass, TestProject].Method()",
                "[System.String, mscorlib, Version=4.0.0.0] [System.Object, mscorlib, Version=4.0.0.0].ToString()");
        }

        [Test]
        public void ShouldTranslateVariableProposals()
        {
            WhenCodeCompletionIsInvokedInFile("VariableProposals");

            ThenProposalCollectionContains(
                "[System.Exception, mscorlib, Version=4.0.0.0] var_Exception",
                "[System.Object, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.VariableProposals, TestProject].var_Field",
                "[System.Int32, mscorlib, Version=4.0.0.0] var_Index",
                "[System.Int32, mscorlib, Version=4.0.0.0] var_Param",
                "[System.String, mscorlib, Version=4.0.0.0] var_Str",
                "[System.IO.MemoryStream, mscorlib, Version=4.0.0.0] var_Using");
        }

        [Test]
        public void ShouldTranslateMethodProposals()
        {
            WhenCodeCompletionIsInvokedInFile("MethodProposals");

            ThenProposalCollectionContains(
                "[System.Int32, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MethodProposals, TestProject].MyMethodWithAliasedReturnType()",
                "[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MethodProposals, TestProject].MyMethodWithOptionalParameter(opt [System.Object, mscorlib, Version=4.0.0.0] obj)",
                "[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MethodProposals, TestProject].MyMethodWithOutParameter(out [System.Boolean, mscorlib, Version=4.0.0.0] b)",
                "[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MethodProposals, TestProject].MyMethodWithParamArray(params [System.Object[], mscorlib, Version=4.0.0.0] objs)",
                "[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MethodProposals, TestProject].MyMethodWithParameter([System.Object, mscorlib, Version=4.0.0.0] param)",
                "[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MethodProposals, TestProject].MyMethodWithRefParameter(ref [System.Int32, mscorlib, Version=4.0.0.0] i)",
                "[System.Object, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MethodProposals, TestProject].MyMethodWithReturnType()");
        }

        [Test]
        public void ShouldTranslateMemberProposalsOfAllKinds()
        {
            WhenCodeCompletionIsInvokedInFile("MemberKindProposals");

            ThenProposalCollectionContains("[CodeExamples.CompletionProposals.MemberKindProposals+Delegate, TestProject] [CodeExamples.CompletionProposals.MemberKindProposals, TestProject].Event",
                "[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MemberKindProposals, TestProject].Method([System.Object, mscorlib, Version=4.0.0.0] param)",
                "set get [System.Int32, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MemberKindProposals, TestProject].Property()",
                "get [System.Object, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MemberKindProposals, TestProject].Item([System.Int32, mscorlib, Version=4.0.0.0] i)",
                "[System.String, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MemberKindProposals, TestProject]._field");
        }

        [Test]
        public void ShouldTranslateGenericTypeProposals()
        {
            WhenCodeCompletionIsInvokedInFile("GenericTypeProposals");

            ThenProposalCollectionContains("[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.GenericTypeProposals`1[[T -> T -> ?]], TestProject].TriggerCompletionHerein([A -> ?] param)",
                "[System.Collections.Generic.IList`1[[T -> CodeExamples.CompletionProposals.GenericTypeProposals`1[[T -> System.Collections.Generic.IList`1[[T -> System.String, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0]], TestProject]], mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.GenericTypeProposals`1[[T -> T -> ?]], TestProject]._complexList",
                "[System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.GenericTypeProposals`1[[T -> T -> ?]], TestProject]._dictionary",
                "[System.Nullable`1[[T -> System.Int32, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.GenericTypeProposals`1[[T -> T -> ?]], TestProject]._nullableInt",
                "[System.Collections.Generic.IList`1[[T -> System.String, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.GenericTypeProposals`1[[T -> T -> ?]], TestProject]._simpleList");
        }

        [Test]
        public void ShouldTranslateProposalWithInstantiatedTypeParameters()
        {
            WhenCodeCompletionIsInvokedInFile("GenericTypeInstanceProposals");

            ThenProposalCollectionContains("[System.Void, mscorlib, Version=4.0.0.0] [System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].Add([TKey -> ?] key, [TValue -> ?] value)",
                "[System.Void, mscorlib, Version=4.0.0.0] [System.Collections.Generic.ICollection`1[[T -> System.Collections.Generic.KeyValuePair`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].Clear()",
                "[System.Boolean, mscorlib, Version=4.0.0.0] [System.Collections.Generic.ICollection`1[[T -> System.Collections.Generic.KeyValuePair`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].Contains([T -> ?] item)",
                "[System.Boolean, mscorlib, Version=4.0.0.0] [System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].ContainsKey([TKey -> ?] key)",
                "[System.Void, mscorlib, Version=4.0.0.0] [System.Collections.Generic.ICollection`1[[T -> System.Collections.Generic.KeyValuePair`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].CopyTo([T[] -> ?] array, [System.Int32, mscorlib, Version=4.0.0.0] arrayIndex)",
                "get [System.Int32, mscorlib, Version=4.0.0.0] [System.Collections.Generic.ICollection`1[[T -> System.Collections.Generic.KeyValuePair`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].Count()",
                "[System.Collections.Generic.IEnumerator`1[[T -> T -> ?]], mscorlib, Version=4.0.0.0] [System.Collections.Generic.IEnumerable`1[[T -> System.Collections.Generic.KeyValuePair`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].GetEnumerator()",
                "get [System.Boolean, mscorlib, Version=4.0.0.0] [System.Collections.Generic.ICollection`1[[T -> System.Collections.Generic.KeyValuePair`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].IsReadOnly()",
                "get [System.Collections.Generic.ICollection`1[[T -> TKey -> ?]], mscorlib, Version=4.0.0.0] [System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].Keys()",
                "[System.Boolean, mscorlib, Version=4.0.0.0] [System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].Remove([TKey -> ?] key)",
                "[System.Boolean, mscorlib, Version=4.0.0.0] [System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].TryGetValue([TKey -> ?] key, out [TValue -> ?] value)",
                "get [System.Collections.Generic.ICollection`1[[T -> TValue -> ?]], mscorlib, Version=4.0.0.0] [System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].Values()",
                "set get [TValue -> ?] [System.Collections.Generic.IDictionary`2[[TKey -> System.String, mscorlib, Version=4.0.0.0],[TValue -> System.Object, mscorlib, Version=4.0.0.0]], mscorlib, Version=4.0.0.0].Item([TKey -> ?] key)");
        }

        [Test]
        public void ShouldTranslateStaticMembers()
        {
            WhenCodeCompletionIsInvokedInFile("StaticMemberProposals");

            ThenProposalCollectionContains("static [System.String, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.StaticMemberProposals, TestProject].Constant",
                "CodeExamples.CompletionProposals.StaticMemberProposals+Nested, TestProject",
                "static [System.Boolean, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.StaticMemberProposals, TestProject].StaticMethod()",
                "static [System.String, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.StaticMemberProposals, TestProject]._field");
        }

        [Test]
        public void ShouldTranslateNamespaceProposals()
        {
            WhenCodeCompletionIsInvokedInFile("NamespaceProposals");

            ThenProposalCollectionContains("CodeExamples.CompletionProposals.SubNamespace");
        }

        [Test]
        public void ShouldTranslateNewInstanceProposals()
        {
            WhenCodeCompletionIsInvokedInFile("NewInstanceProposals");

            ThenProposalCollectionContains("[CodeExamples.CompletionProposals.ClassWithArgsConstructor, TestProject] [CodeExamples.CompletionProposals.ClassWithArgsConstructor, TestProject]..ctor()",
                "[CodeExamples.CompletionProposals.ClassWithExplicitNoArgsConstructor, TestProject] [CodeExamples.CompletionProposals.ClassWithExplicitNoArgsConstructor, TestProject]..ctor()",
                "[CodeExamples.CompletionProposals.ClassWithImplicitConstructor, TestProject] [CodeExamples.CompletionProposals.ClassWithImplicitConstructor, TestProject]..ctor()",
                "[CodeExamples.CompletionProposals.NewInstanceProposals, TestProject] [CodeExamples.CompletionProposals.NewInstanceProposals, TestProject]..ctor()",
                "[CodeExamples.CompletionProposals.Struct, TestProject] [CodeExamples.CompletionProposals.Struct, TestProject]..ctor()");
        }

        [Test]
        public void ShouldTranslateNewArrayInstanceProposals()
        {
            WhenCodeCompletionIsInvokedInFile("NewArrayInstanceProposals");

            ThenProposalCollectionContains("CombinedLookupItem:new NewArrayInstanceProposals[]",
                "CombinedLookupItem:new [] { }");
        }

        [Test]
        public void ShouldTranslateArrayTypeProposals()
        {
            WhenCodeCompletionIsInvokedInFile("ArrayTypeProposals");

            ThenProposalCollectionContains("[System.Object[][][], mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.ArrayTypeProposals, TestProject].myJaggedArray",
                "[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.ArrayTypeProposals, TestProject].myMethod([R[] -> ?] p)",
                "[System.Object[,,], mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.ArrayTypeProposals, TestProject].myMultidimensionalArray",
                "[System.String[], mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.ArrayTypeProposals, TestProject].myStringArray");
        }

        [Test]
        public void ShouldTranslateProposalsFromUncompilableFile()
        {
            WhenCodeCompletionIsInvokedInFile("UncompilableFileProposals");

            ThenProposalCollectionContains("[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.UncompilableFileProposals, TestProject].Method()");
        }

        [Test]
        public void ShouldTranslateProposalsForOverloadedMethod()
        {
            WhenCodeCompletionIsInvokedInFile("MethodOverloadProposals");

            // Actually, for overloaded methods only one proposal shows up in the completion list, which is
            // by default showing the "first" overloading. When it is selected, one can cycle through the
            // overloads.
            ThenProposalCollectionContains("[System.Void, mscorlib, Version=4.0.0.0] [CodeExamples.CompletionProposals.MethodOverloadProposals, TestProject].MyMethod([System.Int32, mscorlib, Version=4.0.0.0] i)");
        }

        [Test]
        public void ShouldTranslateClassLevelProposals()
        {
            WhenCodeCompletionIsInvokedInFile("ClassLevelProposals");

            ThenProposalCollectionContains("System.IEquatable`1[[T -> T -> ?]], mscorlib, Version=4.0.0.0",
                "CombinedLookupItem:public override Equals(object) { ... }",
                "System.Collections.Generic.EqualityComparer`1[[T -> T -> ?]], mscorlib, Version=4.0.0.0",  
                "System.Collections.IEqualityComparer, mscorlib, Version=4.0.0.0",
                "System.Collections.Generic.IEqualityComparer`1[[T -> T -> ?]], mscorlib, Version=4.0.0.0",
                "System.Collections.IStructuralEquatable, mscorlib, Version=4.0.0.0");
        }

        // Test cases for keywords (e.g., private), templates (e.g., for), and
        // combined proposals (e.g., return true) donnot seem possible, as such
        // proposals are not made by the test environment's completion engine.
    }
}