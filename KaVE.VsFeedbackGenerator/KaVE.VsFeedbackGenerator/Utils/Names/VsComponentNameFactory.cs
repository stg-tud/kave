using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
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
            return window == null ? null : WindowName.Get(window.Type + " " + window.Caption);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static SolutionName GetName([CanBeNull] this Solution solution)
        {
            return solution == null ? null : SolutionName.Get(solution.FullName);
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
            return DocumentName.Get(document.Kind + " " + document.Language + " " + documentName);
        }

        private static string GetSolutionRelativeName(this Document document)
        {
            var fullDocumentName = document.FullName;
            var solution = document.DTE.Solution;

            // If no solution is opened, we cannot determine a relative name.
            if (solution == null)
            {
                return fullDocumentName;
            }

            // If the file is inside a project (which may or may not be placed within the solution, on the filesystem)
            // its full name starts with the project's full name, which we abbreviate to the project's name.
            foreach (var project in solution.Projects.Cast<Project>())
            {
                try
                {
                    var projectPath = project.FullName;
                    if (projectPath.Any() && fullDocumentName.StartsWith(projectPath))
                    {
                        var projectRelativeName = fullDocumentName.Substring(projectPath.Length);
                        return string.Format("\\{0}{1}", project.Name, projectRelativeName);
                    }
                }
                catch (NotImplementedException)
                {
                    // sometimes project.FullName throws this exception for reasons I don't understand (project is a
                    // System._ComObject in these cases). We just ignore this here and continue trying.
                }
            }

            // If the file is on solution level, its full name starts with the solution's full name (which is a prefix
            // if the name of all projects placed inside the solution on the filesystem), which we remove.
            var solutionPath = solution.FullName;
            if (fullDocumentName.StartsWith(solutionPath))
            {
                return fullDocumentName.Substring(solutionPath.Length);
            }
            return fullDocumentName;
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
            return projectItem == null ? null : ProjectItemName.Get(projectItem.Kind + " " + projectItem.Name);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static ProjectName GetName([CanBeNull] this Project project)
        {
            return project == null ? null : ProjectName.Get(project.Kind + " " + project.UniqueName);
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