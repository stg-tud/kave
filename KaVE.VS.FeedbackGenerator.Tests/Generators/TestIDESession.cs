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

using EnvDTE;
using KaVE.VS.FeedbackGenerator.VsIntegration;
using Moq;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators
{
    internal class TestIDESession : IIDESession
    {
        private readonly Mock<DTE> _mockDTE;

        public TestIDESession()
        {
            _mockDTE = new Mock<DTE>();
            _mockDTE.Setup(dte => dte.ActiveWindow).Returns((Window) null);
            _mockDTE.Setup(dte => dte.ActiveDocument).Returns((Document) null);
        }

        public Mock<DTE> MockDTE
        {
            get { return _mockDTE; }
        }

        public string UUID
        {
            get { return "TestIDESessionUUID"; }
        }

        public DTE DTE
        {
            get { return _mockDTE.Object; }
        }
    }
}