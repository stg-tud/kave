/*
 * Copyright 2017 Sebastian Proksch
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
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate
{
    internal class AssemblyCounterTest
    {
        private AssemblyCounter _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new AssemblyCounter();
        }

        [Test]
        public void Simple()
        {
            Count(A(1));

            AssertRes(new[] {1}, new[] {1});
        }

        [Test]
        public void Multiple()
        {
            Count(A(1), A(2));

            AssertRes(new[] {1, 2}, new[] {1, 1});
        }

        [Test]
        public void MultipleSln()
        {
            Count(A(1));
            Count(A(2));

            AssertRes(new[] {1, 2}, new[] {1, 1});
        }

        [Test]
        public void Overlap()
        {
            Count(A(1), A(2));
            Count(A(2), A(3));

            AssertRes(new[] {1, 2, 3}, new[] {1, 2, 1});
        }

        private void Count(params IAssemblyName[] assemblies)
        {
            _sut.Count(Sets.NewHashSetFrom(assemblies));
        }

        private void AssertRes(int[] assemblyNos, int[] counts)
        {
            var res = _sut.Counts;
            Assert.AreEqual(assemblyNos.Length, counts.Length);
            Assert.AreEqual(assemblyNos.Length, res.Keys.Count);

            for (var i = 0; i < assemblyNos.Length; i++)
            {
                var expAsm = A(assemblyNos[i]);
                var expCount = counts[i];

                Assert.That(res.ContainsKey(expAsm));
                Assert.AreEqual(expCount, res[expAsm]);
            }
        }

        private IAssemblyName A(int i)
        {
            return Names.Assembly("A{0}, 1.2.3.4", i);
        }
    }
}