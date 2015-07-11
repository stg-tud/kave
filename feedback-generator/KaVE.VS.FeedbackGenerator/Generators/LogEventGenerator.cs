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
using System.Linq;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    public class LogEventGenerator : EventGeneratorBase, ILogger
    {
        private readonly IDateUtils _dateUtils;

        public LogEventGenerator(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _dateUtils = dateUtils;
        }

        public void Error(Exception exception)
        {
            Error(exception, null);
        }

        public void Error(string content, params object[] args)
        {
            Error(null, content, args);
        }

        public virtual void Error(Exception exception, string content, params object[] args)
        {
            var e = CreateErrorEvent();

            if (content != null)
            {
                // TODO does it make sense to move this to the ErrorEvent class?
                e.Content = ReplaceNewLineByBr(string.Format(content, args));
            }

            if (exception != null)
            {
                var lines = exception.ToString().Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
                e.StackTrace = lines
                    .Select(line => line.Trim())
                    .Where(line => line.Length > 0)
                    .ToArray();
            }

            FireNow(e);
        }

        private ErrorEvent CreateErrorEvent()
        {
            // Do not use Create<ErrorEvent>() here, because retrieving the
            // active window/document might freeze the UI, depending on the
            // current error state.
            var e = new ErrorEvent
            {
                TriggeredAt = _dateUtils.Now
            };
            return e;
        }

        private void FireNow(ErrorEvent e)
        {
            // ReSharper disable once EmptyGeneralCatchClause
            try
            {
                FireNow<ErrorEvent>(e);
            }
            catch
            {
                // if logging an error fails here, there's nothing we can do.
            }
        }

        public void Info(string info, params object[] args)
        {
            var e = Create<InfoEvent>();
            e.Info = ReplaceNewLineByBr(string.Format(info, args));
            FireNow(e);
        }

        private static string ReplaceNewLineByBr(string text)
        {
            return text.Replace("\r\n", "<br />").Replace("\n", "<br />");
        }
    }
}