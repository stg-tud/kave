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

using JetBrains.Application.Extensions;
using JetBrains.Util;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Moq;

namespace KaVE.VsFeedbackGenerator.Tests
{
    internal class TestRSEnv : IRSEnv
    {
        private readonly IIDESession _session;
        private const string KaVEVersion = "1.0-test";

        public TestRSEnv(IIDESession session)
        {
            _session = session;
            MockExtension = new Mock<IExtension>();
            MockExtension.Setup(ex => ex.Version).Returns(SemanticVersion.Parse(KaVEVersion));
            KaVEExtension = MockExtension.Object;
        }

        public Mock<IExtension> MockExtension { get; private set; }
        public IExtension KaVEExtension { get; private set; }

        public IIDESession IDESession
        {
            get { return _session; }
        }
    }
}