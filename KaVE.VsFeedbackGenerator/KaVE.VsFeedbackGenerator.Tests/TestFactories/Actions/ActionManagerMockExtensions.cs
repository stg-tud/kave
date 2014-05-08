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
using JetBrains.ActionManagement;
using Moq;

namespace KaVE.VsFeedbackGenerator.Tests.TestFactories.Actions
{
    internal static class ActionManagerMockExtensions
    {
        public static void SetupExecutableAction(this Mock<IActionManager> manager, string actionId)
        {
            // TODO configure triggerable action mocks...
            manager.Setup(m => m.GetExecutableAction(actionId)).Returns(new MockExecutableAction(actionId));
        }
    }
}