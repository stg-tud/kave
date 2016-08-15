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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming
{
    internal class NamesTest
    {
        [Test]
        public void CorrectInitializationAndNonNull()
        {
            AssertInit(Names.General("x"), typeof(GeneralName));

            AssertInit(Names.Alias("x"), typeof(AliasName));
            AssertInit(Names.Event("x"), typeof(EventName));
            AssertInit(Names.Field("x"), typeof(FieldName));
            AssertInit(Names.Lambda("x"), typeof(LambdaName));
            AssertInit(Names.LocalVariable("x"), typeof(LocalVariableName));
            AssertInit(Names.Method("[?] [?].M()"), typeof(MethodName));
            AssertInit(Names.Parameter("[?] p"), typeof(ParameterName));
            AssertInit(Names.Property("get [?] [?].P()"), typeof(PropertyName));

            AssertInit(Names.CommandBarControl("x"), typeof(CommandBarControlName));
            AssertInit(Names.Command("x"), typeof(CommandName));
            AssertInit(Names.Document("x y"), typeof(DocumentName));
            AssertInit(Names.ProjectItem("x y"), typeof(ProjectItemName));
            AssertInit(Names.Project("x y"), typeof(ProjectName));
            AssertInit(Names.Solution("x"), typeof(SolutionName));
            AssertInit(Names.Window("x y"), typeof(WindowName));

            AssertInit(Names.ReSharperLiveTemplate("x:y"), typeof(ReSharperLiveTemplateName));

            AssertInit(Names.Assembly("x"), typeof(AssemblyName));
            AssertInit(Names.AssemblyVersion("1.2.3.4"), typeof(AssemblyVersion));
            AssertInit(Names.Namespace("x"), typeof(NamespaceName));

            AssertInit(Names.Type("T,P"), typeof(TypeName));
            AssertInit(Names.Type("T"), typeof(TypeParameterName));
            AssertInit(Names.Type("T[],P"), typeof(ArrayTypeName));
            AssertInit(Names.Type("d:[?] [?].()"), typeof(DelegateTypeName));
            AssertInit(Names.Type("p:int"), typeof(PredefinedTypeName));
        }

        [Test]
        public void Unknowns()
        {
            AssertUnknown(Names.UnknownGeneral, new GeneralName());

            AssertUnknown(Names.UnknownAlias, new AliasName());
            AssertUnknown(Names.UnknownEvent, new EventName());
            AssertUnknown(Names.UnknownField, new FieldName());
            AssertUnknown(Names.UnknownLambda, new LambdaName());
            AssertUnknown(Names.UnknownLocalVariable, new LocalVariableName());
            AssertUnknown(Names.UnknownMethod, new MethodName());
            AssertUnknown(Names.UnknownParameter, new ParameterName());
            AssertUnknown(Names.UnknownProperty, new PropertyName());

            AssertUnknown(Names.UnknownCommandBarControl, new CommandBarControlName());
            AssertUnknown(Names.UnknownCommand, new CommandName());
            AssertUnknown(Names.UnknownDocument, new DocumentName());
            AssertUnknown(Names.UnknownProjectItem, new ProjectItemName());
            AssertUnknown(Names.UnknownProject, new ProjectName());
            AssertUnknown(Names.UnknownSolution, new SolutionName());
            AssertUnknown(Names.UnknownWindow, new WindowName());

            AssertUnknown(Names.UnknownReSharperLiveTemplate, new ReSharperLiveTemplateName());

            AssertUnknown(Names.UnknownAssembly, new AssemblyName());
            AssertUnknown(Names.UnknownAssemblyVersion, new AssemblyVersion());
            AssertUnknown(Names.UnknownNamespace, new NamespaceName());

            AssertUnknown(Names.UnknownType, new TypeName());
            AssertUnknown(Names.UnknownDelegateType, new DelegateTypeName());
        }

        [Test]
        public void CanDeriveArrays()
        {
            var actual = Names.ArrayType(2, new TypeName("T, P"));
            var expected = new ArrayTypeName("T[,], P");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanCreateTypeParameter()
        {
            var actual = Names.TypeParameter("T", "U, P");
            var expected = new TypeParameterName("T -> U, P");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanCreateTypeParameter_unbound()
        {
            var actual = Names.TypeParameter("T");
            var expected = new TypeParameterName("T");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DoesNotBreakForRegularStringsThatDoNotNeedToBeReplaced_General()
        {
            var actual = Names.General("CombinedLookupItem:public override ToString() { ... }");
            var expected = new GeneralName("CombinedLookupItem:public override ToString() { ... }");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DoesNotBreakForRegularStringsThatDoNotNeedToBeReplaced_Command()
        {
            var actual = Names.Command("{E272D1B...}:42:SomeId");
            var expected = new CommandName("{E272D1B...}:42:SomeId");
            Assert.AreEqual(expected, actual);
        }

        private static void AssertUnknown(IName actual, IName expected)
        {
            Assert.IsTrue(actual.IsUnknown);
            Assert.AreEqual(expected, actual);
        }

        private static void AssertInit(IName name, Type expectedType)
        {
            Assert.NotNull(name);
            Assert.True(expectedType.IsInstanceOfType(name));
        }
    }
}