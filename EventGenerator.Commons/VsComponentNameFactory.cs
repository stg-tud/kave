﻿using System.Collections.Generic;
using System.Linq;
using CodeCompletion.Model.Names.VisualStudio;
using EnvDTE;

namespace EventGenerator.Commons
{
    public static class VsComponentNameFactory
    {
        public static WindowName GetNameOf(Window window)
        {
            return window == null ? null : WindowName.Get(window.Type + " " + window.Caption);
        }

        public static SolutionName GetNameOf(Solution solution)
        {
            return solution == null ? null : SolutionName.Get(solution.FullName);
        }

        public static IEnumerable<WindowName> GetNamesOf(Windows windows)
        {
            return (from Window window in windows select GetNameOf(window));
        }

        public static DocumentName GetNameOf(Document document)
        {
            return document == null ? null : DocumentName.Get(document.Kind + " " + document.Language + " " + document.FullName);
        }

        public static IEnumerable<DocumentName> GetNamesOf(Documents documents)
        {
            return (from Document document in documents select GetNameOf(document));
        }

        public static ProjectItemName GetNameOf(ProjectItem projectItem)
        {
            return projectItem == null ? null : ProjectItemName.Get(projectItem.Kind + " " + projectItem.Name);
        }

        public static ProjectName GetNameOf(Project project)
        {
            return project == null ? null : ProjectName.Get(project.Kind + " " + project.UniqueName);
        }

        public static CommandName GetNameOf(Command command)
        {
            return command == null ? null : CommandName.Get("{" + command.Guid + "}:" + command.ID + ":" + command.Name);
        }
    }
}
