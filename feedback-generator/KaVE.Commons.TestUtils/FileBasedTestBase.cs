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
using System.IO;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.TestUtils
{
    public class FileBasedTestBase
    {
        protected string DirTestRoot;

        [SetUp]
        public void FileBasedTestBaseSetup()
        {
            DirTestRoot = CreateTempDir();
        }

        private static string CreateTempDir()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(path);
            return path;
        }

        [TearDown]
        public void FileBasedTestBaseTeardown()
        {
            if (Directory.Exists(DirTestRoot))
            {
                Directory.Delete(DirTestRoot, true);
            }
        }

        protected void WritePlain<T>(string file, params T[] ts)
        {
            Assert.IsTrue(file.StartsWith(DirTestRoot));
            EnsureParentExists(file);

            var json = ts.ToFormattedJson();
            File.WriteAllText(file, json);
        }

        protected void WriteZip<T>(string zip, params T[] ts)
        {
            Assert.IsTrue(zip.StartsWith(DirTestRoot));
            EnsureParentExists(zip);
            using (var wa = new WritingArchive(zip))
            {
                wa.AddAll(ts);
            }
        }

        protected T ReadPlain<T>(string file)
        {
            Assert.IsTrue(file.StartsWith(DirTestRoot));
            var json = File.ReadAllText(file);
            return json.ParseJsonTo<T>();
        }

        protected IList<T> ReadZip<T>(string file)
        {
            Assert.IsTrue(file.StartsWith(DirTestRoot));
            using (var ra = new ReadingArchive(file))
            {
                return ra.GetAll<T>();
            }
        }

        private static void EnsureParentExists(string file)
        {
            var parent = Path.GetDirectoryName(file);
            Asserts.NotNull(parent);
            Directory.CreateDirectory(parent);
        }
    }
}