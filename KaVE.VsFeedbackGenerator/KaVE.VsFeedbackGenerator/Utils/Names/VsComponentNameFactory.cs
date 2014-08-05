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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Names.VisualStudio;
using KaVE.Utils.Assertion;
using Microsoft.VisualStudio.CommandBars;

namespace KaVE.VsFeedbackGenerator.Utils.Names
{
    public static class VsComponentNameFactory
    {
        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static WindowName GetName([CanBeNull] this Window window)
        {
            return window == null ? null : GetWindowName(window.Type.ToString(), window.Caption);
        }

        [NotNull]
        public static WindowName GetWindowName([NotNull] string vsWindowType, [NotNull] string caption)
        {
            return WindowName.Get(vsWindowType + " " + caption);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static SolutionName GetName([CanBeNull] this Solution solution)
        {
            return solution == null ? null : GetSolutionName(solution.FullName);
        }

        public static SolutionName GetSolutionName(string fullName)
        {
            return SolutionName.Get(fullName);
        }

        [NotNull]
        public static IList<WindowName> GetNames([NotNull] this Windows windows)
        {
            Asserts.NotNull(windows, "windows");
            return (from Window window in windows select window.GetName()).ToList();
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static DocumentName GetName([CanBeNull] this Document document)
        {
            if (document == null)
            {
                return null;
            }
            var documentName = document.GetSolutionRelativeName();
            var language = document.Language;
            return GetDocumentName(language, documentName);
        }

        [NotNull]
        public static DocumentName GetDocumentName([NotNull] string language, [NotNull] string fileName)
        {
            return DocumentName.Get(language + " " + fileName);
        }

        private static string GetSolutionRelativeName(this Document document)
        {
            var fullDocumentName = document.FullName;
            var solution = document.DTE.Solution;

            if (solution == null || solution.FullName.IsEmpty())
            {
                return Path.GetFileName(fullDocumentName);
            }
            var solutionPath = Path.GetDirectoryName(solution.FullName);
            if (solutionPath != null && fullDocumentName.StartsWith(solutionPath))
            {
                return fullDocumentName.Substring(solutionPath.Length);
            }
            return Path.GetFileName(fullDocumentName);
        }


        [NotNull]
        public static IList<DocumentName> GetNames([NotNull] this Documents documents)
        {
            Asserts.NotNull(documents, "documents");
            return (from Document document in documents select document.GetName()).ToList();
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static ProjectItemName GetName([CanBeNull] this ProjectItem projectItem)
        {
            return projectItem == null ? null : GetProjectItemName(projectItem.Kind, projectItem.Name);
        }

        public static ProjectItemName GetProjectItemName(string kind, string name)
        {
            return ProjectItemName.Get(kind + " " + name);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static ProjectName GetName([CanBeNull] this Project project)
        {
            return project == null ? null : GetProjectName(project.Kind, project.UniqueName);
        }

        public static ProjectName GetProjectName(string kind, string uniqueName)
        {
            return ProjectName.Get(kind + " " + uniqueName);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static CommandName GetName([CanBeNull] this Command command)
        {
            return command == null ? null : GetCommandName(command.Guid, command.ID, command.Name);
        }

        private static CommandName GetCommandName(string guid, int id, string name)
        {
            Asserts.NotNull(guid, "guid");
            var identifier = guid + ":" + id;
            if (name != null)
            {
                identifier += ":" + name;
            }
            return CommandName.Get(identifier);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static CommandBarControlName GetName([CanBeNull] this CommandBarControl control)
        {
            return control == null
                ? null
                : CommandBarControlName.Get(
                    control.Parent.GetIdentifier() + CommandBarControlName.HierarchySeperator + control.Caption);
        }

        private static string GetIdentifier([NotNull] this CommandBar bar)
        {
            var parent = bar.Parent as CommandBar;
            return parent == null
                ? bar.Name
                : parent.GetIdentifier() + CommandBarControlName.HierarchySeperator + bar.Name;
        }
    }
}