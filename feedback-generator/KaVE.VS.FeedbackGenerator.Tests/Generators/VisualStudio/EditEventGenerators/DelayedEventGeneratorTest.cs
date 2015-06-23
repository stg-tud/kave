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
 *    - Mattis Manfred Kämmerer
 */

using System;
using EnvDTE;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Utils.Assertion;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators.EventContext;
using KaVE.VS.FeedbackGenerator.Tests.TestFactories;
using KaVE.VS.FeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.VisualStudio.EditEventGenerators
{
    internal class DelayedEventGeneratorTest : EventGeneratorTestBase
    {
        public static Context ValidContext
        {
            get { return new Context {SST = new SST {EnclosingType = TypeName.Get("TestType")}}; }
        }

        private static Document TestDocument
        {
            get
            {
                var documentMock = new Mock<Document>();
                documentMock.Setup(doc => doc.Language).Returns("Project");
                documentMock.Setup(doc => doc.FullName).Returns(@"C:\Project.csproj");
                documentMock.Setup(doc => doc.DTE).Returns(DTEMockUtils.MockSolution(@"C:\Solution.sln").DTE);
                return documentMock.Object;
            }
        }

        private DelayedEditEventGenerator _uut;

        private RetryRunnerTestImpl _retryRunner;

        private ContextProviderTestImpl _contextProvider;

        [SetUp]
        public void Setup()
        {
            TestDateUtils.Now = DateTime.Now;

            _retryRunner = new RetryRunnerTestImpl();
            _contextProvider = new ContextProviderTestImpl();
            _uut = new DelayedEditEventGenerator(
                TestRSEnv,
                MockTestMessageBus.Object,
                TestDateUtils,
                _retryRunner,
                _contextProvider);
        }

        [Test]
        public void ShouldTryToGetContext()
        {
            _contextProvider.ReturnValidContext = true;
            _uut.TryFireWithContext(TestDocument);

            _retryRunner.OnTry();

            Assert.AreEqual(1, _contextProvider.NumberOfCalls);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void TryShouldFailIfContextIsInvalid()
        {
            _contextProvider.ReturnValidContext = false;
            _uut.TryFireWithContext(TestDocument);

            _retryRunner.OnTry();
        }

        [Test]
        public void ShouldFireEventWhenContextIsReady()
        {
            _uut.TryFireWithContext(TestDocument);

            _retryRunner.OnDone(ValidContext);

            System.Threading.Thread.Sleep(100);

            var publishedEvent = GetSinglePublished<EditEvent>();
            Assert.AreNotEqual(publishedEvent.Context2, new Context());
            Assert.AreEqual(publishedEvent.ActiveDocument, TestDocument.GetName());
            Assert.AreEqual(publishedEvent.TriggeredAt, TestDateUtils.Now);
        }
    }

    internal class RetryRunnerTestImpl : IRetryRunner
    {
        public Object Result;

        public Func<Context> OnTry
        {
            get { return (Func<Context>) _onTry; }
        }

        public Action<Context> OnDone
        {
            get { return (Action<Context>) _onDone; }
        }

        private object _onTry;
        private object _onDone;

        // ReSharper disable once CSharpWarnings::CS0693
        public void Try<TResult>(Func<TResult> onTry, TimeSpan retryInterval, int numberOfTries, Action<TResult> onDone)
        {
            _onTry = onTry;
            _onDone = onDone;
        }
    }

    internal class ContextProviderTestImpl : IContextProvider
    {
        public int NumberOfCalls;

        public bool ReturnValidContext;

        public Context GetCurrentContext(Document document)
        {
            NumberOfCalls++;
            return ReturnValidContext
                ? DelayedEventGeneratorTest.ValidContext
                : new Context();
        }

        public Context GetCurrentContext(TextPoint startPoint)
        {
            throw new NotImplementedException("shouldn't call this method!");
        }
    }
}