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

using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.FeedbackProcessor.Intervals;
using KaVE.FeedbackProcessor.Intervals.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Intervals
{
    internal class TransformerUtilsTest
    {
        [Test]
        public void GuessDocumentType_Test()
        {
            var docName = DocumentName.Get("CSharp /TestProject/Test.cs");
            var sst = new SST
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
                                        MethodName.Get(
                                            "[System.Bool, mscorlib, 4.0.0.0] [TestFramework.Assert, TestFramework, 1.0.0.0].AreEqual()")
                                }
                            }
                        }
                    }
                }
            };

            var actual = TransformerUtils.GuessDocumentType(docName, sst);
            Assert.AreEqual(DocumentType.Test, actual);
        }

        [Test]
        public void GuessDocumentType_FilenameTest()
        {
            var docName = DocumentName.Get("CSharp /TestProject/Test.cs");
            var actual = TransformerUtils.GuessDocumentType(docName, new SST());
            Assert.AreEqual(DocumentType.FilenameTest, actual);
        }

        [Test]
        public void GuessDocumentType_PathnameTest()
        {
            var docName = DocumentName.Get("CSharp /TestProject/SomeFile.cs");
            var actual = TransformerUtils.GuessDocumentType(docName, new SST());
            Assert.AreEqual(DocumentType.PathnameTest, actual);
        }

        [Test]
        public void GuessDocumentType_Production()
        {
            var docName = DocumentName.Get("CSharp /Project/SomeFile.cs");
            var actual = TransformerUtils.GuessDocumentType(docName, new SST());
            Assert.AreEqual(DocumentType.Production, actual);
        }

        [Test]
        public void GuessDocumentType_Undefined()
        {
            var docName = DocumentName.Get("NotCSharp /Project/SomeFile.abc");
            var actual = TransformerUtils.GuessDocumentType(docName, new SST());
            Assert.AreEqual(DocumentType.Undefined, actual);
        }
    }
}