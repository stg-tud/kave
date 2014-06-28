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
 *    - Dennis Albrecht
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;
using Smile;

namespace KaVE.VsFeedbackGenerator.Tests.CodeCompletion
{
    [TestFixture]
    internal class SmileTest
    {
        private static readonly IIoUtils IoUtils = new IoUtils();
        private static readonly string TmpDir = IoUtils.GetTempDirectoryName();

        [Test]
        public void Tutorial1()
        {
            var net = new Network();
            net.AddNode(Network.NodeType.Cpt, "Success");
            net.SetOutcomeId("Success", 0, "Success");
            net.SetOutcomeId("Success", 1, "Failure");
            net.AddNode(Network.NodeType.Cpt, "Forecast");
            net.AddOutcome("Forecast", "Good");
            net.AddOutcome("Forecast", "Moderate");
            net.AddOutcome("Forecast", "Poor");
            net.DeleteOutcome("Forecast", 0);
            net.DeleteOutcome("Forecast", 0);
            net.AddArc("Success", "Forecast");
            double[] aSuccessDef = {0.2, 0.8};
            net.SetNodeDefinition("Success", aSuccessDef);
            double[] aForecastDef = {0.4, 0.4, 0.2, 0.1, 0.3, 0.6};
            net.SetNodeDefinition("Forecast", aForecastDef);
            // Changing the nodes' spacial and visual attributes:
            net.SetNodePosition("Success", 20, 20, 80, 30);
            net.SetNodeBgColor("Success", Color.Tomato);
            net.SetNodeTextColor("Success", Color.White);
            net.SetNodeBorderColor("Success", Color.Black);
            net.SetNodeBorderWidth("Success", 2);
            net.SetNodePosition("Forecast", 30, 100, 60, 30);
            net.WriteFile(GetFilePath("tutorial_1.xdsl"));
        }

        [Test]
        public void Tutorial2()
        {
            var net = new Network();
            net.ReadFile(GetFilePath("tutorial_1.xdsl"));
            net.UpdateBeliefs();
            var aForecastOutcomeIds = net.GetOutcomeIds("Forecast");
            int outcomeIndex;
            for (outcomeIndex = 0; outcomeIndex < aForecastOutcomeIds.Length; outcomeIndex++)
            {
                if ("Moderate".Equals(aForecastOutcomeIds[outcomeIndex]))
                {
                    break;
                }
            }
            var aValues = net.GetNodeValue("Forecast");
            var pForecastIsModerate = aValues[outcomeIndex];

            Console.WriteLine(@"P(""Forecast"" = Moderate) = " + pForecastIsModerate);
            net.SetEvidence("Forecast", "Good");
            net.UpdateBeliefs();
            // Getting the index of the "Failure" outcome:
            var aSuccessOutcomeIds = net.GetOutcomeIds("Success");
            for (outcomeIndex = 0; outcomeIndex < aSuccessOutcomeIds.Length; outcomeIndex++)
            {
                if ("Failure".Equals(aSuccessOutcomeIds[outcomeIndex]))
                {
                    break;
                }
            }


            // Getting the value of the probability:
            aValues = net.GetNodeValue("Success");
            var pSuccIsFailGivenForeIsGood = aValues[outcomeIndex];

            Console.WriteLine(@"P(""Success"" = Failure | ""Forecast"" = Good) = " + pSuccIsFailGivenForeIsGood);
            net.ClearEvidence("Forecast");
            net.SetEvidence("Forecast", "Poor");
            net.UpdateBeliefs();
            // Getting the index of the "Failure" outcome:
            aSuccessOutcomeIds = net.GetOutcomeIds("Success");
            for (outcomeIndex = 0; outcomeIndex < aSuccessOutcomeIds.Length; outcomeIndex++)
            {
                if ("Failure".Equals(aSuccessOutcomeIds[outcomeIndex]))
                {
                    break;
                }
            }

            // Getting the value of the probability:
            aValues = net.GetNodeValue("Success");
            var pSuccIsSuccGivenForeIsPoor = aValues[outcomeIndex];

            Console.WriteLine(@"P(""Success"" = Success | ""Forecast"" = Poor) = " + pSuccIsSuccGivenForeIsPoor);
        }

        [Test]
        public void Tutorial3()
        {
            var net = new Network();
            net.ReadFile(GetFilePath("tutorial_1.xdsl"));
            net.AddNode(Network.NodeType.List, "Invest");
            net.SetOutcomeId("Invest", 0, "Invest");
            net.SetOutcomeId("Invest", 1, "DoNotInvest");
            net.AddNode(Network.NodeType.Table, "Gain");
            net.AddArc("Invest", "Gain");
            net.AddArc("Success", "Gain");
            double[] aGainDef = {10000, -5000, 500, 500};
            net.SetNodeDefinition("Gain", aGainDef);
            net.WriteFile(GetFilePath("tutorial_3.xdsl"));
        }

        [Test]
        public void Tutorial4()
        {
            var net = new Network();
            net.ReadFile(GetFilePath("tutorial_3.xdsl"));
            net.UpdateBeliefs();
            var aValueIndexingParents = net.GetValueIndexingParents("Gain");
            var nodeDecision = aValueIndexingParents[0];
            var decisionName = net.GetNodeName(nodeDecision);
            Console.WriteLine(@"These are the expected utilities:");
            for (var i = 0; i < net.GetOutcomeCount(nodeDecision); i++)
            {
                var parentOutcomeId = net.GetOutcomeId(nodeDecision, i);
                var expectedUtility = net.GetNodeValue("Gain")[i];

                Console.Write(@"  - """ + decisionName + @""" = " + parentOutcomeId + @": ");
                Console.WriteLine(@"Expected Utility = " + expectedUtility);
            }
        }

        [Test]
        public void Tutorial5()
        {
            var net = new Network();
            net.ReadFile(GetFilePath("tutorial_3.xdsl"));
            var voi = new ValueOfInfo(net);
            net.GetNode("Forecast");
            net.GetNode("Invest");
            voi.AddNode("Forecast");
            voi.SetDecision("Invest");
            voi.Update();
            var results = voi.GetValues();
            var eviForecast = results[0];
            Console.WriteLine(@"Expected Value of Information (""Forecast"") = " + eviForecast);
        }

        [Test]
        public void ProposalExample()
        {
            var net = ExemplaryProposalProvider.CreateProposalNetwork(new[] {"Init", "Execute", "Finish"});

            net.UpdateBeliefs();
            Console.WriteLine(@"Without Evidences: " + FormatArray(net.GetNodeValue("Proposal")));

            SetEvidences(net, "False", "False", "False");
            SetEvidences(net, "True", "False", "False");
            SetEvidences(net, "True", "True", "False");
            SetEvidences(net, "True", "True", "True");
        }

        private void SetEvidences(Network net, string init, string execute, string finish)
        {
            net.SetEvidence("Init", init);
            net.SetEvidence("Execute", execute);
            net.SetEvidence("Finish", finish);
            net.UpdateBeliefs();
            Console.WriteLine(
                @"Init:" + init[0] + @" Execute:" + execute[0] + @" Finish:" + finish[0] + @" Proposal: " +
                FormatArray(net.GetNodeValue("Proposal")));
            net.ClearAllEvidence();
        }

        private string FormatArray(IEnumerable<double> array)
        {
            return string.Join(", ", array.Select(d => d.ToString(CultureInfo.InvariantCulture)));
        }

        private string GetFilePath(string fileName)
        {
            Console.WriteLine(TmpDir);
            return IoUtils.Combine(TmpDir, fileName);
        }
    }
}