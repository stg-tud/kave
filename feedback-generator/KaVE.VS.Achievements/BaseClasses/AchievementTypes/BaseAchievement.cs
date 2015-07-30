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
using System.Globalization;
using KaVE.Commons.Utils;
using KaVE.VS.Achievements.Properties;
using KaVE.VS.Statistics.Utils;
using Newtonsoft.Json;

namespace KaVE.VS.Achievements.BaseClasses.AchievementTypes
{
    /// <summary>
    ///     Represents any basic Achievement that can only be complete or incomplete
    ///     (e.g. install this plugin)
    /// </summary>
    public class BaseAchievement
    {
        [JsonIgnore]
        public readonly IDateUtils Clock;

        /// <summary>
        ///     Achievement ID;
        ///     Used for identifying Achievements and getting the target value of <see cref="ProgressAchievement" />
        /// </summary>
        public int Id;

        public BaseAchievement(int id)
        {
            Id = id;
            Clock = Registry.GetComponent<IDateUtils>();
        }

        public bool IsCompleted { get; set; }

        /// <summary>
        ///     The Date and Time of when the Achievement was completed
        /// </summary>
        public DateTime CompletionDateTime { get; set; }

        /// <summary>
        ///     The Completion Date in dd/MM/yyyy format when completed;
        ///     "Not Completed" when
        /// </summary>
        [JsonIgnore]
        public string CompletionDate
        {
            get
            {
                return IsCompleted
                    ? CompletionDateTime.Date.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture)
                    : UIText.Achievement_NotCompleted;
            }
        }

        /// <summary>
        ///     Raises a completed event when the Achievement is completed
        /// </summary>
        public static event EventHandler CompletedEventHandler;

        public void Initialize()
        {
            IsCompleted = false;
        }

        public void Unlock()
        {
            if (IsCompleted)
            {
                return;
            }

            IsCompleted = true;
            CompletionDateTime = Clock.Now;

            if (CompletedEventHandler != null)
            {
                RaiseCompletedEvent(this, null);
            }
        }

        private static void RaiseCompletedEvent(object o, EventArgs args)
        {
            if (CompletedEventHandler != null)
            {
                CompletedEventHandler(o, args);
            }
        }
    }
}