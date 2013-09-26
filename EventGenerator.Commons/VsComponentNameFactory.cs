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
        public static IEnumerable<WindowName> GetNamesOf([NotNull] Windows windows)
        {
            Asserts.NotNull(windows, "windows");
            return (from Window window in windows select GetNameOf(window));
        }

        [ContractAnnotation("notnull => notnull"), CanBeNull]
        public static DocumentName GetNameOf([CanBeNull] Document document)
        {
            return document == null
                ? null
                : DocumentName.Get(document.Kind + " " + document.Language + " " + document.FullName);
        }

        [NotNull]
        public static IEnumerable<DocumentName> GetNamesOf([NotNull] Documents documents)
        {
            Asserts.NotNull(documents, "documents");
            return (from Document document in documents select GetNameOf(document));
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

        [NotNull]
        public static CommandName GetNameOfCommand([NotNull] string guid, int id, [CanBeNull] string name = null)
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
                : CommandBarControlName.Get(GetIdentifierOf(control.Parent) + CommandBarControlName.HierarchySeperator + control.Caption);
        }

        private static string GetIdentifierOf([NotNull] CommandBar bar)
        {
            var parent = bar.Parent as CommandBar;
            return parent == null ? bar.Name : GetIdentifierOf(parent) + CommandBarControlName.HierarchySeperator + bar.Name;
        }
    }
}