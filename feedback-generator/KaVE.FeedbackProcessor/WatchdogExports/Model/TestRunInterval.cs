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
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.WatchdogExports.Model
{
    public class TestRunInterval : Interval
    {
        public class TestClassResult
        {
            public string TestClassName { get; set; }
            // TODO untested (and not part of hc/eq)
            public DateTime StartedAt { get; set; }

            public TimeSpan Duration { get; set; }
            public TestResult Result { get; set; }
            public IKaVEList<TestMethodResult> TestMethods { get; set; }

            public TestClassResult()
            {
                TestMethods = Lists.NewList<TestMethodResult>();
            }

            protected bool Equals(TestClassResult other)
            {
                return string.Equals(TestClassName, other.TestClassName) && Result == other.Result &&
                       Equals(TestMethods, other.TestMethods);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((TestClassResult) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (TestClassName != null ? TestClassName.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (int) Result;
                    hashCode = (hashCode*397) ^ (TestMethods != null ? TestMethods.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public class TestMethodResult
        {
            public string TestMethodName { get; set; }
            // TODO untested (and not part of hc/eq)
            public DateTime StartedAt { get; set; }

            public TimeSpan Duration { get; set; }
            public TestResult Result { get; set; }

            protected bool Equals(TestMethodResult other)
            {
                return string.Equals(TestMethodName, other.TestMethodName) && Result == other.Result;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((TestMethodResult) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((TestMethodName != null ? TestMethodName.GetHashCode() : 0)*397) ^ (int) Result;
                }
            }
        }

        public string ProjectName { get; set; }
        public TestResult Result { get; set; }
        public IKaVEList<TestClassResult> TestClasses { get; set; }

        public TestRunInterval()
        {
            TestClasses = Lists.NewList<TestClassResult>();
        }

        protected bool Equals(TestRunInterval other)
        {
            return base.Equals(other) && string.Equals(ProjectName, other.ProjectName) && Result == other.Result &&
                   Equals(TestClasses, other.TestClasses);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((TestRunInterval) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (ProjectName != null ? ProjectName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Result;
                hashCode = (hashCode*397) ^ (TestClasses != null ? TestClasses.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}