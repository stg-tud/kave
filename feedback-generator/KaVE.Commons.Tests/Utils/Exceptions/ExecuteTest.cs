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
using System.Threading;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Exceptions
{
    internal class ExecuteTest
    {
        private Mock<ILogger> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ILogger>();
        }

        private readonly Exception[] _blockedAndLoggedExceptions =
        {
            new AssertException(""),
            new JsonException(),
            new NullReferenceException(),
            new ArgumentException(),
            new ArithmeticException(),
            new IndexOutOfRangeException(),
            new FormatException(),
            new InvalidCastException(),
            new InvalidOperationException(),
            new NotSupportedException()
        };

        [Test, TestCaseSource("_blockedAndLoggedExceptions")]
        public void ShouldLogAssertException(Exception e)
        {
            Execute.WithExceptionLogging(_mockLogger.Object, () => { throw e; });

            _mockLogger.Verify(logger => logger.Error(e, "executing action failed"));
        }

        [Test, ExpectedException(typeof(Exception))]
        public void ShouldNotHandleGeneralException()
        {
            Execute.WithExceptionLogging(_mockLogger.Object, () => { throw new Exception(); });
        }

        [Test, TestCaseSource("_blockedAndLoggedExceptions")]
        public void ShouldLogAssertException_Callback(Exception e)
        {
            var wasCalled = false;
            Execute.WithExceptionCallback(() => { throw e; }, cb => { wasCalled = true; });
            Assert.That(wasCalled);
        }

        [Test, ExpectedException(typeof(Exception))]
        public void ShouldNotHandleGeneralException_Callback()
        {
            var wasCalled = false;
            Execute.WithExceptionCallback(() => { throw new Exception(); }, cb => { wasCalled = true; });
            Assert.False(wasCalled);
        }

        [Test, ExpectedException(typeof(ThreadInterruptedException))]
        public void ShouldNotHandleThreadInterruptedException()
        {
            Execute.WithExceptionLogging(_mockLogger.Object, () => { throw new ThreadInterruptedException(); });
        }

        [Test]
        public void ShouldNotRewriteStacktrace()
        {
            try
            {
                // throw in other method and check that this method remains in the trace
                Execute.WithExceptionLogging(_mockLogger.Object, ThrowException);
            }
            catch (Exception e)
            {
                Assert.That(
                    e.StackTrace,
                    Is.StringContaining("KaVE.Commons.Tests.Utils.Exceptions.ExecuteTest.ThrowException()"));
            }
        }

        [Test, TestCaseSource("_blockedAndLoggedExceptions")]
        public void ShouldSuppressException(Exception e)
        {
            Execute.AndSupressExceptions(() => { throw e; });
        }

        [Test, ExpectedException(typeof(IOException))]
        public void ShouldNotSuppressOtherExceptions()
        {
            Execute.AndSupressExceptions(() => { throw new IOException(); });
        }

        private static void ThrowException()
        {
            throw new Exception();
        }
    }
}