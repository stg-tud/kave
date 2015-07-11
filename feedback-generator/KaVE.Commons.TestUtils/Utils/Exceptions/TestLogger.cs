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

namespace KaVE.Commons.TestUtils.Utils.Exceptions
{
    public class TestLogger : ILogger
    {
        public delegate void LogInfoHandler(string info);

        public event LogInfoHandler InfoLogged = delegate { };

        public delegate void LogErrorHandler(Exception e, string content);

        public event LogErrorHandler ErrorLogged = delegate { };

        public TestLogger(bool failOnError)
        {
            if (failOnError)
            {
                ErrorLogged += (exception, content) =>
                {
                    if (exception == null)
                    {
                        Assert.Fail(content);
                    }
                    else
                    {
                        Assert.Fail("{0}\r\n{1}", content, exception);
                    }
                };
            }
        }

        public void Error(Exception exception, string content, params object[] args)
        {
            ErrorLogged(exception, string.Format(content, args));
        }

        public void Error(Exception exception)
        {
            Error(exception, "");
        }

        public void Error(string content, params object[] args)
        {
            Error(null, content, args);
        }

        public void Info(string info, params object[] args)
        {
            InfoLogged(string.Format(info, args));
        }
    }
}