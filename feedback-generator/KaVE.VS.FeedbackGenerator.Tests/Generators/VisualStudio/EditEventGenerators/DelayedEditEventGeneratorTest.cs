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
using EnvDTE;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators.EventContext;
using KaVE.VS.FeedbackGenerator.Tests.TestFactories;
using KaVE.VS.FeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.VisualStudio.EditEventGenerators
{
    internal class DelayedEditEventGeneratorTest : EventGeneratorTestBase
    {
        public static Context ValidContext
        {
            get { return new Context {SST = new SST {EnclosingType = TypeName.Get("TestType")}}; }
        }

        private static Mock<Document> _testDocumentMock;

        private DelayedEditEventGenerator _uut;

        private RetryRunnerTestImpl _testRetryRunner;

        private ContextProviderTestImpl _contextProvider;

        [SetUp]
        public void Setup()
        {
            _testDocumentMock = new Mock<Document>();
            _testDocumentMock.Setup(doc => doc.Language).Returns("SomeLanguage");
            _testDocumentMock.Setup(doc => doc.FullName).Returns(@"C:\SomeCSharpFile.cs");
            _testDocumentMock.Setup(doc => doc.DTE).Returns(DTEMockUtils.MockSolution(@"C:\Solution.sln").DTE);

            TestDateUtils.Now = DateTime.Now;

            _testRetryRunner = new RetryRunnerTestImpl();
            _contextProvider = new ContextProviderTestImpl();
            _uut = new DelayedEditEventGenerator(
                TestRSEnv,
                MockTestMessageBus.Object,
                TestDateUtils,
                _testRetryRunner,
                _contextProvider);
        }

        [Test]
        public void ShouldTryToGetContext()
        {
            _uut.TryFireWithContext(_testDocumentMock.Object);

            _testRetryRunner.OnTry();

            Assert.AreEqual(1, _contextProvider.NumberOfCalls);
        }

        [Test]
        public void TryShouldBeSuccessfulIfContextIsValid()
        {
            _contextProvider.CurrentContext = ValidContext;
            _uut.TryFireWithContext(_testDocumentMock.Object);

            Assert.True(_testRetryRunner.OnTry());
        }

        [Test]
        public void TryShouldFailIfContextIsInvalid()
        {
            _contextProvider.CurrentContext = Context.Default;
            _uut.TryFireWithContext(_testDocumentMock.Object);

            Assert.False(_testRetryRunner.OnTry());
        }

        // TODO RS9: generator is disabled for now
        [Test, Ignore]
        public void ShouldFireEventWhenContextIsReady()
        {
            _contextProvider.CurrentContext = ValidContext;
            _uut.TryFireWithContext(_testDocumentMock.Object);

            _testRetryRunner.OnTry();

            System.Threading.Thread.Sleep(100);

            var publishedEvent = GetSinglePublished<EditEvent>();
            Assert.AreNotEqual(publishedEvent.Context2, new Context());
            Assert.AreEqual(publishedEvent.ActiveDocument, _testDocumentMock.Object.GetName());
            Assert.AreEqual(publishedEvent.TriggeredAt, TestDateUtils.Now);
        }

        [Test]
        public void ShouldNotFireWhenNoValidContextIsAvailable()
        {
            _contextProvider.CurrentContext = Context.Default;
            _uut.TryFireWithContext(_testDocumentMock.Object);

            _testRetryRunner.OnTry();

            System.Threading.Thread.Sleep(100);

            AssertNoEvent();
        }

        [Test]
        public void ShouldDoNothingForNonCSharpFiles()
        {
            _testDocumentMock.Setup(doc => doc.FullName).Returns(@"C://NoCSharpFile.xaml");
            _uut.TryFireWithContext(_testDocumentMock.Object);

            Assert.False(_testRetryRunner.WasCalled);
        }
    }

    internal class RetryRunnerTestImpl : IRetryRunner
    {
        public bool WasCalled;

        public object Result;

        public Func<bool> OnTry;

        public void Try(Func<bool> onTry)
        {
            WasCalled = true;
            OnTry = onTry;
        }
    }

    internal class ContextProviderTestImpl : IContextProvider
    {
        public int NumberOfCalls;

        public Context CurrentContext = Context.Default;

        public Context GetCurrentContext(Document document)
        {
            NumberOfCalls++;
            return CurrentContext;
        }

        public Context GetCurrentContext(TextPoint startPoint)
        {
            throw new NotImplementedException();
        }
    }
}