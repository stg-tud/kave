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

using KaVE.VS.Achievements.Utils;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Utils
{
    [TestFixture]
    internal class TargetValueProviderTest
    {
        [Test, ExpectedException(typeof (InvalidAchievementIdException),
            ExpectedMessage = "Target Value is not defined in Target_0")]
        public void InvalidAchievementIdTest()
        {
            var uut = new TargetValueProvider();

            uut.GetTargetValue(0);
        }
    }
}