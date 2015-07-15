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

using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite
{
    internal class UserProvileEventSerializationTest
    {
        [Test]
        public void FromJson_Compact()
        {
            var actual = CurrentJson().ParseJsonTo<IUserProfileEvent>();
            var expected = CurrentObject();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToJson()
        {
            var actual = CurrentObject().ToCompactJson();
            var expected = CurrentJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Roundtrip()
        {
            var actual = CurrentObject().ToCompactJson().ParseJsonTo<IUserProfileEvent>();
            var expected = CurrentObject();
            Assert.AreEqual(expected, actual);
        }

        private static UserProfileEvent CurrentObject()
        {
            return new UserProfileEvent
            {
                ProfileId = "p",
                Education = Educations.Training,
                Position = Positions.HobbyProgrammer,
                ProjectsCourses = true,
                ProjectsPersonal = true,
                ProjectsSharedSmall = true,
                ProjectsSharedMedium = true,
                ProjectsSharedLarge = true,
                TeamsSolo = true,
                TeamsSmall = true,
                TeamsMedium = true,
                TeamsLarge = true,
                CodeReviews = YesNoUnknown.Yes,
                ProgrammingGeneral = Likert7Point.Negative1,
                ProgrammingCSharp = Likert7Point.Negative2,
                Comment = "c"
            };
        }

        private static string CurrentJson()
        {
            return
                "{\"$type\":\"KaVE.Commons.Model.Events.UserProfiles.UserProfileEvent, KaVE.Commons\",\"ProfileId\":\"p\",\"Education\":3,\"Position\":1,\"ProjectsCourses\":true,\"ProjectsPersonal\":true,\"ProjectsSharedSmall\":true,\"ProjectsSharedMedium\":true,\"ProjectsSharedLarge\":true,\"TeamsSolo\":true,\"TeamsSmall\":true,\"TeamsMedium\":true,\"TeamsLarge\":true,\"CodeReviews\":1,\"ProgrammingGeneral\":3,\"ProgrammingCSharp\":2,\"Comment\":\"c\",\"TriggeredBy\":0}";
        }
    }
}