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
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.Statistics.Calculators.BaseClasses;
using KaVE.VS.Statistics.Filters;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics.Calculators
{
    [ShellComponent]
    public class SolutionCalculator : StatisticCalculator
    {
        private static readonly string[] FileExtensionsOfSourceCodeItems = {".cs", ".cpp", ".c"};

        public SolutionCalculator(IStatisticListing statisticListing, IMessageBus messageBus, IErrorHandler errorHandler)
            : base(statisticListing, messageBus, errorHandler, typeof (SolutionStatistic), new SolutionFilter()) {}

        protected override IStatistic Process(IDEEvent @event)
        {
            var solutionEvent = @event as SolutionEvent;
            if (solutionEvent == null)
            {
                return null;
            }

            var solutionStatistic = (SolutionStatistic) StatisticListing.GetStatistic(StatisticType);

            var targetIdentifier = solutionEvent.Target == null ? "" : solutionEvent.Target.Identifier;

            switch (solutionEvent.Action)
            {
                case SolutionEvent.SolutionAction.OpenSolution:
                    solutionStatistic.SolutionsOpened++;
                    break;
                case SolutionEvent.SolutionAction.RenameSolution:
                    solutionStatistic.SolutionsRenamed++;
                    break;
                case SolutionEvent.SolutionAction.CloseSolution:
                    solutionStatistic.SolutionsClosed++;
                    break;
                case SolutionEvent.SolutionAction.AddSolutionItem:
                    solutionStatistic.SolutionItemsAdded++;
                    break;
                case SolutionEvent.SolutionAction.RenameSolutionItem:
                    solutionStatistic.SolutionItemsRenamed++;
                    break;
                case SolutionEvent.SolutionAction.RemoveSolutionItem:
                    solutionStatistic.SolutionItemsRemoved++;
                    break;
                case SolutionEvent.SolutionAction.AddProject:
                    solutionStatistic.ProjectsAdded++;
                    break;
                case SolutionEvent.SolutionAction.RenameProject:
                    solutionStatistic.ProjectsRenamed++;
                    break;
                case SolutionEvent.SolutionAction.RemoveProject:
                    solutionStatistic.ProjectsRemoved++;
                    break;
                case SolutionEvent.SolutionAction.AddProjectItem:
                    solutionStatistic.ProjectItemsAdded++;
                    if (IsTestItem(targetIdentifier))
                    {
                        solutionStatistic.TestClassesCreated++;
                    }
                    break;
                case SolutionEvent.SolutionAction.RenameProjectItem:
                    solutionStatistic.ProjectItemsRenamed++;
                    break;
                case SolutionEvent.SolutionAction.RemoveProjectItem:
                    solutionStatistic.ProjectItemsRemoved++;
                    if (IsTestItem(targetIdentifier))
                    {
                        solutionStatistic.TestClassesCreated--;
                    }
                    break;
            }

            return solutionStatistic;
        }

        public static bool IsTestItem(string targetIdentifier)
        {
            return
                !FileExtensionsOfSourceCodeItems.Where(
                    fileExtensionOfSourceCodeItems => targetIdentifier.EndsWith("Test" + fileExtensionOfSourceCodeItems))
                                                .IsEmpty();
        }
    }
}