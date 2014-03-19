using System;
using System.IO;
using JetBrains.Application;
using KaVE.Model.Events;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    internal static class JsonLogFileManagerLocation
    {
        /// <summary>
        ///     Usually something like "C:\Users\%USERNAME%\AppData\Roaming\"
        /// </summary>
        private static readonly string AppDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);

        private const string ProjectName = "KaVE";
        private static readonly string EventLogsScope = typeof(JsonLogFileManagerLocation).Assembly.GetName().Name;

        /// <summary>
        ///     E.g., "C:\Users\%USERNAME%\AppData\Roaming\KaVE\KaVE.VsFeedbackGenerator\"
        /// </summary>
        internal static readonly string EventLogsPath = Path.Combine(AppDataPath, ProjectName, EventLogsScope);
    }

    [ShellComponent]
    public class JsonIDEEventLogFileManager : JsonLogFileManager<IDEEvent> { }

    public class JsonLogFileManager<TMessage> : LogFileManager<TMessage>
    {
        public JsonLogFileManager()
            : base(
                JsonLogFileManagerLocation.EventLogsPath,
                JsonLogIoProvider.JsonFormatWriter<TMessage>(),
                JsonLogIoProvider.CompressedJsonFormatWriter<TMessage>()) { }
    }
}