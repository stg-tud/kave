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

using System.Linq;
using JetBrains.Application;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Exceptions;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Calculators.BaseClasses;
using KaVE.VS.Statistics.Filters;
using KaVE.VS.Statistics.Statistics;

namespace KaVE.VS.Statistics.Calculators
{
    [ShellComponent]
    public class SolutionCalculator : StatisticCalculator<SolutionStatistic>
    {
        private static readonly string[] FileExtensionsOfSourceCodeItems = {".cs", ".cpp", ".c"};

        public SolutionCalculator(IStatisticListing statisticListing, IMessageBus messageBus, ILogger errorHandler)
            : base(statisticListing, messageBus, errorHandler, new SolutionPreprocessor()) {}

        protected override void Calculate(SolutionStatistic statistic, IDEEvent @event)
        {
            var solutionEvent = @event as SolutionEvent;
            if (solutionEvent == null)
            {
                return;
            }

            var targetIdentifier = solutionEvent.Target == null ? "" : solutionEvent.Target.Identifier;

            switch (solutionEvent.Action)
            {
                case SolutionEvent.SolutionAction.OpenSolution:
                    statistic.SolutionsOpened++;
                    break;
                case SolutionEvent.SolutionAction.RenameSolution:
                    statistic.SolutionsRenamed++;
                    break;
                case SolutionEvent.SolutionAction.CloseSolution:
                    statistic.SolutionsClosed++;
                    break;
                case SolutionEvent.SolutionAction.AddSolutionItem:
                    statistic.SolutionItemsAdded++;
                    break;
                case SolutionEvent.SolutionAction.RenameSolutionItem:
                    statistic.SolutionItemsRenamed++;
                    break;
                case SolutionEvent.SolutionAction.RemoveSolutionItem:
                    statistic.SolutionItemsRemoved++;
                    break;
                case SolutionEvent.SolutionAction.AddProject:
                    statistic.ProjectsAdded++;
                    break;
                case SolutionEvent.SolutionAction.RenameProject:
                    statistic.ProjectsRenamed++;
                    break;
                case SolutionEvent.SolutionAction.RemoveProject:
                    statistic.ProjectsRemoved++;
                    break;
                case SolutionEvent.SolutionAction.AddProjectItem:
                    statistic.ProjectItemsAdded++;
                    if (IsTestItem(targetIdentifier))
                    {
                        statistic.TestClassesCreated++;
                    }
                    break;
                case SolutionEvent.SolutionAction.RenameProjectItem:
                    statistic.ProjectItemsRenamed++;
                    break;
                case SolutionEvent.SolutionAction.RemoveProjectItem:
                    statistic.ProjectItemsRemoved++;
                    if (IsTestItem(targetIdentifier))
                    {
                        statistic.TestClassesCreated--;
                    }
                    break;
            }
        }

        public static bool IsTestItem(string targetIdentifier)
        {
            return
                FileExtensionsOfSourceCodeItems.Any(
                    fileExtensionOfSourceCodeItems => targetIdentifier.EndsWith("Test" + fileExtensionOfSourceCodeItems));
        }
    }
}