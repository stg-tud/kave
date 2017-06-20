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
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.FeedbackProcessor.Preprocessing.Logging
{
    public class AppendingFileLogger : IPrepocessingLogger, IDisposable
    {
        private readonly string _logFile;
        private readonly IDateUtils _dateUtils;

        private bool _isFirstLine;
        private readonly FileStream _fs;
        private readonly StreamWriter _sw;

        public AppendingFileLogger(string logFile, IDateUtils dateUtils)
        {
            _logFile = logFile;
            _dateUtils = dateUtils;

            Asserts.NotNull(dateUtils);
            Asserts.Not(string.IsNullOrEmpty(_logFile));
            _isFirstLine = !File.Exists(logFile);

            var parentDir = Path.GetDirectoryName(logFile);
            Asserts.NotNull(parentDir);
            Asserts.That(Directory.Exists(parentDir));

            _fs = new FileStream(logFile, FileMode.Append, FileAccess.Write);
            _sw = new StreamWriter(_fs);
        }

        public void Log()
        {
            Log("");
        }

        public void Log(string text, params object[] args)
        {
            if (!_isFirstLine)
            {
                _sw.Write('\n');
                _sw.Flush();
            }
            _isFirstLine = false;
            Append("{0} {1}", _dateUtils.Now, args.Length == 0 ? text : string.Format(text, args));
        }

        public void Append(string text, params object[] args)
        {
            _isFirstLine = false;
            var content = args.Length == 0 ? text : string.Format(text, args);
            _sw.Write(content);
        }

        public void Dispose()
        {
            _sw.Dispose();
            _fs.Dispose();
        }
    }
}