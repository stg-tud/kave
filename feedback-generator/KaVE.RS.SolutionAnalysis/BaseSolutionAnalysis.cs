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

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Modules;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;

namespace KaVE.RS.SolutionAnalysis
{
    public abstract class BaseSolutionAnalysis
    {
        private readonly ISolution _solution;
        private readonly ILogger _logger;

        protected BaseSolutionAnalysis(ISolution solution, ILogger logger)
        {
            _solution = solution;
            _logger = logger;
        }

        /// <summary>
        ///     Requires re-entrency guard (ReentrancyGuard.Current.Execute) and read lock (ReadLockCookie.Execute).
        /// </summary>
        public void AnalyzeAllProjects()
        {
            var projects = _solution.GetAllProjects();
            projects.Remove(_solution.MiscFilesProject);
            projects.Remove(_solution.SolutionProject);
            foreach (var project in projects)
            {
                AnalyzeProject(project);
            }
        }

        private void AnalyzeProject(IProject project)
        {
            _logger.Info("");
            _logger.Info("");
            _logger.Info("###### Analyzing project '{0}'... ################################", project.Name);

            // the correct API usage for the following part is unclear at this point...

            // problem: sometimes, the primary psi module is "null" for TargetFrameworkId.Default
            // and, as a result, we need to code around that.

            // the docu suggests that it is a bad practice to use this anyways, so we try a loop
            // over all known ids (which also includes "default") and pick the first NonNull result
            // Snother option that we have considered is going through all available PSI modules
            // and pick the one that can be cast to an IProjectPSIModule

            var psiModules = _solution.PsiModules();
            IPsiModule primaryPsiModule = null;
            foreach (var id in TargetFrameworkId.AllKnownIds)
            {
                primaryPsiModule = psiModules.GetPrimaryPsiModule(project, id);
                if (primaryPsiModule != null)
                {
                    break;
                }
            }
            Asserts.NotNull(primaryPsiModule, "ReSharper was unable to provide a valid PSI module");
            AnalyzePrimaryPsiModule(primaryPsiModule);
        }

        protected abstract void AnalyzePrimaryPsiModule(IPsiModule primaryPsiModule);
    }
}