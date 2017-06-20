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

using System;
using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;

namespace KaVE.RS.SolutionAnalysis.UserStatistics
{
    class UserStatsRunner
    {
        private readonly UserStatsIo _io;

        public UserStatsRunner(UserStatsIo io)
        {
            _io = io;
        }

        public void Run()
        {
            ISet<UserStats> stats = new HashSet<UserStats>();
            foreach (var user in _io.FindZips())
            {
                Console.Write(user + "\t");
                if (user.StartsWith("DATEV"))
                {
                    Console.WriteLine(Positions.SoftwareEngineer);
                }
                else
                {
                    var upe = FindUpe(user);
                    if (upe != null)
                    {
                        Console.WriteLine(upe.Position);
                    }
                    else
                    {
                        Console.WriteLine(Positions.Unknown);
                    }
                }
            }
            _io.WriteStats(stats);
        }

        private UserProfileEvent FindUpe(string user)
        {
            foreach (var e in _io.Read(user))
            {
                var upe = e as UserProfileEvent;
                if (upe != null)
                {
                    return upe;
                }
            }
            return null;
        }

        private static UserStats Extract(IEnumerable<IDEEvent> es)
        {
            var us = new UserStats();
            foreach (var e in es)
            {
                var upe = e as UserProfileEvent;
                if (upe != null)
                {
                    us.Position = upe.Position;
                }
            }
            return us;
        }
    }
}