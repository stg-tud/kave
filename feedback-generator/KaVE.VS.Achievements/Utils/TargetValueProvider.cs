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
using JetBrains.Application;
using KaVE.VS.Achievements.Properties;

namespace KaVE.VS.Achievements.Utils
{
    [ShellComponent]
    public class TargetValueProvider : ITargetValueProvider
    {
        private const string IdPrefix = "Target_";

        public object GetTargetValue(int id)
        {
            try
            {
                return targets.Default[IdPrefix + id];
            }
            catch
            {
                throw new InvalidAchievementIdException(
                    string.Format(
                        "Target Value is not defined in {0}{1}",
                        IdPrefix,
                        id));
            }
        }
    }

    public interface ITargetValueProvider
    {
        object GetTargetValue(int id);
    }

    public class InvalidAchievementIdException : Exception
    {
        public InvalidAchievementIdException(string message)
            : base(message) {}
    }
}