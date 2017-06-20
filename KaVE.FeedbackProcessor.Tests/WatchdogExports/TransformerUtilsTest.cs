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
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.FeedbackProcessor.WatchdogExports;
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports
{
    internal class TransformerUtilsTest
    {
        // Method parameters were omitted.
        private const string NUnitTestMethod =
            "static [System.Void, mscorlib, 4.0.0.0] [NUnit.Framework.Assert, nunit.framework, 2.6.4.14350].AreEqual()";

        private const string NUnitTestMethod2 =
            "static [System.Void, mscorlib, 4.0.0.0] [NUnit.Framework.Assert, other.framework, 2.6.4.14350].AreEqual()";

        private const string NotNUnit =
            "static [System.Void, mscorlib, 4.0.0.0] [SomeProject.Assert, SomeProject, 2.6.4.14350].AreEqual()";

        private const string NotNUnit2 =
            "static [System.Void, mscorlib, 4.0.0.0] [SomeProject.Helpers, SomeProject, 2.6.4.14350].Assert()";

        private const string MoqMethod =
            "static [T] [Moq.Mock, Moq, 4.2.1507.118].Of`1[[T -> T]]()";

        private static ISST PrepareSST(string testMethod)
        {
            return new SST
            {
                Methods =
                {
                    new MethodDeclaration
                    {
                        Body =
                        {
                            new ExpressionStatement
                            {
                                Expression = new InvocationExpression
                                {
                                    MethodName =
                                        Names.Method(
                                            testMethod)
                                }
                            }
                        }
                    }
                }
            };
        }

        [Test, TestCase(NUnitTestMethod), TestCase(NUnitTestMethod2)]
        public void GuessDocumentType_Test(string methodName)
        {
            var docName = Names.Document("CSharp /TestProject/Test.cs");
            var sst = PrepareSST(methodName);

            var actual = TransformerUtils.GuessDocumentType(docName, sst);
            Assert.AreEqual(DocumentType.Test, actual);
        }

        [Test, TestCase(NotNUnit), TestCase(NotNUnit2)]
        public void GuessDocumentType_NotTest(string methodName)
        {
            var docName = Names.Document("CSharp /TestProject/Test.cs");
            var sst = PrepareSST(methodName);

            var actual = TransformerUtils.GuessDocumentType(docName, sst);
            Assert.AreNotEqual(DocumentType.Test, actual);
        }

        [Test, TestCase(MoqMethod)]
        public void GuessDocumentType_TestFramework(string methodName)
        {
            var docName = Names.Document("CSharp /TestProject/MockHelpers.cs");
            var sst = PrepareSST(methodName);

            var actual = TransformerUtils.GuessDocumentType(docName, sst);
            Assert.AreEqual(DocumentType.TestFramework, actual);
        }

        [Test]
        public void GuessDocumentType_FilenameTest()
        {
            var docName = Names.Document("CSharp /TestProject/Test.cs");
            var actual = TransformerUtils.GuessDocumentType(docName, new SST());
            Assert.AreEqual(DocumentType.FilenameTest, actual);
        }

        [Test]
        public void GuessDocumentType_PathnameTest()
        {
            var docName = Names.Document("CSharp /TestProject/SomeFile.cs");
            var actual = TransformerUtils.GuessDocumentType(docName, new SST());
            Assert.AreEqual(DocumentType.PathnameTest, actual);
        }

        [Test]
        public void GuessDocumentType_Production()
        {
            var docName = Names.Document("CSharp /Project/SomeFile.cs");
            var actual = TransformerUtils.GuessDocumentType(docName, new SST());
            Assert.AreEqual(DocumentType.Production, actual);
        }

        [Test]
        public void GuessDocumentType_Undefined()
        {
            var docName = Names.Document("NotCSharp /Project/SomeFile.abc");
            var actual = TransformerUtils.GuessDocumentType(docName, new SST());
            Assert.AreEqual(DocumentType.Undefined, actual);
        }
    }
}