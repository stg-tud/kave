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

using JetBrains.UI.ActionsRevised;

namespace KaVE.VS.FeedbackGenerator.Tests.TestFactories.Actions
{
    internal class MockNextActionHandler
    {
        private readonly MockExecutableAction _action;
        private int _nextIndex;

        private IExecutableAction NextHandler
        {
            get { return _action.GetNextHandler(ref _nextIndex); }
        }

        private MockNextActionHandler MockNext
        {
            get { return new MockNextActionHandler(_action, _nextIndex); }
        }

        public MockNextActionHandler(MockExecutableAction action, int index)
        {
            _action = action;
            _nextIndex = index - 1;
        }

        public bool CallUpdate()
        {
            return NextHandler.Update(_action.DataContext, _action.Presentation, MockNext.CallUpdate);
        }

        public void CallExecute()
        {
            NextHandler.Execute(_action.DataContext, MockNext.CallExecute);
        }
    }
}