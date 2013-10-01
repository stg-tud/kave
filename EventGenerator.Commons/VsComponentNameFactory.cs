using System.Collections.Generic;
using System.Linq;
using CodeCompletion.Model.Names.VisualStudio;
using CodeCompletion.Utils.Assertion;
using EnvDTE;
using JetBrains.Annotations;
using Microsoft.VisualStudio.CommandBars;

namespace EventGenerator.Commons
{
    public static class VsComponentNameFactory
    {
        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static WindowName GetNameOf([CanBeNull] Window window)
        {
            return window == null ? null : WindowName.Get(window.Type + " " + window.Caption);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static SolutionName GetNameOf([CanBeNull] Solution solution)
        {
            return solution == null ? null : SolutionName.Get(solution.FullName);
        }

        [NotNull]
        public static IList<WindowName> GetNamesOf([NotNull] Windows windows)
        {
            Asserts.NotNull(windows, "windows");
            return (from Window window in windows select GetNameOf(window)).ToList();
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static DocumentName GetNameOf([CanBeNull] Document document)
        {
            if (document == null)
            {
                return null;
            }
            var documentName = GetSolutionRelativeName(document);
            return DocumentName.Get(document.Kind + " " + document.Language + " " + documentName);
        }

        private static string GetSolutionRelativeName(Document document)
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
                var projectPath = project.FullName;
                if (projectPath.Any() && fullDocumentName.StartsWith(projectPath))
                {
                    var projectRelativeName = fullDocumentName.Substring(projectPath.Length);
                    return string.Format("\\{0}{1}", project.Name, projectRelativeName);
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
        public static IList<DocumentName> GetNamesOf([NotNull] Documents documents)
        {
            Asserts.NotNull(documents, "documents");
            return (from Document document in documents select GetNameOf(document)).ToList();
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static ProjectItemName GetNameOf([CanBeNull] ProjectItem projectItem)
        {
            return projectItem == null ? null : ProjectItemName.Get(projectItem.Kind + " " + projectItem.Name);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static ProjectName GetNameOf([CanBeNull] Project project)
        {
            return project == null ? null : ProjectName.Get(project.Kind + " " + project.UniqueName);
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static CommandName GetNameOf([CanBeNull] Command command)
        {
            return command == null ? null : GetNameOfCommand(command.Guid, command.ID, command.Name);
        }

        private static CommandName GetNameOfCommand(string guid, int id, string name)
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
        public static CommandBarControlName GetNameOf([CanBeNull] CommandBarControl control)
        {
            return control == null
                ? null
                : CommandBarControlName.Get(
                    GetIdentifierOf(control.Parent) + CommandBarControlName.HierarchySeperator + control.Caption);
        }

        private static string GetIdentifierOf([NotNull] CommandBar bar)
        {
            var parent = bar.Parent as CommandBar;
            return parent == null
                ? bar.Name
                : GetIdentifierOf(parent) + CommandBarControlName.HierarchySeperator + bar.Name;
        }
    }
}