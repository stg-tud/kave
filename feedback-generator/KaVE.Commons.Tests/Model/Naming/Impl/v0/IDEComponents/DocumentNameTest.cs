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

using KaVE.Commons.Model.Naming.Impl.v0.IDEComponents;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.IDEComponents
{
    internal class DocumentNameTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new DocumentName();
            Assert.AreEqual("???", sut.Language);
            Assert.AreEqual("???", sut.FileName);
            Assert.IsTrue(sut.IsUnknown);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.True(new DocumentName().IsUnknown);
            Assert.True(new DocumentName("???").IsUnknown);
            Assert.False(new DocumentName("C# f.cs").IsUnknown);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldAvoidNullParameters()
        {
            // ReSharper disable once ObjectCreationAsStatement
            // ReSharper disable once AssignNullToNotNullAttribute
            new DocumentName(null);
        }

        [TestCase("CSharp C:\\File.cs", "CSharp", "C:\\File.cs"),
         TestCase(" \\File.ext", "", "\\File.ext"),
         TestCase("Basic Code.vb", "Basic", "Code.vb"),
         TestCase("C/C++ Code.c", "C/C++", "Code.c"),
         TestCase("Plain Text Readme.txt", "Plain Text", "Readme.txt"),
         TestCase("Plain Text Path With Spaces\\Readme.txt", "Plain Text", "Path With Spaces\\Readme.txt")]
        public void ParsesName(string identifier, string language, string fileName)
        {
            var uut = new DocumentName(identifier);
            Assert.AreEqual(language, uut.Language);
            Assert.AreEqual(fileName, uut.FileName);
        }

        [Test, ExpectedException(typeof(AssertException))]
        public void ShouldRejectNameWithoutSpaces()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new DocumentName("C:\\No\\Type\\Only\\File.cs");
        }
    }
}