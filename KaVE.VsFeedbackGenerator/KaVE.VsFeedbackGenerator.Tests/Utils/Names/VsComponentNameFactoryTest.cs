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

using EnvDTE;
using KaVE.VsFeedbackGenerator.Tests.Assertion;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using KaVE.VsFeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Names
{
    [TestFixture]
    class VsComponentNameFactoryTest
    {
        [Test]
        public void ShouldConvertSolutionDocumentToRelativeName()
        {
            var documentMock = new Mock<Document>();
            documentMock.Setup(doc => doc.Language).Returns("Project");
            documentMock.Setup(doc => doc.FullName).Returns(@"C:\Users\Sven Amann\Documents\Projects\KaVE-Solution\Project\Project.csproj");
            documentMock.Setup(doc => doc.DTE).Returns(DTEMockUtils.MockSolution(@"C:\Users\Sven Amann\Documents\Projects\KaVE-Solution\Solution.sln").DTE);

            var documentName = documentMock.Object.GetName();

            NameAssert.AreEqual(@"Project \Project\Project.csproj", documentName);
        }

        [Test]
        public void ShouldConvertDocumentToFileNameIfNoSolutionIsOpen()
        {
            var documentMock = new Mock<Document>();
            documentMock.Setup(doc => doc.Language).Returns("CSharp");
            documentMock.Setup(doc => doc.FullName).Returns(@"C:\Users\Sven Amann\Documents\External\Reference\GoldenHammer.cs");
            documentMock.Setup(doc => doc.DTE).Returns(new Mock<DTE>().Object);

            var documentName = documentMock.Object.GetName();

            NameAssert.AreEqual(@"CSharp GoldenHammer.cs", documentName);
        }

        [Test]
        public void ShouldConvertDocumentToFileNameIfItIsSolutionExternal()
        {
            var documentMock = new Mock<Document>();
            documentMock.Setup(doc => doc.Language).Returns("Properties");
            documentMock.Setup(doc => doc.FullName).Returns(@"D:\My\Private\Stuff.Properties");
            documentMock.Setup(doc => doc.DTE).Returns(DTEMockUtils.MockSolution(@"C:\Users\Sven Amann\Documents\Projects\KaVE-Solution\Solution.sln").DTE);

            var documentName = documentMock.Object.GetName();

            NameAssert.AreEqual(@"Properties Stuff.Properties", documentName);
        }

        [Test]
        public void ShouldConvertNullDocumentToNull()
        {
            var documentName = ((Document)null).GetName();
            Assert.IsNull(documentName);
        }

        // TODO test for other factory methods
    }
}
