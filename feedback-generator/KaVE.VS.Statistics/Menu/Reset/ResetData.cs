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

using System;
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;
using JetBrains.Util;
using KaVE.VS.Statistics.Calculators.BaseClasses;
using KaVE.VS.Statistics.Properties;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.UI;
using KaVE.VS.Statistics.UI.Utils;
using KaVE.VS.Statistics.Utils;
using MessageBox = System.Windows.Forms.MessageBox;

namespace KaVE.VS.Statistics.Menu.Reset
{
    /// <summary>
    ///     Resets all Data
    /// </summary>
    [Action(Id, "Reset", Id = 8645770)]
    public class ResetData : IExecutableAction
    {
        public const string Id = "KaVE.BP.Achievements.Reset";

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            // return true or false to enable/disable this action
            return true;
        }

        /// <summary>
        ///     Calls Reset methods of each component
        /// </summary>
        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var result = MessageBox.Show(
                UIText.ResetData_Dialog,
                UIText.ResetData_Title,
                MessageBoxButtons.YesNo);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                var uiDelegator = Registry.GetComponent<IStatisticsUiDelegator>();

                var listing = Registry.GetComponent<IStatisticListing>();

                listing.BlockUpdateToObservers = true;

                listing.DeleteData();

                Registry.GetComponents<IStatisticCalculator>().ForEach(calculator => calculator.InitializeStatistic());

                Registry.GetComponent<StatisticViewModel>().ResetCollections();

                if (uiDelegator.StatisticUserControl != null)
                {
                    uiDelegator.StatisticUserControl.ResetView();
                }

                listing.BlockUpdateToObservers = false;

                listing.SendUpdateToObserversWithAllStatistics();
            }
            catch (Exception e)
            {
                Registry.GetComponent<IErrorHandler>().SendExceptionToLogger(e);
            }
        }
    }
}