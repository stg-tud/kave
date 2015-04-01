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

using KaVE.Commons.Model.Names.VisualStudio;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.VisualStudio
{
    [TestFixture]
    class DocumentNameTest
    {
        [Test]
        public void ShouldParseLanguage()
        {
            var uut = DocumentName.Get("CSharp C:\\File.cs");

            Assert.AreEqual("CSharp", uut.Language);
        }

        [Test]
        public void ShouldParseFileName()
        {
            var uut = DocumentName.Get("Language C:\\File.ext");

            Assert.AreEqual("C:\\File.ext", uut.FileName);
        }
    }
}
