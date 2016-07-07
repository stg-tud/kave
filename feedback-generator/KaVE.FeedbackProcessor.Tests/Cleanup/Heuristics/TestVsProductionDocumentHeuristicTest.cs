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

using KaVE.Commons.Model.Naming;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Heuristics
{
    internal class TestVsProductionDocumentHeuristicTest
    {
        [TestCase("CSharp Foo.cs"),
         TestCase("Plain Text Class.txt")]
        public void IdentifiesProductionDocument(string identifier)
        {
            var document = Names.Document(identifier);
            Assert.IsTrue(document.IsProductionDocument());
        }

        [TestCase("XAML SomethingTest.xaml"),
         TestCase("CSharp Test.xaml.cs"),
         TestCase("CSharp Test.cs"),
         TestCase("CSharp SomeTest.cs"),
         TestCase("CSharp TestBase.cs"),
         TestCase("CSharp \\Project.Tests\\Utils.cs"),
         TestCase("Plain Text sometestdocument.txt")]
        public void IdentifiesTestDocument(string identifier)
        {
            var document = Names.Document(identifier);
            Assert.IsTrue(document.IsTestDocument());
        }
    }
}