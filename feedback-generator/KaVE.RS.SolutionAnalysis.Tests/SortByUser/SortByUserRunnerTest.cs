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

using System.Collections.Generic;
using KaVE.RS.SolutionAnalysis.SortByUser;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests.SortByUser
{
    internal class SortByUserRunnerTest
    {
        [Test]
        public void MergesUsersWithSameProfileId()
        {
            var dictionary = new Dictionary<string, UserIdentifiers>
            {
                {"1.zip", new UserIdentifiers {UserProfileId = "abc"}},
                {"2.zip", new UserIdentifiers {UserProfileId = "abc"}},
                {"3.zip", new UserIdentifiers {UserProfileId = "def"}}
            };

            var actual = SortByUserRunner.AssembleUsers(dictionary);

            var expected = new[]
            {
                new User {Files = {"1.zip", "2.zip"}, Identifiers = new UserIdentifiers {UserProfileId = "abc"}},
                new User {Files = {"3.zip"}, Identifiers = new UserIdentifiers {UserProfileId = "def"}}
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void MergesUsersWithOverlappingSessionIDs()
        {
            var dictionary = new Dictionary<string, UserIdentifiers>
            {
                {"1.zip", new UserIdentifiers {SessionsIDs = {"a", "b"}}},
                {"2.zip", new UserIdentifiers {SessionsIDs = {"c", "d"}}},
                {"3.zip", new UserIdentifiers {SessionsIDs = {"b", "c"}}}
            };

            var actual = SortByUserRunner.AssembleUsers(dictionary);

            var expected = new[]
            {
                new User {Files = {"1.zip", "2.zip", "3.zip"}, Identifiers = new UserIdentifiers {SessionsIDs = {"a", "b", "c", "d"}}},
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void DoesNotMergeIfIdentifiersAreEmpty()
        {
            var dictionary = new Dictionary<string, UserIdentifiers>
            {
                {"1.zip", new UserIdentifiers()},
                {"2.zip", new UserIdentifiers()},
                {"3.zip", new UserIdentifiers()},
            };

            var actual = SortByUserRunner.AssembleUsers(dictionary);

            var expected = new[]
            {
                new User {Files = {"1.zip"}}, new User {Files = {"2.zip"}}, new User {Files = {"3.zip"}}
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}