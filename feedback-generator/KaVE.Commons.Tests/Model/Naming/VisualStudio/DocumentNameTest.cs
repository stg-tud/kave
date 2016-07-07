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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.VisualStudio
{
    internal class DocumentNameTest
    {
        [TestCase("CSharp C:\\File.cs", "CSharp", "C:\\File.cs"),
         TestCase(" \\File.ext", "", "\\File.ext"),
         TestCase("Basic Code.vb", "Basic", "Code.vb"),
         TestCase("C/C++ Code.c", "C/C++", "Code.c"),
         TestCase("Plain Text Readme.txt", "Plain Text", "Readme.txt")]
        public void ParsesName(string identifier, string language, string fileName)
        {
            var uut = Names.Document(identifier);

            Assert.AreEqual(language, uut.Language);
            Assert.AreEqual(fileName, uut.FileName);
        }
    }
}