using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Application;
using KaVE.Utils.Assertion;

#if !DEBUG
using System.IO.Compression;
#endif

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    [ShellComponent]
    public class JsonLogFileManager
    {
        /// <summary>
        /// Usually something like "C:\Users\%USERNAME%\AppData\Roaming\"
        /// </summary>
        private static readonly string AppDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);

        private const string ProjectName = "KaVE";
        private static readonly string EventLogsScope = typeof (JsonLogFileManager).Assembly.GetName().Name;

        /// <summary>
        /// E.g., "C:\Users\%USERNAME%\AppData\Roaming\KaVE\KaVE.VsFeedbackGenerator\"
        /// </summary>
        private static readonly string EventLogsPath = Path.Combine(AppDataPath, ProjectName, EventLogsScope);

#if DEBUG
        private const string LogFileExtension = ".log";
#else
        private const string LogFileExtension = ".log.gz";
#endif

        /// <summary>
        /// Retrieves the file names of all existing log files.
        /// </summary>
        public IEnumerable<string> GetLogFileNames()
        {
            return Directory.Exists(EventLogsPath)
                ? Directory.GetFiles(EventLogsPath, "*" + LogFileExtension)
                : new string[0];
        }

        /// <summary>
        /// Creates the name of a log file with the given sessionUUID as the file's base name. Neither the file itself
        /// nor its parent directory is guaranteed to exist.
        /// </summary>
        public string GetLogFileName(string sessionUUID)
        {
            return Path.Combine(EventLogsPath, sessionUUID + LogFileExtension);
        }

        /// <summary>
        /// Creates a writer on the given log file. If the file exists, the writer will append to it. If the file
        /// doesn't exist, it and any non-existent parent directories will be created.
        /// </summary>
        public JsonLogWriter NewLogWriter(string logFileName)
        {
            EnsureParentDirectoryExists(logFileName);
            Stream logStream = new FileStream(logFileName, FileMode.Append, FileAccess.Write);
            try
            {
#if !DEBUG
                logStream = new GZipStream(logStream, CompressionMode.Compress);
#endif
                return new JsonLogWriter(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
            }
        }

        private static void EnsureParentDirectoryExists(string fileName)
        {
            var parentDirectory = Path.GetDirectoryName(fileName);
            Asserts.NotNull(parentDirectory, "could not determine parent directory from path '{0}'", fileName);
            Directory.CreateDirectory(parentDirectory);
        }

        /// <summary>
        /// Creates a reader on a given log file. The file is expected to exist.
        /// </summary>
        public JsonLogReader NewLogReader(string logFileName)
        {
            Asserts.That(File.Exists(logFileName), "log file '{0}' doesn't exist", logFileName);
            Stream logStream = new FileStream(logFileName, FileMode.Open);
            try
            {
#if !DEBUG
                logStream = new GZipStream(logStream, CompressionMode.Decompress);
#endif
                return new JsonLogReader(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
            }
        }
    }
}