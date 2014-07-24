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
using KaVE.Model.SSTs;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite.SSTAnalysisFixture;

namespace KaVE.VsFeedbackGenerator.RS8Tests.Analysis.SSTAnalysisTestSuite
{
    [Ignore]
    internal class MemberDeclaration : BaseSSTAnalysisTest
    {
        [Test]
        public void Simple()
        {
            CompleteInClass(@"
                public bool _b;
                public int I { get; set; }
                public void M() { }
                public delegate void D(object o);
                public event D E;
                $
            ");

            var sst = NewSST();

            // TODO @Seb: Properties und Events wie Methoden behandeln (können überschrieben werden)
            sst.Add(new FieldDeclaration("_b", Fix.Bool));
            sst.Add(new PropertyDeclaration("I", Fix.Int));
            sst.Add(NewMethodDeclaration(Fix.Void, "M"));
            sst.Add(new DelegateDeclaration("I", MethodName.Get("Delegate Syntax")));
            sst.Add(new EventDeclaration("E", TypeName.Get("MyClass")));

            Assert.AreEqual(ResultSST, sst);
        }
    }
}