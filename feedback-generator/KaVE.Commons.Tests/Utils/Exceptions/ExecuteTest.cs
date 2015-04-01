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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Threading;
using KaVE.Utils.Assertion;
using KaVE.Utils.Exceptions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KaVE.Utils.Tests.Exceptions
{
    [TestFixture]
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

        [Test, ExpectedException(typeof (Exception))]
        public void ShouldNotHandleGeneralException()
        {
            Execute.WithExceptionLogging(_mockLogger.Object, () => { throw new Exception(); });
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
                Assert.That(e.StackTrace, Is.StringContaining("KaVE.Utils.Tests.Exceptions.ExecuteTest.ThrowException()"));
            }
        }

        private static void ThrowException()
        {
            throw new Exception();
        }
    }
}