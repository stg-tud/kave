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
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.TestUtils
{
    public class TestRunnerLogger : ILogger
    {
        private readonly ILogger _logger;
        private readonly string _logPath;
        private bool _hasError;

        public TestRunnerLogger(string logPath)
        {
            _logger = new FileLogger(logPath);
            _logPath = logPath;
        }

        public void Error(Exception exception, string content, params object[] args)
        {
            _hasError = true;
            WriteSeparator();
            Console.WriteLine(content, args);
            Console.WriteLine(exception);
            _logger.Error(exception, content, args);
        }

        private static void WriteSeparator()
        {
            Console.WriteLine(@"==========================================================================");
        }

        public void Error(Exception exception)
        {
            _hasError = true;
            WriteSeparator();
            Console.WriteLine(exception);
            _logger.Error(exception);
        }

        public void Error(string content, params object[] args)
        {
            _hasError = true;
            WriteSeparator();
            Console.WriteLine(content, args);
            _logger.Error(content, args);
        }

        public void Info(string info, params object[] args)
        {
            _logger.Info(info, args);
        }

        public void EndPossibleErrorBlock()
        {
            if (_hasError)
            {
                WriteSeparator();
            }
        }

        public void AssertNoError()
        {
            if (_hasError)
            {
                Assert.Fail("execution produced at least one error, see log for more details: {0}\n", _logPath);
            }
        }
    }
}