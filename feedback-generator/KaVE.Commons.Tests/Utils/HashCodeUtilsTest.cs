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

using System.Collections.Generic;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    internal class HashCodeUtilsTest
    {
        private const int Seed = 123;

        [Test]
        public void Dictionary_EmptyReturnsSeed()
        {
            var a = new Dictionary<int, int>();
            Assert.IsTrue(HashCodeUtils.For(Seed, a) == Seed);
        }

        [Test]
        public void Dictionary_EqualContents()
        {
            var a = D(1, 2);
            var b = D(1, 2);
            Assert.IsTrue(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void Dictionary_UnequalContents()
        {
            var a = D(1, 2);
            var b = D(3, 4);
            Assert.IsFalse(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void DictionarySet_EmptyReturnsSeed()
        {
            var a = new Dictionary<int, IEnumerable<int>>();
            Assert.IsTrue(HashCodeUtils.For(Seed, a) == Seed);
        }

        [Test]
        public void DictionarySet_EqualContents()
        {
            var a = Ds(1, L());
            var b = Ds(1, L());
            Assert.IsTrue(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void DictionarySet_EqualLists()
        {
            var a = Ds(1, L(2, 3));
            var b = Ds(1, L(2, 3));
            Assert.IsTrue(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void DictionarySet_UnequalLists()
        {
            var a = Ds(1, L(2, 3));
            var b = Ds(2, L(2, 3));
            Assert.IsFalse(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void DictionarySet_UnequalLists2()
        {
            var a = Ds(1, L(2, 3));
            var b = Ds(1, L(4, 3));
            Assert.IsFalse(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void DictionarySet_EqualSets()
        {
            var a = Ds(1, S(2, 3));
            var b = Ds(1, S(2, 3));
            Assert.IsTrue(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void DictionarySet_UnequalSets()
        {
            var a = Ds(1, S(2, 3));
            var b = Ds(2, S(2, 3));
            Assert.IsFalse(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void DictionarySet_UnequalSets2()
        {
            var a = Ds(1, S(2, 3));
            var b = Ds(1, S(4, 3));
            Assert.IsFalse(HashCodeUtils.For(Seed, a) == HashCodeUtils.For(Seed, b));
        }

        [Test]
        public void DictionarySet_UnequalSets3_AdaptationOfContextBug()
        {
            var a = new Dictionary<string, ISet<IMethodName>> {{"A", new HashSet<IMethodName> {M("C")}}};
            var b = new Dictionary<string, ISet<IMethodName>> {{"A", new HashSet<IMethodName> {M("C")}}};

            Assert.AreEqual(a, b);
            Assert.IsTrue(HashCodeUtils.For(1, a) == HashCodeUtils.For(1, b));
        }

        private static Dictionary<int, int> D(int k, int v)
        {
            return new Dictionary<int, int> {{k, v}};
        }

        private static Dictionary<int, IEnumerable<int>> Ds(int k, IEnumerable<int> vs)
        {
            return new Dictionary<int, IEnumerable<int>> {{k, vs}};
        }

        public static IMethodName M(string methodName)
        {
            return
                Names.Method(
                    "[System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0]." + methodName +
                    "(opt [System.Int32, mscore, 4.0.0.0] length)");
        }

        private static IEnumerable<int> L(params int[] args)
        {
            var r = new List<int>();
            r.AddRange(args);
            return r;
        }

        private static IEnumerable<int> S(params int[] args)
        {
            var r = new HashSet<int>();
            foreach (var a in args)
            {
                r.Add(a);
            }
            return r;
        }
    }
}