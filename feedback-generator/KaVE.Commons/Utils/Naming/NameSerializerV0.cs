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
            Register(typeof(CommandBarControlName), CommandBarControl, "0Ctrl", "CSharp.CommandBarControlName");
            Register(typeof(CommandName), Command, "0Cmd", "CSharp.CommandName");
            Register(typeof(DocumentName), Document, "0Doc", "CSharp.DocumentName");
            Register(typeof(ProjectItemName), ProjectItem, "0Itm", "CSharp.ProjectItemName");
            Register(typeof(ProjectName), Project, "0Prj", "CSharp.ProjectName");
            Register(typeof(SolutionName), Solution, "0Sln", "CSharp.SolutionName");
            Register(typeof(WindowName), Window, "0Win", "CSharp.WindowName");

            // others
            Register(
                typeof(ReSharperLiveTemplateName),
                ReSharperLiveTemplate,
                "0RSTpl",
                "CSharp.ReSharperLiveTemplateName");

            // types/organization
            Register(typeof(AssemblyName), Assembly, "0A", "CSharp.AssemblyName");
            Register(typeof(AssemblyVersion), AssemblyVersion, "0V", "CSharp.AssemblyVersion");
            Register(typeof(NamespaceName), Namespace, "0N", "CSharp.NamespaceName");

            //types
            RegisterTypeMapping(
                typeof(TypeName),
                typeof(ArrayTypeName),
                typeof(DelegateTypeName),
                typeof(TypeParameterName));
            Register(typeof(TypeName), Type, "0T", "CSharp.TypeName");
        }

        #region code elements

        private IName Alias(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Event(string arg)
        {
            return new EventName(arg);
        }

        private IName Field(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Lambda(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName LocalVar(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Method(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Parameter(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Property(string arg)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region ide components & other

        private IName CommandBarControl(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Command(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Document(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName ProjectItem(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Project(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Solution(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName Window(string arg)
        {
            throw new System.NotImplementedException();
        }

        private IName ReSharperLiveTemplate(string arg)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region types

        private static IName Assembly(string input)
        {
            return TypeUtils.CreateTypeName(input);
        }

        private static IName AssemblyVersion(string input)
        {
            return TypeUtils.CreateTypeName(input);
        }

        private static IName Namespace(string input)
        {
            return TypeUtils.CreateTypeName(input);
        }

        private static IName Type(string input)
        {
            return TypeUtils.CreateTypeName(input);
        }

        #endregion
    }
}