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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.IDEComponents;
using KaVE.Commons.Model.Naming.Impl.v0.Others;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;

namespace KaVE.Commons.Utils.Naming
{
    internal class NameSerializerV0 : NameSerializerBase
    {
        protected override void RegisterTypes()
        {
            Register(typeof(GeneralName), General, "0General", "CSharp.Name");

            // code elements
            Register(typeof(AliasName), Alias, "0Alias", "CSharp.AliasName");
            Register(typeof(EventName), Event, "0E", "CSharp.EventName");
            Register(typeof(FieldName), Field, "0F", "CSharp.FieldName");
            Register(typeof(LambdaName), Lambda, "0L", "CSharp.LambdaName");
            Register(typeof(LocalVariableName), LocalVar, "0LocalVar", "CSharp.LocalVariableName");
            Register(typeof(MethodName), Method, "0M", "CSharp.MethodName");
            Register(typeof(ParameterName), Parameter, "0Param", "CSharp.ParameterName");
            Register(typeof(PropertyName), Property, "0P", "CSharp.PropertyName");

            // ide components
            Register(typeof(CommandBarControlName), CommandBarControl, "0Ctrl", "VisualStudio.CommandBarControlName");
            Register(typeof(CommandName), Command, "0Cmd", "VisualStudio.CommandName");
            Register(typeof(DocumentName), Document, "0Doc", "VisualStudio.DocumentName");
            Register(typeof(ProjectItemName), ProjectItem, "0Itm", "VisualStudio.ProjectItemName");
            Register(typeof(ProjectName), Project, "0Prj", "VisualStudio.ProjectName");
            Register(typeof(SolutionName), Solution, "0Sln", "VisualStudio.SolutionName");
            Register(typeof(WindowName), Window, "0Win", "VisualStudio.WindowName");

            // others
            Register(typeof(ReSharperLiveTemplateName), ReSharperLiveTemplate, "0RSTpl", "ReSharper.LiveTemplateName");

            // types/organization
            Register(typeof(AssemblyName), Assembly, "0A", "CSharp.AssemblyName");
            Register(typeof(AssemblyVersion), AssemblyVersion, "0V", "CSharp.AssemblyVersion");
            Register(typeof(NamespaceName), Namespace, "0N", "CSharp.NamespaceName");

            //types
            RegisterTypeMapping(
                typeof(TypeName),
                typeof(ArrayTypeName),
                typeof(DelegateTypeName),
                typeof(TypeParameterName),
                typeof(PredefinedTypeName));
            Register(
                typeof(TypeName),
                Type,
                "0T",
                "CSharp.TypeName",
                "CSharp.UnknownTypeName",
                "CSharp.ArrayTypeName",
                "CSharp.DelegateTypeName",
                "CSharp.EnumTypeName",
                "CSharp.InterfaceTypeName",
                "CSharp.StructTypeName",
                "CSharp.PredefinedTypeName",
                "CSharp.TypeParameterName");
        }

        protected override string FixLegacyIdentifiers(string prefix, string id)
        {
            return id.FixIdentifiers(prefix);
        }

        private static IName General(string id)
        {
            return new GeneralName(id);
        }

        #region code elements

        private static IName Alias(string id)
        {
            return new AliasName(id);
        }

        private static IName Event(string id)
        {
            return new EventName(id);
        }

        private static IName Field(string id)
        {
            return new FieldName(id);
        }

        private static IName Lambda(string id)
        {
            return new LambdaName(id);
        }

        private static IName LocalVar(string id)
        {
            return new LocalVariableName(id);
        }

        private static IName Method(string id)
        {
            return new MethodName(id);
        }

        private static IName Parameter(string id)
        {
            return new ParameterName(id);
        }

        private static IName Property(string id)
        {
            return new PropertyName(id);
        }

        #endregion

        #region ide components & other

        private static IName CommandBarControl(string id)
        {
            return new CommandBarControlName(id);
        }

        private static IName Command(string id)
        {
            return new CommandName(id);
        }

        private static IName Document(string id)
        {
            return new DocumentName(id);
        }

        private static IName ProjectItem(string id)
        {
            return new ProjectItemName(id);
        }

        private static IName Project(string id)
        {
            return new ProjectName(id);
        }

        private static IName Solution(string id)
        {
            return new SolutionName(id);
        }

        private static IName Window(string id)
        {
            return new WindowName(id);
        }

        private static IName ReSharperLiveTemplate(string id)
        {
            return new ReSharperLiveTemplateName(id);
        }

        #endregion

        #region types

        private static IName Assembly(string id)
        {
            return new AssemblyName(id);
        }

        private static IName AssemblyVersion(string id)
        {
            return new AssemblyVersion(id);
        }

        private static IName Namespace(string id)
        {
            return new NamespaceName(id);
        }

        private static IName Type(string id)
        {
            return TypeUtils.CreateTypeName(id);
        }

        #endregion
    }
}