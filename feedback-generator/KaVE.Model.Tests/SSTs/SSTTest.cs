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
 *    - Sebastian Proksch
 */

using KaVE.Model.Collections;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Declarations;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs
{
    internal class SSTTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new SST();

            Assert.NotNull(sut.Delegates);
            Assert.NotNull(sut.Events);
            Assert.NotNull(sut.Fields);
            Assert.NotNull(sut.Methods);
            Assert.NotNull(sut.Properties);
            Assert.NotNull(sut.EntryPoints);
            Assert.NotNull(sut.NonEntryPoints);

            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new SST {EnclosingType = TypeName.UnknownName};
            sut.Delegates.Add(new DelegateDeclaration());
            sut.Events.Add(new EventDeclaration());
            sut.Fields.Add(new FieldDeclaration());
            sut.Methods.Add(new MethodDeclaration());
            sut.Properties.Add(new PropertyDeclaration());

            Assert.AreEqual(TypeName.UnknownName, sut.EnclosingType);
            Assert.AreEqual(Lists.NewList(new DelegateDeclaration()), sut.Delegates);
            Assert.AreEqual(Lists.NewList(new EventDeclaration()), sut.Events);
            Assert.AreEqual(Lists.NewList(new FieldDeclaration()), sut.Fields);
            Assert.AreEqual(Lists.NewList(new MethodDeclaration()), sut.Methods);
            Assert.AreEqual(Lists.NewList(new PropertyDeclaration()), sut.Properties);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new SST();
            var b = new SST();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new SST();
            var b = new SST();
            a.EnclosingType = TypeName.UnknownName;
            b.EnclosingType = TypeName.UnknownName;
            a.Delegates.Add(new DelegateDeclaration());
            b.Delegates.Add(new DelegateDeclaration());
            a.Events.Add(new EventDeclaration());
            b.Events.Add(new EventDeclaration());
            a.Fields.Add(new FieldDeclaration());
            b.Fields.Add(new FieldDeclaration());
            a.Methods.Add(new MethodDeclaration());
            b.Methods.Add(new MethodDeclaration());
            a.Properties.Add(new PropertyDeclaration());
            b.Properties.Add(new PropertyDeclaration());

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentX()
        {
            Assert.Fail();
        }
    }
}