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
using System.Runtime.Serialization;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events.TestRunEvents
{
    public class TestCaseResult
    {
        [DataMember, NotNull]
        public IMethodName TestMethod { get; set; }

        [DataMember, NotNull]
        public string Parameters { get; set; }

        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public TimeSpan Duration { get; set; }

        [DataMember]
        public TestResult Result { get; set; }

        public TestCaseResult()
        {
            TestMethod = Names.UnknownMethod;
            Parameters = "";
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(TestCaseResult other)
        {
            return TestMethod.Equals(other.TestMethod) && string.Equals(Parameters, other.Parameters) &&
                   StartTime.Equals(other.StartTime) && Duration.Equals(other.Duration) && Result == other.Result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TestMethod.GetHashCode();
                hashCode = (hashCode*397) ^ Parameters.GetHashCode();
                hashCode = (hashCode*397) ^ StartTime.GetHashCode();
                hashCode = (hashCode*397) ^ Duration.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Result;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }

    public enum TestResult
    {
        Unknown,
        Success,
        Failed,
        Error,
        Ignored
    }
}