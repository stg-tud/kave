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

using JetBrains.DataFlow;
using JetBrains.IDE.SolutionBuilders;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    //[SolutionComponent]
    public class BuildEventGenerator
    {
        public BuildEventGenerator(Lifetime lifetime, ISolutionBuilder builder, BuildIsRunning bir)
        {
            bir.BuildIsRunnning.Change.Advise(
                lifetime,
                val =>
                {
                    if (val.HasNew && val.New)
                    {
                        // new build
                    }
                });

            builder.RunningRequest.Change.Advise(
                lifetime,
                val =>
                {
                    if (val.HasNew)
                    {
                        var req = val.New;
                        req.RequestedProjects[0].State.Change.Advise(
                            lifetime,
                            val2 =>
                            {
                                // ...
                            });
                    }
                });
        }
    }
}