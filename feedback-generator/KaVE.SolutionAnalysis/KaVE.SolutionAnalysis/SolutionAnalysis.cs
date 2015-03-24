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
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace KaVE.SolutionAnalysis
{
    [SolutionComponent]
    public class SolutionAnalysis
    {
        public static IList<String> AnalyzedProjects = new List<string>();
        public static IList<String> AnalyzedFiles = new List<string>();

        public SolutionAnalysis(ISolution solution)
        {
            var projects = solution.GetAllProjects().Where(NotDefaultProject);
            projects.ForEach(AnalyzeProject);
        }

        private void AnalyzeProject(IProject project)
        {
            AnalyzedProjects.Add(project.Name);
            AnalyzedFiles.AddRange(Directory.GetFiles(project.Location.FullPath, "*.cs", SearchOption.AllDirectories));
        }

        private bool NotDefaultProject(IProject project)
        {
            return !project.Name.Equals("Miscellaneous Files") && !project.Name.Equals("&");
        }
    }
}