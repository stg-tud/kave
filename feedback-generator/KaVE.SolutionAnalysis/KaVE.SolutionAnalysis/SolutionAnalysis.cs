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
using System.Linq;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.Threading;
using JetBrains.Util;

namespace KaVE.SolutionAnalysis
{
    [SolutionComponent]
    public class SolutionAnalysis
    {
        public static IList<String> AnalysedProjects = new List<String>();

        public SolutionAnalysis(ISolution solution)
        {
            // Do everything in Analyse(), because in the test environment the solution is not ready yet
            // and thus the test calls StartFromTestContext() when this is the case.
            Analyse(solution);
        }

        private static void Analyse(ISolution solution)
        {
            var projects = solution.GetAllProjects();
            AnalysedProjects.AddRange(projects.Select(project => project.Name));
        }

        internal static void StartFromTestContext(ISolution solution)
        {
            ReentrancyGuard.Current.Execute(
                "solution analysis",
                () =>
                    ReadLockCookie.Execute(
                        () => Analyse(solution)));
        }
    }
}