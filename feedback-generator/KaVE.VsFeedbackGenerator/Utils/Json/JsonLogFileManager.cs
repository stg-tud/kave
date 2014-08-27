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

using System;
using System.IO;
using JetBrains.Application;
using KaVE.VsFeedbackGenerator.Utils.Logging;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    [ShellComponent]
    public class IDEEventLogFileManager : LogFileManager
    {
        /// <summary>
        ///     Usually something like "C:\Users\%USERNAME%\AppData\Roaming\"
        /// </summary>
        private static readonly string AppDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);

        private const string ProjectName = "KaVE";
        private static readonly string EventLogsScope = typeof(IDEEventLogFileManager).Assembly.GetName().Name;

        /// <summary>
        ///     E.g., "C:\Users\%USERNAME%\AppData\Roaming\KaVE\KaVE.VsFeedbackGenerator\"
        /// </summary>
        private static readonly string EventLogsPath = Path.Combine(AppDataPath, ProjectName, EventLogsScope);

        public IDEEventLogFileManager() : base(EventLogsPath) { }
    }
}