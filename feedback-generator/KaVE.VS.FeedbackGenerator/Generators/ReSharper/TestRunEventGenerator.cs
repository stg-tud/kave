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
using System.Collections.Generic;
using System.Linq;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Launch;
using JetBrains.ReSharper.UnitTestFramework.Session;
using JetBrains.Threading;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Utils;
using KaVE.RS.Commons.Utils.Naming;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    // See https://github.com/citizenmatt/resharper-clippy/commit/56bb5fdba58b932ea462c5a3329f7a81c767e836#diff-25
    [SolutionComponent]
    public class TestRunEventGenerator : EventGeneratorBase
    {
        private readonly IUnitTestResultManager _resultManager;
        private readonly IThreading _threading;

        public TestRunEventGenerator(Lifetime lifetime,
            IUnitTestSessionConductor sessionManager,
            IUnitTestResultManager resultManager,
            IThreading threading,
            IRSEnv env,
            IMessageBus messageBus,
            IDateUtils dateUtils)
            : base(env, messageBus, dateUtils, threading)
        {
            _resultManager = resultManager;
            _threading = threading;

            var testSessionLifetimes = new Dictionary<IUnitTestSessionTreeViewModelBase, LifetimeDefinition>();

            sessionManager.SessionOpened.Advise(
                lifetime,
                sessionView =>
                {
                    var sessionLifetimeDefinition = Lifetimes.Define(lifetime, "KaVE::TestRunEventGenerator");
                    testSessionLifetimes.Add(sessionView, sessionLifetimeDefinition);

                    SubscribeToSessionLaunch(sessionLifetimeDefinition.Lifetime, sessionView.Session);
                });

            sessionManager.SessionClosed.Advise(
                lifetime,
                sessionView =>
                {
                    LifetimeDefinition sessionLifetimeDefinition;
                    if (testSessionLifetimes.TryGetValue(sessionView, out sessionLifetimeDefinition))
                    {
                        sessionLifetimeDefinition.Terminate();
                        testSessionLifetimes.Remove(sessionView);
                    }
                });
        }

        private void SubscribeToSessionLaunch(Lifetime sessionLifetime, IUnitTestSession session)
        {
            var sequentialLifetimes = new SequentialLifetimes(sessionLifetime);

            session.Launch.Change.Advise(
                sessionLifetime,
                args =>
                {
                    if (args.HasNew && args.New != null)
                    {
                        sequentialLifetimes.Next(
                            launchLifetime => { SubscribeToLaunchState(launchLifetime, session, args.New); });
                    }
                });
        }

        private void SubscribeToLaunchState(Lifetime launchLifetime,
            IUnitTestSession session,
            IUnitTestLaunch unitTestLaunch)
        {
            var aborted = false;
            unitTestLaunch.Status.Change.Advise(
                launchLifetime,
                args =>
                {
                    if (args.HasNew)
                    {
                        switch (args.New)
                        {
                            case UnitTestLaunchStatus.Building:
                            case UnitTestLaunchStatus.Starting:
                            case UnitTestLaunchStatus.Running:
                                aborted = false;
                                break;
                            case UnitTestLaunchStatus.Stopping:
                                // This will happen if the build failed.
                                if (session.Launch.Value == null)
                                {
                                    break;
                                }
                                // These need to be declared here because session.Launch.Value is null by 
                                // the time the Dispatcher executes the action.
                                var relevantTestElements = session.Launch.Value.Elements.Where(e => !e.Children.Any());
                                var launchTime = session.Launch.Value.StartedOn;
                                _threading.Dispatcher.BeginOrInvoke(
                                    "KaVE::TestStopping",
                                    () =>
                                    {
                                        ReadLockCookie.GuardedExecute(
                                            () =>
                                            {
                                                var results = _resultManager.GetResults(relevantTestElements, session);

                                                CreateAndFireTestRunEvent(launchTime, aborted, results);
                                            });
                                    });
                                break;
                            case UnitTestLaunchStatus.Aborting:
                                aborted = true;
                                break;
                        }
                    }
                });
        }

        private void CreateAndFireTestRunEvent(DateTime launchTime,
            bool aborted,
            IDictionary<IUnitTestElement, UnitTestResult> results)
        {
            var testRunEvent = Create<TestRunEvent>();
            testRunEvent.WasAborted = aborted;
            testRunEvent.TriggeredAt = launchTime;

            foreach (var result in results)
            {
                var declaredElement = result.Key.GetDeclaredElement();
                if (declaredElement == null)
                {
                    continue;
                }

                var substitution = declaredElement.GetIdSubstitutionSafe();
                var methodName = declaredElement.GetName<IName>(substitution) as IMethodName;
                if (methodName == null)
                {
                    continue;
                }

                var singleTestResult = new TestCaseResult
                {
                    TestMethod = methodName,
                    StartTime = result.Value.StartTime,
                    Duration = result.Value.Duration,
                    Parameters = GetParameterSubstringFromShortName(result.Key.ShortName),
                    Result = TranslateStatus(result.Value.Status)
                };

                testRunEvent.Tests.Add(singleTestResult);
            }

            FireNow(testRunEvent);
        }

        public static string GetParameterSubstringFromShortName(string shortName)
        {
            var beginning = shortName.IndexOf("(", StringComparison.Ordinal) + 1;
            if (beginning == 0)
            {
                return "";
            }
            var end = shortName.LastIndexOf(")", StringComparison.Ordinal);
            return shortName.Substring(beginning, end - beginning);
        }

        private static TestResult TranslateStatus(UnitTestStatus status)
        {
            if (status == UnitTestStatus.Success)
            {
                return TestResult.Success;
            }
            if (status == UnitTestStatus.Failed)
            {
                return TestResult.Failed;
            }
            if (status == UnitTestStatus.Ignored)
            {
                return TestResult.Ignored;
            }
            if (status == UnitTestStatus.Inconclusive)
            {
                return TestResult.Error;
            }
            return TestResult.Unknown;
        }
    }
}