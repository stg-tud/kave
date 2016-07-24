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

namespace KaVE.Commons.Tests.Model.Naming.Impl.v1
{
    [Ignore]
    public class CsNamespaceTest
    {
        [TestCase("a.b.c.", "a.b."),
         TestCase("a.b.", "a.")]
        public void ParentNamespace(string input, string expected)
        {
            Assert.AreEqual(Names.Namespace(input).ParentNamespace.Identifier, expected);
        }

        [TestCase("a.b.", "b"),
         TestCase("a.b.c.", "c")]
        public void Name(string input, string expected)
        {
            Assert.AreEqual(Names.Namespace(input).Name, expected);
        }
    }
}