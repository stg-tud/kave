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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp
{
    [TestFixture]
    internal class LambdaNameTest
    {
        [Test]
        public void ReturnType()
        {
            var name = LambdaName.Get("[System.String, mscorlib, 4.0.0.0] ()");

            Assert.AreEqual(TypeName.Get("System.String, mscorlib, 4.0.0.0"), name.ReturnType);
        }

        [Test]
        public void WithoutParameters()
        {
            var name = LambdaName.Get("[System.String, mscorlib, 4.0.0.0] ()");

            Assert.False(name.HasParameters);
            CollectionAssert.IsEmpty(name.Parameters);
            Assert.AreEqual("()", name.Signature);
        }

        [Test]
        public void WithParameters()
        {
            var name = LambdaName.Get("[System.String, mscorlib, 4.0.0.0] ([C, A] p1, [C, B] p2)");

            Assert.True(name.HasParameters);
            CollectionAssert.AreEqual(new[] { ParameterName.Get("[C, A] p1"), ParameterName.Get("[C, B] p2") }, name.Parameters);
            Assert.AreEqual("([C, A] p1, [C, B] p2)", name.Signature);
        }

        [Test]
        public void StringHelperWorks()
        {
            const string id = "[{0}] ()";
            const string t1 = "T1,P";
            var a = LambdaName.Get(string.Format(id, t1));
            var b = LambdaName.Get(id, t1);

            Assert.AreEqual(a, b);
            Assert.AreSame(a, b);
        }
    }
}