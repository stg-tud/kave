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

using System.IO;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Preprocessing.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Preprocessing
{
    internal abstract class FileBasedPreprocessingTestBase : FileBasedTestBase
    {
        protected string RawDir;
        protected string MergedDir;
        protected string FinalDir;

        protected PreprocessingIo Io;

        [SetUp]
        public void BaseSetup()
        {
            RawDir = MkDir("raw");
            MergedDir = MkDir("merged");
            FinalDir = MkDir("final");

            Io = new PreprocessingIo(RawDir, MergedDir, FinalDir);
        }

        private string MkDir(string dirName)
        {
            var dir = Path.Combine(DirTestRoot, dirName);
            Directory.CreateDirectory(dir);
            return dir;
        }

        protected IKaVEList<T> Read<T>(string fileName)
        {
            return ReadPlain<IKaVEList<T>>(fileName);
        }
    }
}