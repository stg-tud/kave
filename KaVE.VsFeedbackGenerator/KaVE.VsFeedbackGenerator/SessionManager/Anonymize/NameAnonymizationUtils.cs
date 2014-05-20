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
 *    - 
 */

using System;
using System.Security.Cryptography;
using JetBrains.Annotations;
using KaVE.Model.Names;
using KaVE.Model.Names.VisualStudio;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.SessionManager.Anonymize
{
    internal static class NameAnonymizationUtils
    {
        [JetBrains.Annotations.NotNull]
        public static string ToHash([JetBrains.Annotations.NotNull] this string value)
        {
            var tmpSource = value.AsBytes();
            var hash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return Convert.ToBase64String(hash);
        }

        [ContractAnnotation("notnull => notnull")]
        public static IName ToAnonymousName<TName>(this TName name) where TName : class, IName
        {
            return ToAnonymousName<DocumentName>(name, ToAnonymousName) ??
                   ToAnonymousName<WindowName>(name, ToAnonymousName) ??
                   ToAnonymousName<SolutionName>(name, ToAnonymousName) ??
                   ToAnonymousName<ProjectName>(name, ToAnonymousName) ??
                   ToAnonymousName<ProjectItemName>(name, ToAnonymousName) ??
                   Asserts.Fail<IName>("name type not handled");
        }

        private static TName ToAnonymousName<TName>(IName name, Func<TName, TName> anonymizer)
            where TName : class, IName
        {
            var concreteName = name as TName;
            return concreteName != null ? anonymizer(concreteName) : null;
        }

        [ContractAnnotation("notnull => notnull")]
        public static DocumentName ToAnonymousName([JetBrains.Annotations.CanBeNull] this DocumentName document)
        {
            return document == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetDocumentName, document.Language, document.FileName);
        }

        [ContractAnnotation("notnull => notnull")]
        public static ProjectItemName ToAnonymousName([JetBrains.Annotations.CanBeNull] this ProjectItemName projectItem)
        {
            return projectItem == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetProjectItemName, projectItem.Type, projectItem.Name);
        }

        [ContractAnnotation("notnull => notnull")]
        public static ProjectName ToAnonymousName([JetBrains.Annotations.CanBeNull] this ProjectName project)
        {
            return project == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetProjectName, project.Type, project.Name);
        }

        [ContractAnnotation("notnull => notnull")]
        public static SolutionName ToAnonymousName([JetBrains.Annotations.CanBeNull] this SolutionName solution)
        {
            return solution == null
                ? null
                : VsComponentNameFactory.GetSolutionName(solution.Identifier.ToHash());
        }

        [ContractAnnotation("notnull => notnull")]
        public static WindowName ToAnonymousName([JetBrains.Annotations.CanBeNull] this WindowName window)
        {
            return window == null
                ? null
                : CreateAnonymizedName(VsComponentNameFactory.GetWindowName, window.Type, window.Caption);
        }

        private static TName CreateAnonymizedName<TName>(Func<string, string, TName> factory, string type, string name)
        {
            var isFileName = name.Contains("\\") || name.Contains(".");
            if (isFileName)
            {
                name = name.ToHash();
            }
            return factory(type, name);
        }
    }
}