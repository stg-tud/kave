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

using KaVE.JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize
{
    [TestFixture]
    internal class AnonymousNameUtilsTest
    {
        [Test]
        public void ShouldAnonymizeFileNameFromDocumentName()
        {
            var original = DocumentName.Get("CSharp C:\\File.cs");
            var expected = DocumentName.Get("CSharp ixlmuLAuUg0yq59EtLWB7w==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeWindowCaptionIfItContainsAFileName()
        {
            var original = WindowName.Get("vsSomeWindowType C:\\Contains\\File.Name");
            var expected = WindowName.Get("vsSomeWindowType aVxPI+qHR+QO3bMv+Ker6w==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepWindowCaptionIfItDoesNotContainAFileName()
        {
            var original = WindowName.Get("vsToolWindow Unit Test Sessions");

            AssertAnonymizedEquals(original, original);
        }

        [Test]
        public void ShouldAnonymizeSolutionPath()
        {
            var original = SolutionName.Get("C:\\Solution.sln");
            var expected = SolutionName.Get("H/MB2iBprhCn9SyXdxnVNQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeProjectPath()
        {
            var original = ProjectName.Get("Folder C:\\A\\B\\C");
            var expected = ProjectName.Get("Folder IklTG/YtPBAhWOIrB65I1Q==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeProjectItemPath()
        {
            var original = ProjectItemName.Get("CSharp C:\\A\\B\\Class.cs");
            var expected = ProjectItemName.Get("CSharp nmTd/+pgymTyNZrw5bGrpg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeAlias()
        {
            var original = AliasName.Get("global");
            var expected = AliasName.Get("rW1oPYChRX9JiYuIQBWjBQ==");
            
            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeGenericName()
        {
            var original = Name.Get("some name that might or might not contain private information");
            var expected = Name.Get("C5my9gXfmcktCtPzYR9MEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeAssemblyIfEnclosingProject()
        {
            var original = AssemblyName.Get("MyProject");
            var expected = AssemblyName.Get("zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepAssemblyIfFullQualified()
        {
            var original = AssemblyName.Get("SomeAssembly, 1.5.6.3");

            AssertAnonymizedEquals(original, original);
        }

        [Test]
        public void ShouldKeepTypeFromOtherAssembly()
        {
            var original = TypeName.Get("SomeType, MyProject, 1.2.3.4");

            AssertAnonymizedEquals(original, original);
        }

        [Test]
        public void ShouldAnonymizeTypeNameIfFromEnclosingProject()
        {
            var original = TypeName.Get("SomeType, MyProject");
            var expected = TypeName.Get("5TEfRdZBhGQY3JybERVp+w==, zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepTypeParameterShortName()
        {
            var original = TypeName.Get("TT -> AType, MyProject");
            var expected = TypeName.Get("TT -> S8jqvjvDTBSSXY7BIBFNOQ==, zRLpydQJBMrk8DCiP3BwEQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeTypeParametersIfDefinedInEnclosingProject()
        {
            var original = TypeName.Get("Some.Type`1[[T -> OtherType, A]], B, 1.2.3.4");
            var expected = TypeName.Get("Some.Type`1[[T -> xJGI74kh+RBFid7+a1wFlg==, ghTRAD9op9mwNWwMvX7uGg==]], B, 1.2.3.4");

            AssertAnonymizedEquals(original, expected);
        }

        // TODO @Sven: Write test for parameterized types from enclosing project
        // TODO @Sven: Write tests for i: name subtypes

        [Test]
        public void ShouldAnonymizeLocalVariableName()
        {
            var original = LocalVariableName.Get("[System.Int32, mscorlib, 4.0.0.0] variable");
            var expected = LocalVariableName.Get("[System.Int32, mscorlib, 4.0.0.0] ex1ycJF4ixZdevwEdEfKcQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeLocalVariableValueTypeIfDeclaredInEnclosingProject()
        {
            var original = LocalVariableName.Get("[ValueType, EnclosingProject] variable");
            var expected = LocalVariableName.Get("[K6+3xDZUlJ+Wew/p0xcfQg==, qfFVtSOtve+XEFJXWTbfXw==] ex1ycJF4ixZdevwEdEfKcQ==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepFieldNameIfDeclaredInOtherAssembly()
        {
            var original = FieldName.Get("static [System.Int32, mscorlib, 4.0.0.0] [AClass, AnAssembly, 1.2.3.4]._field");
            var expected = FieldName.Get("static [System.Int32, mscorlib, 4.0.0.0] [AClass, AnAssembly, 1.2.3.4]._field");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeFieldNameIfDeclaredInEnclosingProject()
        {
            var original = FieldName.Get("[System.Int32, mscorlib, 4.0.0.0] [Class, Project]._field");
            var expected = FieldName.Get("[System.Int32, mscorlib, 4.0.0.0] [C30g7wWDiaLWDoT99aNK/Q==, Mxp53D4r1Kx8kPEM01ySAA==].gcnTNGyqNJv6QToYz/Vmbg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeFieldValueTypeIfDeclaredInEnclosingProject()
        {
            var original = FieldName.Get("[ValueType, EnclosingProject] [SomeType, SomeAssembly, 1.2.3.4]._field");
            var expected = FieldName.Get("[K6+3xDZUlJ+Wew/p0xcfQg==, qfFVtSOtve+XEFJXWTbfXw==] [SomeType, SomeAssembly, 1.2.3.4]._field");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepPropertyNameIfDeclaredInOtherAssembly()
        {
            var original = PropertyName.Get("set get static [System.Int32, mscorlib, 4.0.0.0] [AClass, AnAssembly, 1.2.3.4].Property");
            var expected = PropertyName.Get("set get static [System.Int32, mscorlib, 4.0.0.0] [AClass, AnAssembly, 1.2.3.4].Property");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizePropertyNameIfDeclaredInEnclosingProject()
        {
            var original = PropertyName.Get("get [System.Int32, mscorlib, 4.0.0.0] [Declarator, MyProject].Property");
            var expected = PropertyName.Get("get [System.Int32, mscorlib, 4.0.0.0] [UFthX8igK4OWY+bjuPcWaA==, zRLpydQJBMrk8DCiP3BwEQ==].3/9+BEZu3bkEMnTfk5eHKw==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizePropertyValueTypeIfDeclaredInEnclosingProject()
        {
            var original = PropertyName.Get("set [PropType, AProject] [AType, AnAssembly, 6.5.4.3].Property");
            var expected = PropertyName.Get("set [Mh2DRn/FRby9df2VWWFg4Q==, CD0OwIZmS7FL5zL5GiXZbg==] [AType, AnAssembly, 6.5.4.3].Property");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldKeepEventNameIfDeclaredInOtherAssembly()
        {
            var original = EventName.Get("static [ChangeEventHandler, Assembly, 6.3.5.2] [C, Foo, 9.1.2.3].Event");
            var expected = EventName.Get("static [ChangeEventHandler, Assembly, 6.3.5.2] [C, Foo, 9.1.2.3].Event");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeEventNameIfDeclaredInEnclosingProject()
        {
            var original = EventName.Get("[ChangeEventHandler, Assembly, 6.3.5.2] [A, Foo].Event");
            var expected = EventName.Get("[ChangeEventHandler, Assembly, 6.3.5.2] [ghTRAD9op9mwNWwMvX7uGg==, sl/wrZDQnTlQkOiin/TGPA==].Ryz5fpCQs0Nwm/x0Vy4cQg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeEventHandlerTypeIfDeclaredInEnclosingProject()
        {
            var original = EventName.Get("[Handler, Project] [AType, AnAssembly, 6.5.4.3].Event");
            var expected = EventName.Get("[ooP/qY1chg4oTJoBIeq1/A==, Mxp53D4r1Kx8kPEM01ySAA==] [AType, AnAssembly, 6.5.4.3].Event");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeNamespace()
        {
            var original = NamespaceName.Get("Some.Arbitrary.Namespace");
            var expected = NamespaceName.Get("PU4V3sU7dhVQzcD16BGtuw==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeOutParameterName()
        {
            var original = ParameterName.Get("out [ParamType, A, 1.2.3.4] parameter");
            var expected = ParameterName.Get("out [ParamType, A, 1.2.3.4] jaWpYMumKzk5dZafVWTD1A==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeRefParameterName()
        {
            var original = ParameterName.Get("ref [System.Int32, mscorlib, 4.0.0.0] name");
            var expected = ParameterName.Get("ref [System.Int32, mscorlib, 4.0.0.0] mT62IUL9/OAA7vtSkeTMzg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeParamsParameterName()
        {
            var original = ParameterName.Get("params [System.Int32, mscorlib, 4.0.0.0] name");
            var expected = ParameterName.Get("params [System.Int32, mscorlib, 4.0.0.0] mT62IUL9/OAA7vtSkeTMzg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeOptParameterName()
        {
            var original = ParameterName.Get("opt [System.Int32, mscorlib, 4.0.0.0] name");
            var expected = ParameterName.Get("opt [System.Int32, mscorlib, 4.0.0.0] mT62IUL9/OAA7vtSkeTMzg==");

            AssertAnonymizedEquals(original, expected);
        }

        [Test]
        public void ShouldAnonymizeParameterValueTypeIfDeclaredInEnclosingProject()
        {
            var original = ParameterName.Get("[Type, Project] name");
            var expected = ParameterName.Get("[aSO4V69Y4hQtcEQCnqsGww==, Mxp53D4r1Kx8kPEM01ySAA==] mT62IUL9/OAA7vtSkeTMzg==");

            AssertAnonymizedEquals(original, expected);
        }

        // TODO @Sven: write tests for methods

        private static void AssertAnonymizedEquals([NotNull] IName original, [NotNull] IName expected)
        {
            var actual = original.ToAnonymousName();
            Assert.AreEqual(expected, actual);
        }
    }
}