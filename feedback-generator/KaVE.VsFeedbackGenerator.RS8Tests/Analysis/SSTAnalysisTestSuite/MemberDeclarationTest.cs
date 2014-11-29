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

using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs.Declarations;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    internal class MemberDeclarationTest : BaseSSTAnalysisTest
    {
        [Test]
        public void DelegateDeclaration()
        {
            CompleteInClass(@"
                public delegate void D(object o);
                $
            ");

            var sst = NewSST();

            sst.Delegates.Add(
                new DelegateDeclaration
                {
                    Name = TypeName.Get("d:N.C+D, TestProject")
                });
            Assert.AreEqual(ResultSST, sst);
        }

        [Test]
        public void EventDeclaration()
        {
            CompleteInClass(@"
                public event int E;
                $
            ");

            var sst = NewSST();

            sst.Events.Add(new EventDeclaration());
            Assert.AreEqual(ResultSST, sst);
        }

        [Test]
        public void FieldDeclaration()
        {
            CompleteInClass(@"
                public int _f;
                $
            ");

            var sst = NewSST();

            sst.Fields.Add(new FieldDeclaration());
            Assert.AreEqual(ResultSST, sst);
        }

        [Test]
        public void MethodDeclaration_EntryPoint()
        {
            CompleteInClass(@"
                public void M() {}
                $
            ");

            var sst = NewSST();

            sst.Methods.Add(new MethodDeclaration());
            Assert.AreEqual(ResultSST, sst);
        }

        [Test]
        public void MethodDeclaration_NonEntryPoint()
        {
            CompleteInClass(@"
                private void M() {}
                $
            ");

            var sst = NewSST();

            sst.Methods.Add(new MethodDeclaration());
            Assert.AreEqual(ResultSST, sst);
        }

        [Test]
        public void PropertyDeclaration()
        {
            CompleteInClass(@"
                public int P {get;set;}
                $
            ");

            var sst = NewSST();

            sst.Properties.Add(new PropertyDeclaration());
            Assert.AreEqual(ResultSST, sst);
        }
    }
}