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
using System.IO;

namespace KaVE.Commons.Utils.Exceptions
{
    public class FileLogger : ILogger
    {
        private readonly string _logFilePath;

        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public virtual void Error(Exception exception, string content, params object[] args)
        {
            var msg = args.Length == 0 ? content : string.Format(content, args);
            LogLine("[ERROR] {0} - {1}\r\n{2}", DateTime.Now, msg, exception);
        }

        public virtual void Error(Exception exception)
        {
            LogLine("[ERROR] {0} - {1}", DateTime.Now, exception);
        }

        public virtual void Error(string content, params object[] args)
        {
            var msg = args.Length == 0 ? content : string.Format(content, args);
            LogLine("[ERROR] {0} - {1}", DateTime.Now, msg);
        }

        public virtual void Info(string content, params object[] args)
        {
            var msg = args.Length == 0 ? content : string.Format(content, args);
            LogLine("[INFO] {0} - {1}", DateTime.Now, msg);
        }

        private void LogLine(string message, params object[] args)
        {
            File.AppendAllText(_logFilePath, string.Format(message + Environment.NewLine, args));
        }
    }
}