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
using KaVE.Commons.Utils.Exceptions;

namespace KaVE.VS.Achievements.Util
{
    public interface IErrorHandler
    {
        void SendExceptionToLogger(Exception e);
        void SendErrorMessageToLogger(Exception e, string content);
    }

    [ShellComponent]
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger _logger;

        public ErrorHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void SendExceptionToLogger(Exception e)
        {
            _logger.Error(e);
        }

        public void SendErrorMessageToLogger(Exception e, string content)
        {
            _logger.Error(e, content);
        }
    }
}