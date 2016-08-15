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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Naming;
using KaVE.JetBrains.Annotations;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Naming
{
    internal class AnonymousNameUtilsTest
    {
        [Test]
        public void ShouldAnonymizeStrings()
        {
            Assert.AreEqual("QUThlfRt54o2I9pzZNBPEQ==", "a".ToHash());
        }

        [Test]
        public void ShouldAnonymizeNull()
        {
            Assert.Null(((string) null).ToHash());
        }

        [Test]
        public void ShouldNotAnonymizeEmptyString()
        {
            Assert.AreEqual("", "".ToHash());
        }

        [Test]
        public void ShouldNotAnonymizeUnknownNames()
        {
            var actual = Names.UnknownGeneral.ToAnonymousName();
            var expected = Names.UnknownGeneral;
            Assert.AreEqual(expected, actual);
            // equivalent...
            var actual2 = Names.UnknownType.ToAnonymousName();
            var expected2 = Names.UnknownType;
            Assert.AreEqual(expected2, actual2);
            // equivalent...
            var actual3 = Names.UnknownMethod.ToAnonymousName();
            var expected3 = Names.UnknownMethod;
            Assert.AreEqual(expected3, actual3);
        }

        [Test]
        public void ShouldAnonymizeFileNameFromDocumentName()
        {
            var original = Names.Document("CSharp C:\\File.cs");
            var expected = Names.Document("CSharp ixlmuLAuUg0yq59EtLWB7w==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeWindowCaptionIfItContainsAFileName()
        {
            var original = Names.Window("vsSomeWindowType C:\\Contains\\File.Name");
            var expected = Names.Window("vsSomeWindowType aVxPI-qHR-QO3bMv-Ker6w==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepWindowCaptionIfItDoesNotContainAFileName()
        {
            var original = Names.Window("vsToolWindow Unit Test Sessions");

            AssertAnonymizedEquals(original, original);
        }

        [Test]
        public void ShouldAnonymizeSolutionPath()
        {
            var original = Names.Solution("C:\\Solution.sln");
            var expected = Names.Solution("H_MB2iBprhCn9SyXdxnVNQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeProjectPath()
        {
            var original = Names.Project("Folder C:\\A\\B\\C");
            var expected = Names.Project("Folder IklTG_YtPBAhWOIrB65I1Q==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeProjectItemPath()
        {
            var original = Names.ProjectItem("CSharp C:\\A\\B\\Class.cs");
            var expected = Names.ProjectItem("CSharp nmTd_-pgymTyNZrw5bGrpg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeAlias()
        {
            var original = Names.Alias("global");
            var expected = Names.Alias("rW1oPYChRX9JiYuIQBWjBQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeGenericName()
        {
            var original = Names.General("some name that might or might not contain private information");
            var expected = Names.General("C5my9gXfmcktCtPzYR9MEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeAssemblyIfEnclosingProject()
        {
            var original = Names.Assembly("MyProject");
            var expected = Names.Assembly("zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepAssemblyIfFullQualified()
        {
            var original = Names.Assembly("SomeAssembly, 1.5.6.3");

            AssertAnonymizedEquals(original, original);
        }

        [TestCase("p:int"), TestCase("p:int[]")]
        public void ShouldKeepPredefinedTypes(string id)
        {
            var original = Names.Type(id);
            AssertAnonymizedEquals(original, original);
        }

        [Test]
        public void ShouldKeepTypeFromOtherAssembly()
        {
            var original = Names.Type("SomeType, MyProject, 1.2.3.4");

            AssertAnonymizedEquals(original, original);
        }

        [Test]
        public void ShouldKeepUnknownTypeName()
        {
            var original = Names.UnknownType;

            AssertAnonymizedEquals(original, original);
        }

        [Test]
        public void ShouldAnonymizeFieldNameIfDeclaringTypeIsUnknown()
        {
            var original = Names.Field("[?] [?].field");
            var expected = Names.Field("[?] [?].uH-HUtyKzOVVTdxGpUvTRg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeMethodMembersIfDeclaringTypeIsUnknown()
        {
            var original = Names.Method("[?] [?].method([?] arg)");
            var expected = Names.Method("[?] [?].S2MqM0cJGKIdPyRb46oevg==([?] cjjZM6DVkmp283JnWfyH_A==)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeTypeNameIfFromEnclosingProject()
        {
            var original = Names.Type("SomeType, MyProject");
            var expected = Names.Type("5TEfRdZBhGQY3JybERVp-w==, zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepNestedTypeMarkersWhenAnonymizingTypeName()
        {
            var original = Names.Type("Outer+Intermediate+Inner, MyProject");
            var expected =
                Names.Type(
                    "vWJW7HmayjJvbX16XC9VnQ==+471REvNW-WCCyW7mDRT4EA==+YDcvejSpfAK3U9T4L-U5Ng==, zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepNamespaceToTypeSeparatorWhenAnonymizingTypeName()
        {
            var original = Names.Type("My.Namespace.MyType, MyProject");
            var expected = Names.Type("L5-7Qmufwl5lDD-ks5-QzQ==.Q-vTVCo_g8yayGGoDdH7BA==, zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepTypeParameterShortName()
        {
            var original = Names.Type("TT -> AType, MyProject");
            var expected = Names.Type("TT -> S8jqvjvDTBSSXY7BIBFNOQ==, zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepNestedTypeParameterShortNames()
        {
            var original = Names.Type("TT -> TU -> TV -> AType, MyProject");
            var expected = Names.Type("TT -> TU -> TV -> S8jqvjvDTBSSXY7BIBFNOQ==, zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeTypeParametersIfDefinedInEnclosingProject()
        {
            var original = Names.Type("Some.Type`1[[T -> OtherType, A]], B, 1.2.3.4");
            var expected =
                Names.Type("Some.Type`1[[T -> xJGI74kh-RBFid7-a1wFlg==, ghTRAD9op9mwNWwMvX7uGg==]], B, 1.2.3.4");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepTypeParametersFromOtherAssembly()
        {
            var original =
                Names.Type(
                    "Some.Type`3[[T -> MyType, A],[U -> System.D, mscorlib, 4.0.0.0],[V -> MyOtherType, A]], B, 1.2.3.4");
            var expected =
                Names.Type(
                    "Some.Type`3[[T -> Q-vTVCo_g8yayGGoDdH7BA==, ghTRAD9op9mwNWwMvX7uGg==],[U -> System.D, mscorlib, 4.0.0.0],[V -> w20iwoM8jFvdUxBRQsKvhg==, ghTRAD9op9mwNWwMvX7uGg==]], B, 1.2.3.4");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeParameterizedTypeIfDefinedInEnclosingProject()
        {
            var original =
                Names.Type("MyTypeFromEnclosingProject`1[[T -> System.X, mscorlib, 4.0.0.0]], EnclosingProject");
            var expected =
                Names.Type(
                    "yqUUbRFTqfCBIMxMRH-qDA==`1[[TM6pgLI0nE5n0EEgAKIIFw== -> System.X, mscorlib, 4.0.0.0]], qfFVtSOtve-XEFJXWTbfXw==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeInterfaceTypeNameFromEnclosingProject()
        {
            var original = Names.Type("i:My.Interface, EnclosingProject");
            var expected = Names.Type("i:S7JFQ1Qpzr6dQZksNAcR7A==.A5028p0XwGGVTyGQASY_Xw==, qfFVtSOtve-XEFJXWTbfXw==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeEnumTypeNameFromEnclosingProject()
        {
            var original = Names.Type("e:My.Enum, EnclosingProject");
            var expected = Names.Type("e:S7JFQ1Qpzr6dQZksNAcR7A==.aFaMOQla8-d2rbQGUGlWZw==, qfFVtSOtve-XEFJXWTbfXw==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeDelegateTypeNameFromEnclosingProject()
        {
            var original = Names.Type("d:[Void, CL, 4.0.0.0] [My.Delegate, EnclosingProject].()");
            var expected =
                Names.Type(
                    "d:[Void, CL, 4.0.0.0] [S7JFQ1Qpzr6dQZksNAcR7A==.lE454GH4mIh5XOQtJhX9ng==, qfFVtSOtve-XEFJXWTbfXw==].()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeDelegateParameterName_yes()
        {
            var original = Names.Type("d:[Void, CL, 4.0.0.0] [D, E, 1.2.3.4].([P, A] p)");
            var expected =
                Names.Type(
                    "d:[Void, CL, 4.0.0.0] [D, E, 1.2.3.4].([aUaDMpYpDqsiSh5nQjiWFw==, ghTRAD9op9mwNWwMvX7uGg==] p)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeDelegateParameterName_no()
        {
            var original = Names.Type("d:[Void, CL, 4.0.0.0] [P, A].([D, E, 1.2.3.4] p)");
            var expected =
                Names.Type(
                    "d:[Void, CL, 4.0.0.0] [aUaDMpYpDqsiSh5nQjiWFw==, ghTRAD9op9mwNWwMvX7uGg==].([D, E, 1.2.3.4] xBzbwjgZ_3fD0cNcmbedKA==)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeRecursiveDelegate()
        {
            var delType = "N.D, P";
            var delTypeAnon = "{0}.{1}, {2}".FormatEx("N".ToHash(), "D".ToHash(), "P".ToHash());
            var original = Names.Type("d:[{0}] [{0}].([{0}] p)", delType);
            var expected = Names.Type("d:[{0}] [{0}].([{0}] {1})", delTypeAnon, "p".ToHash());
            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeDelegateReturnTypeFromEnclosingProject()
        {
            var original = Names.Type("d:[T, EP] [D, E, 1.2.3.4].()");
            var expected = Names.Type("d:[TM6pgLI0nE5n0EEgAKIIFw==, vW8RYxLbF7t21szDOJMe_w==] [D, E, 1.2.3.4].()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeCustomStructTypeNameFromEnclosingProject()
        {
            var original = Names.Type("s:My.Struct, EnclosingProject");
            var expected = Names.Type("s:S7JFQ1Qpzr6dQZksNAcR7A==.pPXyIgQrF3lffm_A0yqnCw==, qfFVtSOtve-XEFJXWTbfXw==");

            HashDebug("Interface", "Enum", "Struct");
            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeValueTypeOfArrayTypeIfDeclaredInEnclosingProject()
        {
            var original = Names.Type("SomeType[], EnclosingProject");
            var expected = Names.Type("5TEfRdZBhGQY3JybERVp-w==[], qfFVtSOtve-XEFJXWTbfXw==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeValueTypeOfDelegateArrayTypeIfDeckaredInEnclosingProject()
        {
            var original = Names.Type(
                "d:[VT, A] [ConsoleApplication1.Program+TestDelegate, A].()[]");
            var expected =
                Names.Type(
                    "d:[3sO2hZAKj3g4-Zk_E0A-_w==, ghTRAD9op9mwNWwMvX7uGg==] [IJXZZxKP9BYUXNF-zFV4Mg==.3hvdSyh3QO13K3vH3w7KxA==+qXAvOqA3UCqSEhbDeHkp3A==, ghTRAD9op9mwNWwMvX7uGg==].()[]");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeLocalVariableName()
        {
            var original = Names.LocalVariable("[System.X, mscorlib, 4.0.0.0] variable");
            var expected = Names.LocalVariable("[System.X, mscorlib, 4.0.0.0] ex1ycJF4ixZdevwEdEfKcQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeLocalVariableValueTypeIfDeclaredInEnclosingProject()
        {
            var original = Names.LocalVariable("[ValueType, EnclosingProject] variable");
            var expected =
                Names.LocalVariable("[K6-3xDZUlJ-Wew_p0xcfQg==, qfFVtSOtve-XEFJXWTbfXw==] ex1ycJF4ixZdevwEdEfKcQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepFieldNameIfDeclaredInOtherAssembly()
        {
            var original = Names.Field(
                "static [System.X, mscorlib, 4.0.0.0] [AClass, AnAssembly, 1.2.3.4]._field");
            var expected = Names.Field(
                "static [System.X, mscorlib, 4.0.0.0] [AClass, AnAssembly, 1.2.3.4]._field");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeFieldNameIfDeclaredInEnclosingProject()
        {
            var original = Names.Field("[System.X, mscorlib, 4.0.0.0] [Class, Project]._field");
            var expected =
                Names.Field(
                    "[System.X, mscorlib, 4.0.0.0] [C30g7wWDiaLWDoT99aNK_Q==, Mxp53D4r1Kx8kPEM01ySAA==].gcnTNGyqNJv6QToYz_Vmbg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeFieldValueTypeIfDeclaredInEnclosingProject()
        {
            var original = Names.Field("[ValueType, EnclosingProject] [SomeType, SomeAssembly, 1.2.3.4]._field");
            var expected =
                Names.Field(
                    "[K6-3xDZUlJ-Wew_p0xcfQg==, qfFVtSOtve-XEFJXWTbfXw==] [SomeType, SomeAssembly, 1.2.3.4]._field");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizePropertiesWithParameters()
        {
            var original = Names.Property("get [p:int] [p:int].P([p:int] p)");
            var expected = Names.Property("get [p:int] [p:int].P([p:int] p)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepPropertyNameIfDeclaredInOtherAssembly()
        {
            var original = Names.Property(
                "set get static [System.X, mscorlib, 4.0.0.0] [AClass, AnAssembly, 1.2.3.4].Property()");
            var expected =
                Names.Property(
                    "set get static [System.X, mscorlib, 4.0.0.0] [AClass, AnAssembly, 1.2.3.4].Property()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizePropertyNameIfDeclaredInEnclosingProject()
        {
            var original = Names.Property("get [System.X, mscorlib, 4.0.0.0] [Declarator, MyProject].Property()");
            var expected =
                Names.Property(
                    "get [System.X, mscorlib, 4.0.0.0] [UFthX8igK4OWY-bjuPcWaA==, zRLpydQJBMrk8DCiP3BwEQ==].3_9-BEZu3bkEMnTfk5eHKw==()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizePropertyValueTypeIfDeclaredInEnclosingProject()
        {
            var original = Names.Property("set [PropType, AProject] [AType, AnAssembly, 6.5.4.3].Property()");
            var expected =
                Names.Property(
                    "set [Mh2DRn_FRby9df2VWWFg4Q==, CD0OwIZmS7FL5zL5GiXZbg==] [AType, AnAssembly, 6.5.4.3].Property()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepEventNameIfDeclaredInOtherAssembly()
        {
            var original = Names.Event("static [ChangeEventHandler, Assembly, 6.3.5.2] [C, Foo, 9.1.2.3].Event");
            var expected = Names.Event("static [ChangeEventHandler, Assembly, 6.3.5.2] [C, Foo, 9.1.2.3].Event");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeEventNameIfDeclaredInEnclosingProject()
        {
            var original = Names.Event("[ChangeEventHandler, Assembly, 6.3.5.2] [A, Foo].Event");
            var expected =
                Names.Event(
                    "[ChangeEventHandler, Assembly, 6.3.5.2] [ghTRAD9op9mwNWwMvX7uGg==, sl_wrZDQnTlQkOiin_TGPA==].Ryz5fpCQs0Nwm_x0Vy4cQg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeEventHandlerTypeIfDeclaredInEnclosingProject()
        {
            var original = Names.Event("[Handler, Project] [AType, AnAssembly, 6.5.4.3].Event");
            var expected =
                Names.Event("[ooP_qY1chg4oTJoBIeq1_A==, Mxp53D4r1Kx8kPEM01ySAA==] [AType, AnAssembly, 6.5.4.3].Event");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeNamespace()
        {
            var original = Names.Namespace("Some.Arbitrary.Namespace");
            var expected = Names.Namespace("PU4V3sU7dhVQzcD16BGtuw==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeOutParameterName()
        {
            var original = Names.Parameter("out [ParamType, A, 1.2.3.4] parameter");
            var expected = Names.Parameter("out [ParamType, A, 1.2.3.4] jaWpYMumKzk5dZafVWTD1A==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeRefParameterName()
        {
            var original = Names.Parameter("ref [p:int] name");
            var expected = Names.Parameter("ref [p:int] mT62IUL9_OAA7vtSkeTMzg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeParamsParameterName()
        {
            var original = Names.Parameter("params [System.X[], mscorlib, 4.0.0.0] name");
            var expected = Names.Parameter("params [System.X[], mscorlib, 4.0.0.0] mT62IUL9_OAA7vtSkeTMzg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeOptParameterName()
        {
            var original = Names.Parameter("opt [System.X, mscorlib, 4.0.0.0] name");
            var expected = Names.Parameter("opt [System.X, mscorlib, 4.0.0.0] mT62IUL9_OAA7vtSkeTMzg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeParameterValueTypeIfDeclaredInEnclosingProject()
        {
            var original = Names.Parameter("[Type, Project] name");
            var expected =
                Names.Parameter("[aSO4V69Y4hQtcEQCnqsGww==, Mxp53D4r1Kx8kPEM01ySAA==] mT62IUL9_OAA7vtSkeTMzg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeMethodNameIfDeclaringTypeIsFromEnclosingProject()
        {
            var original = Names.Method("[ReturnType, A, 1.2.3.4] [DeclaringType, EnclosingProject].M()");
            var expected =
                Names.Method(
                    "[ReturnType, A, 1.2.3.4] [HTr1vZnVhe-8SY78vI2ffQ==, qfFVtSOtve-XEFJXWTbfXw==].lNSAgClcjc9lDeUkXybdNQ==()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeMethodReturnTypeIfDeclaringTypeIsFromEnclosingProject()
        {
            var original = Names.Method("[ReturnType, EP] [DT, A, 1.2.3.4].M()");
            var expected = Names.Method("[a6Ix9ar6tahkEo1TOfBLwg==, vW8RYxLbF7t21szDOJMe_w==] [DT, A, 1.2.3.4].M()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeMethodTypeParametersIfDeclaredInEnclosingProject()
        {
            var original = Names.Method(
                "[RT, A, 1.2.3.4] [DT, A, 1.2.3.4].M`2[[T -> Foo, EP],[E -> Bar, A, 1.2.3.4]]()");
            var expected =
                Names.Method(
                    "[RT, A, 1.2.3.4] [DT, A, 1.2.3.4].M`2[[T -> sl_wrZDQnTlQkOiin_TGPA==, vW8RYxLbF7t21szDOJMe_w==],[E -> Bar, A, 1.2.3.4]]()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeLambdaParametersIfDeclaredInEclosingProject()
        {
            var original = Names.Lambda("[A, B, 1.2.3.4] ([T, EP] p)");
            var expected =
                Names.Lambda(
                    "[A, B, 1.2.3.4] ([TM6pgLI0nE5n0EEgAKIIFw==, vW8RYxLbF7t21szDOJMe_w==] xBzbwjgZ_3fD0cNcmbedKA==)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeLambdaReturnTypeIfDeclaredInEclosingProject()
        {
            var original = Names.Lambda("[T, EP] ()");
            var expected = Names.Lambda("[TM6pgLI0nE5n0EEgAKIIFw==, vW8RYxLbF7t21szDOJMe_w==] ()");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldNotAnonymizeMethodsOrTheirParametersFromOtherAssemblies()
        {
            var original =
                Names.Method("[RT, A, 1.2.3.4] [DT, A, 1.2.3.4].M([System.String, mscorlib, 4.0.0.0] p)");

            AssertAnonymizedEquals(original, original);
        }

        [Test]
        public void AnonymizedMethodsHaveParametersWithHashedNamesButUnhashedTypes()
        {
            var original = Names.Method("[T, A, 1.2.3.4] [T,P].M([T, A, 1.2.3.4] p)");
            var expected =
                Names.Method(
                    "[T, A, 1.2.3.4] [TM6pgLI0nE5n0EEgAKIIFw==, aUaDMpYpDqsiSh5nQjiWFw==].lNSAgClcjc9lDeUkXybdNQ==([T, A, 1.2.3.4] xBzbwjgZ_3fD0cNcmbedKA==)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldOnlyAnonymizeParameterTypeInMethodFromOtherAssembly()
        {
            var original = Names.Method("[T, A, 1.2.3.4] [T, A, 1.2.3.4].M([T,P] p)");
            var expected =
                Names.Method(
                    "[T, A, 1.2.3.4] [T, A, 1.2.3.4].M([TM6pgLI0nE5n0EEgAKIIFw==, aUaDMpYpDqsiSh5nQjiWFw==] p)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldNotAnonymizeConstructorName()
        {
            var original = Names.Method("[T, A, 1.2.3.4] [DT, P]..ctor([T,P] p, [T,A,4.0.0.0] p2)");
            var expected =
                Names.Method(
                    "[T, A, 1.2.3.4] [UP5Ipka5g2hTcMU6LNvz2A==, aUaDMpYpDqsiSh5nQjiWFw==]..ctor([TM6pgLI0nE5n0EEgAKIIFw==, aUaDMpYpDqsiSh5nQjiWFw==] xBzbwjgZ_3fD0cNcmbedKA==, [T, A,4.0.0.0] UIuXc44R1FaeNKJ8ldQB7A==)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldNotAnonymizeStaticConstructorName()
        {
            var original = Names.Method("[T, A, 1.2.3.4] [DT, P]..cctor([T,P] p, [T,A,4.0.0.0] p2)");
            var expected =
                Names.Method(
                    "[T, A, 1.2.3.4] [UP5Ipka5g2hTcMU6LNvz2A==, aUaDMpYpDqsiSh5nQjiWFw==]..cctor([TM6pgLI0nE5n0EEgAKIIFw==, aUaDMpYpDqsiSh5nQjiWFw==] xBzbwjgZ_3fD0cNcmbedKA==, [T, A,4.0.0.0] UIuXc44R1FaeNKJ8ldQB7A==)");

            AssertAnonymizedEquals(original, expected);
        }

        #region anonymization of generic type parameters

        [Test]
        public void ShouldAnonymizeGenericTypeParameter_UnboundGenericInEnclosingProject()
        {
            var original = Names.Method("[T, A, 1.2.3.4] [C`1[[G2]], P].M([G2] p)");
            var expected =
                Names.Method(
                    "[T, A, 1.2.3.4] [3Rx860ySZTppa3kHpN1N8Q==`1[[HAqGEOJc_-qPti2JYHwR3Q==]], aUaDMpYpDqsiSh5nQjiWFw==].lNSAgClcjc9lDeUkXybdNQ==([HAqGEOJc_-qPti2JYHwR3Q==] xBzbwjgZ_3fD0cNcmbedKA==)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeGenericTypeParameter_UnboundGenericAssignedToPlaceholder()
        {
            var original = Names.Method("[T, A, 1.2.3.4] [A`1[[G1 -> G2]], A, 0.0.0.0].M([G1] p)");
            var expected =
                Names.Method("[T, A, 1.2.3.4] [A`1[[G1 -> HAqGEOJc_-qPti2JYHwR3Q==]], A, 0.0.0.0].M([G1] p)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeGenericTypeParameter_BoundGenericInEnclosingProject()
        {
            var original = Names.Method("[T, A, 1.2.3.4] [C`1[[G2 -> T, A, 0.0.0.0]], P].M([G2] p)");
            var expected =
                Names.Method(
                    "[T, A, 1.2.3.4] [3Rx860ySZTppa3kHpN1N8Q==`1[[HAqGEOJc_-qPti2JYHwR3Q== -> T, A, 0.0.0.0]], aUaDMpYpDqsiSh5nQjiWFw==].lNSAgClcjc9lDeUkXybdNQ==([HAqGEOJc_-qPti2JYHwR3Q==] xBzbwjgZ_3fD0cNcmbedKA==)");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeGenericTypeParameter_BoundGenericInAssembly()
        {
            var original = Names.Method("[T, A, 1.2.3.4] [A`1[[G1 -> T, A, 0.0.0.0]], A, 0.0.0.0].M([G1] p)");
            var expected = Names.Method("[T, A, 1.2.3.4] [A`1[[G1 -> T, A, 0.0.0.0]], A, 0.0.0.0].M([G1] p)");

            AssertAnonymizedEquals(original, expected);
        }

        #endregion

        private static void AssertAnonymizedEquals([NotNull] IName original, [NotNull] IName expected)
        {
            var actual = original.ToAnonymousName();
            Assert.AreEqual(expected, actual);
        }

        private static void HashDebug(params string[] strs)
        {
            foreach (var str in strs)
            {
                Console.WriteLine("{0} ==> {1}", str, str.ToHash());
            }
        }
    }
}