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
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.IDEComponents;
using KaVE.Commons.Model.Naming.Impl.v0.Others;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Naming;
using NUnit.Framework;
using NamesV1 = KaVE.Commons.Model.Naming.Impl.v1.Names;

namespace KaVE.Commons.Tests.Utils.Naming
{
    internal class NameSerializationTest
    {
        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShouldFailForUnknownPrefixes()
        {
            "x:...".Deserialize<IName>();
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void ShouldFailForUnknownTypes()
        {
            new TestName().Serialize();
        }

        [Test]
        public void ShouldApplyV0NameFixes()
        {
            var actual = "0T:d:T,P".Deserialize<IDelegateTypeName>();
            var expected = new DelegateTypeName("d:[?] [T,P].()");
            Assert.AreEqual(expected, actual);
        }

        [TestCase("T,P", "CSharp.TypeName", "0T", typeof(TypeName)),
         TestCase("T[],P", "CSharp.TypeName", "0T", typeof(ArrayTypeName)),
         TestCase("p:int", "CSharp.PredefinedTypeName", "0T", typeof(PredefinedTypeName)),
         TestCase("p:int[]", "CSharp.PredefinedTypeName", "0T", typeof(PredefinedTypeName)),
         TestCase("T", "CSharp.TypeName", "0T", typeof(TypeParameterName)),
         TestCase("T", "CSharp.TypeParameterName", "0T", typeof(TypeParameterName)),
         TestCase("T -> T,P", "CSharp.TypeName", "0T", typeof(TypeParameterName)),
         TestCase("e:n.E,P", "CSharp.EnumTypeName", "0T", typeof(TypeName)),
         TestCase("i:n.I,P", "CSharp.InterfaceTypeName", "0T", typeof(TypeName)),
         TestCase("s:n.S,P", "CSharp.StructTypeName", "0T", typeof(TypeName)),
         TestCase("d:[?] [T,P].()", "CSharp.TypeName", "0T", typeof(DelegateTypeName)),
         TestCase("d:[?] [T,P].()", "CSharp.DelegateTypeName", "0T", typeof(DelegateTypeName))]
        public void ShouldDeserializeV0Types(string id, string oldPrefix, string newPrefix, Type expectedType)
        {
            foreach (var prefix in new[] {oldPrefix, newPrefix})
            {
                var input = "{0}:{1}".FormatEx(prefix, id);

                var objExpected = TypeUtils.CreateTypeName(id);
                var objActual = input.Deserialize<IName>();
                Assert.AreEqual(objExpected, objActual);
                Assert.True(expectedType.IsInstanceOfType(objActual));

                var outputActual = objActual.Serialize();
                var outputExpected = "{0}:{1}".FormatEx(newPrefix, id);
                Assert.AreEqual(outputExpected, outputActual);
            }
        }

        [Test]
        public void ShouldHandleOldUnknownType()
        {
            var objExpected = new TypeName();
            var objActual = "CSharp.UnknownTypeName:?".Deserialize<IName>();

            Assert.AreEqual(objExpected, objActual);
            Assert.True(objActual is TypeName);

            var outputActual = objActual.Serialize();
            var outputExpected = "0T:?";
            Assert.AreEqual(outputExpected, outputActual);
        }

        [TestCase("xyz", "CSharp.Name", "0General", typeof(GeneralName)),
        // code elements
         TestCase("A -> ?", "CSharp.AliasName", "0Alias", typeof(AliasName)),
         TestCase("[VT,P] [DT,P]._e", "CSharp.EventName", "0E", typeof(EventName)),
         TestCase("[VT,P] [DT,P]._f", "CSharp.FieldName", "0F", typeof(FieldName)),
         TestCase("[RT,P] ()", "CSharp.LambdaName", "0L", typeof(LambdaName)),
         TestCase("[T,P] v", "CSharp.LocalVariableName", "0LocalVar", typeof(LocalVariableName)),
         TestCase("[RT,P] [DT,P].M()", "CSharp.MethodName", "0M", typeof(MethodName)),
         TestCase("[PT,P] p", "CSharp.ParameterName", "0Param", typeof(ParameterName)),
         TestCase("get [VT,P] [DT,P].P()", "CSharp.PropertyName", "0P", typeof(PropertyName)),
        // ide components
         TestCase("a|b|c", "VisualStudio.CommandBarControlName", "0Ctrl", typeof(CommandBarControlName)),
         TestCase("a:1:abc", "VisualStudio.CommandName", "0Cmd", typeof(CommandName)),
         TestCase("CSharp C:\\File.cs", "VisualStudio.DocumentName", "0Doc", typeof(DocumentName)),
         TestCase("File C:\\Project\\File.txt", "VisualStudio.ProjectItemName", "0Itm", typeof(ProjectItemName)),
         TestCase("ProjectType C:\\Project.csproj", "VisualStudio.ProjectName", "0Prj", typeof(ProjectName)),
         TestCase("C:\\File\\To\\S.sln", "VisualStudio.SolutionName", "0Sln", typeof(SolutionName)),
         TestCase("someType someCaption", "VisualStudio.WindowName", "0Win", typeof(WindowName)),
        // other
         TestCase("someType:someCaption", "ReSharper.LiveTemplateName", "0RSTpl", typeof(ReSharperLiveTemplateName)),
        // types/organization
         TestCase("A, 1.2.3.4", "CSharp.AssemblyName", "0A", typeof(AssemblyName)),
         TestCase("1.2.3.4", "CSharp.AssemblyVersion", "0V", typeof(AssemblyVersion)),
         TestCase("a.b.c", "CSharp.NamespaceName", "0N", typeof(NamespaceName)),
        // types
         TestCase("T,P", "CSharp.TypeName", "0T", typeof(TypeName))]
        public void ShouldDeserializeV0Names(string id, string oldPrefix, string newPrefix, Type expectedType)
        {
            foreach (var prefix in new[] {oldPrefix, newPrefix})
            {
                var input = "{0}:{1}".FormatEx(prefix, id);
                var obj = input.Deserialize<IName>();

                Assert.True(expectedType.IsInstanceOfType(obj));

                var actual = obj.Serialize();
                var expected = "{0}:{1}".FormatEx(newPrefix, id);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestCase("0P:[?] [?].P()", "set get [?] [?].P()"), // serialized name fix
         TestCase("0P:set [?] [?].P", "set [?] [?].P()") // id fix
        ]
        public void ShouldApplyBothSerializedAndIdFixes(string id, string corrected)
        {
            var n = id.Deserialize<IName>();
            Assert.AreEqual(corrected, n.Identifier);
        }

        [Ignore, TestCase("1T", "T,P", typeof(Commons.Model.Naming.Impl.v1.TypeName)),
         TestCase("1Ta", "arr(1):T,P", typeof(Commons.Model.Naming.Impl.v1.ArrayTypeName))]
        public void ShouldDeserializeV1Types(string prefix, string id, Type expectedType)
        {
            var input = "{0}:{1}".FormatEx(prefix, id);
            var expected = NamesV1.Type(id);
            var actual = input.Deserialize<IName>();
            Assert.AreEqual(expected, actual);
            Assert.True(expectedType.IsInstanceOfType(actual));
            Assert.AreEqual(input, actual.Serialize());
        }

        private class TestName : IName
        {
            public string Identifier
            {
                get { return ""; }
            }

            public bool IsUnknown
            {
                get { return false; }
            }

            public bool IsHashed
            {
                get { return false; }
            }
        }
    }
}