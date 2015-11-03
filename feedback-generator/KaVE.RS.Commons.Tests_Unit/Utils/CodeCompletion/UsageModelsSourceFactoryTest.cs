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

using System;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.Commons.Utils.CodeCompletion.Impl.Stores.UsageModelSources;
using KaVE.Commons.Utils.IO;
using KaVE.RS.Commons.Utils.CodeCompletion;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Utils.CodeCompletion
{
    internal class UsageModelsSourceFactoryTest
    {
        #region helper

        private static Uri FileTestSource
        {
            get { return new Uri("C:\\"); }
        }

        private static Uri HttpTestSource
        {
            get { return new Uri("http://www.google.de"); }
        }

        private static Uri FtpTestSource
        {
            get { return new Uri("ftp://someurl"); }
        }

        private static IoUtils TestIoUtils
        {
            get { return new IoUtils(); }
        }

        private static TypePathUtil TestTypePathUtil
        {
            get { return new TypePathUtil(); }
        }

        #endregion

        private UsageModelsSourceFactory _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new UsageModelsSourceFactory(TestIoUtils, TestTypePathUtil);
        }

        [Test]
        public void ReturnsFileSourceForFilePaths()
        {
            Assert.IsInstanceOf<FilePathUsageModelsSource>(
                _uut.GetSource(FileTestSource));
        }

        [Test]
        public void SetsFileSource()
        {
            Assert.AreEqual(
                FileTestSource,
                _uut.GetSource(FileTestSource).Source);
        }

        [Test]
        public void ReturnsHttpSourceForUrls()
        {
            Assert.IsInstanceOf<HttpUsageModelsSource>(
                _uut.GetSource(HttpTestSource));
        }

        [Test]
        public void SetsHttpSource()
        {
            Assert.AreEqual(
                HttpTestSource,
                _uut.GetSource(HttpTestSource).Source);
        }

        [Test]
        public void ReturnsEmptySourceForOtherUris()
        {
            var actualSource = _uut.GetSource(FtpTestSource);
            Assert.IsInstanceOf<EmptyUsageModelsSource>(actualSource);
            Assert.AreEqual(FtpTestSource, actualSource.Source);
        }
    }
}