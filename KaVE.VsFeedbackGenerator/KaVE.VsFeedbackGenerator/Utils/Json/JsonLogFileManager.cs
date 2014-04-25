using System;
using System.IO;
using JetBrains.Application;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Utils.Logging;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    [ShellComponent]
    public class IDEEventLogFileManager : LogFileManager<IDEEvent>
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