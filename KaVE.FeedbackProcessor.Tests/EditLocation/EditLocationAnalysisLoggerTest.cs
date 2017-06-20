/*
 * Copyright 2017 Sebastian Proksch
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
using KaVE.FeedbackProcessor.EditLocation;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.EditLocation
{
    internal class EditLocationAnalysisLoggerTest
    {
        [Test]
        public void Integration()
        {
            var sut = new EditLocationAnalysisLogger();

            sut.SearchingZips("C:\\a\\b\\");
            sut.FoundZips(123);

            sut.StartingStatCreation(1);
            sut.StartingStatCreation(2);
            sut.CreatingStats(1, "a/b.zip");
            sut.CreatingStats(1, "c.zip");
            sut.CreatingStats(1, "d.zip");

            sut.IntermediateResults(
                1,
                new EditLocationResults
                {
                    Zip = "a.zip",
                    NumEvents = 10,
                    NumCompletionEvents = 5,
                    NumLocations = 2,
                    AppliedEditLocations =
                    {
                        new RelativeEditLocation {Location = 1, Size = 2}
                    },
                    OtherEditLocations =
                    {
                        new RelativeEditLocation {Location = 3, Size = 4}
                    }
                });

            sut.FinishedStatCreation(1);
            sut.FinishedStatCreation(2);

            var res = new EditLocationResults
            {
                Zip = "all",
                NumEvents = 10,
                NumCompletionEvents = 5,
                NumLocations = 2
            };

            var rng = new Random();
            for (var i = 0; i < 10; i++)
            {
                res.AppliedEditLocations.Add(CreateRandomLocation(rng));
            }
            for (var i = 0; i < 10; i++)
            {
                res.OtherEditLocations.Add(CreateRandomLocation(rng));
            }

            sut.FinalResults(res);
        }

        private static RelativeEditLocation CreateRandomLocation(Random rng)
        {
            var size = (rng.Next() % 28) + 2;
            var loc = (rng.Next() % (size - 1)) + 1;
            var rnd = new RelativeEditLocation {Location = loc, Size = size};
            return rnd;
        }
    }
}