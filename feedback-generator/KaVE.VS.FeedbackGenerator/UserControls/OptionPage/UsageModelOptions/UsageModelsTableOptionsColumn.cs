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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.UI.Avalon;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions
{
    internal class UsageModelsTableOptionsColumn : DataGridBoundColumn
    {
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var grid = new Grid();

            var usageModelsTableRow = dataItem as UsageModelsTableRow;
            if (usageModelsTableRow != null)
            {
                grid.AddColumnChild(
                    "Auto",
                    GenerateButton(
                        "Install",
                        usageModelsTableRow.Install,
                        usageModelsTableRow.IsInstallable));

                grid.AddColumnChild(
                    "Auto",
                    GenerateButton(
                        "Update",
                        usageModelsTableRow.Update,
                        usageModelsTableRow.IsUpdateable));

                grid.AddColumnChild(
                    "Auto",
                    GenerateButton("Remove", usageModelsTableRow.Remove, usageModelsTableRow.IsRemoveable));
            }

            return grid;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return GenerateElement(cell, dataItem);
        }

        private static Button GenerateButton(string content, ICommand command, bool enabled)
        {
            return new Button
            {
                Content = content,
                Command = command,
                IsEnabled = enabled,
                Visibility = enabled ? Visibility.Visible : Visibility.Collapsed
            };
        }
    }
}