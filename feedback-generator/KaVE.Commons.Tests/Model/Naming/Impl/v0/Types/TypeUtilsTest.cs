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

using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
    internal class TypeUtilsTest
    {
        #region fixes

        [Test]
        public void FixesLegacyDelegateTypeNameFormat()
        {
            var actual = "d:Some.DelegateType, A, 1.0.0.0".FixLegacyFormats();
            var expected = "d:[?] [Some.DelegateType, A, 1.0.0.0].()";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MissingGenericTicksAreAdded()
        {
            var actual = "n.C1`1[[T1]]+C2[[T2]]+C3[[T3]], P".FixLegacyFormats();
            var expected = "n.C1`1[[T1]]+C2`1[[T2]]+C3`1[[T3]], P";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MissingGenericTicksAreAdded_Multiple()
        {
            var actual = "n.C1`1[[T1]]+C2[[T2],[T3]]+C3[[T3]], P";
            var expected = "n.C1`1[[T1]]+C2`2[[T2],[T3]]+C3`1[[T3]], P";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MissingGenericTicksAreAdded_WithWhitespace()
        {
            var actual = "n.C1`1[[T1]]+C2[[T2] , [T3] ]+C3[[T3]], P";
            var expected = "n.C1`1[[T1]]+C2`2[[T2] , [T3] ]+C3`1[[T3]], P";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MissingGenericTicksAreAdded_Array()
        {
            var actual = "N.C1`1[[T1]]+C2[][[T2]],P";
            var expected = "N.C1`1[[T1]]+C2`1[][[T2]],P";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MissingGenericTicksAreAdded_Array2D()
        {
            var actual = "N.C1`1[[T1]]+C2[,][[T2]],P";
            var expected = "N.C1`1[[T1]]+C2`1[,][[T2]],P";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MissingGenericTicksAreAdded_Array3D()
        {
            var actual = "N.C1`1[[T1]]+C2[,,][[T2]],P";
            var expected = "N.C1`1[[T1]]+C2`1[,,][[T2]],P";
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region is xy tests

        [TestCase("n.C1`1[][[T1]],P"), TestCase("n.C1+C2[],P"), TestCase("n.C1`1[[T1]]+C2[],P"),
         TestCase("n.C1+C2`1[][[T1]],P")]
        public void IsArray(string typeId)
        {
            Assert.IsTrue(ArrayTypeName.IsArrayTypeNameIdentifier(typeId));
        }

        [TestCase("n.C1`1[[T1->T[],P]],P")]
        public void IsNoArray(string typeId)
        {
            Assert.IsTrue(ArrayTypeName.IsArrayTypeNameIdentifier(typeId));
        }

        [TestCase("d:[?] [?].()")]
        public void IsDelegate(string typeId)
        {
            Assert.IsTrue(ArrayTypeName.IsArrayTypeNameIdentifier(typeId));
        }

        [TestCase("?"), TestCase("d:[?] [?].()[]"), TestCase("d:[?] [?].()[,]")]
        public void IsNoDelegate(string typeId)
        {
            Assert.IsTrue(ArrayTypeName.IsArrayTypeNameIdentifier(typeId));
        }

       

        #endregion

        #region create type tests

        #endregion
    }
}