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

using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Reflection;
using KaVE.VS.FeedbackGenerator.SessionManager;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager
{
    class ViewModelBaseTest
    {
        private TestViewModel _uut;

        public class TestViewModel : ViewModelBase<TestViewModel>
        {
            public new void SetBusy(string message)
            {
                base.SetBusy(message);
            }

            public new void SetIdle()
            {
                base.SetIdle();
            }
        }

        [SetUp]
        public void SetUp()
        {
            _uut = new TestViewModel();
        }

        [Test]
        public void NotifyBusyMessageChanged()
        {
            var busyMessageChanged = false;
            _uut.OnPropertyChanged(vm => vm.BusyMessage, v => busyMessageChanged = true);

            _uut.BusyMessage = "TestMessage";

            Assert.IsTrue(busyMessageChanged);
        }

        [Test, Timeout(10000)]
        public void AnimatesBusyMessage()
        {
            var update = 0;
            _uut.AnimationRefreshDelayInMillis = 50;
            _uut.OnPropertyChanged(vm => vm.BusyMessageAnimated,
                newValue =>
                {
                    switch (update)
                    {
                        case 0:
                            Assert.AreEqual("Msg   ", newValue);
                            break;
                        case 1:
                            Assert.AreEqual("Msg.  ", newValue);
                            break;
                        case 2:
                            Assert.AreEqual("Msg.. ", newValue);
                            break;
                        case 3:
                            Assert.AreEqual("Msg...", newValue);
                            break;
                        case 4:
                            Assert.AreEqual("Msg   ", newValue);
                            _uut.SetIdle();
                            break;
                    }
                    update++;
                });

            _uut.SetBusy("Msg");

            AsyncTestHelper.WaitForCondition(() => !_uut.IsBusy);
        }
    }
}
