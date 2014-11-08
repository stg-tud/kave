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
 *    - Sebastian Proksch
 */

using System;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    [Ignore]
    internal class TriggerLocationsTest : BaseSSTAnalysisTest
    {
        [Test]
        public void TriggeredOutsideMethod()
        {
            CompleteInClass(@"
                public void A() {}
                $
            ");

            var sst = NewSST();
            sst.Methods.Add(NewMethodDeclaration(Fix.Void, "A"));
            sst.Add(new CompletionTrigger());

            AssertResult(sst);
        }

        [Test]
        public void TriggeredOutsideMethod_WithToken()
        {
            CompleteInClass(@"
                public void A() {}
                B$
            ");

            var sst = NewSST();
            sst.Methods.Add(NewMethodDeclaration(Fix.Void, "A"));

            sst.Add(new CompletionTrigger {Token = "B"});

            AssertResult(sst);
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredInMethodDeclaration()
        {
            CompleteInClass(@"
                public void$ A() {}
            ");
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredInParameterList()
        {
            CompleteInClass(@"
                public void A($) {}
            ");
        }

        [Test]
        public void TriggeredInMethod()
        {
            CompleteInClass(@"
                public void A()
                {
                    $
                }
            ");

            var trigger = new CompletionTrigger();

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(trigger);

            var sst = NewSST();
            sst.Methods.Add(mA);

            AssertResult(sst);
        }

        [Test]
        public void TriggeredInMethod_WithToken()
        {
            CompleteInClass(@"
                public void A()
                {
                    a.b$
                }
            ");

            var trigger = new CompletionTrigger {Token = "a.b"};

            var mA = NewMethodDeclaration(Fix.Void, "A");
            mA.Body.Add(trigger);

            var sst = NewSST();
            sst.Methods.Add(mA);

            AssertResult(sst);
        }

        [Test, ExpectedException(typeof (NotSupportedException))]
        public void TriggeredInInterface()
        {
            CompleteInFile(@"
                public interface I {
                    public void A();
                    $
                }
            ");

            // TODO think about this again... what about abstract base classes?
        }
    }
}