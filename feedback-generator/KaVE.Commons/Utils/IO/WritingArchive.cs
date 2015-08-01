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
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.Utils.IO
{
    public interface IWritingArchive : IDisposable
    {
        void Add<T>(T obj);
    }

    public class WritingArchive : IWritingArchive
    {
        private readonly string _path;
        private readonly List<string> _entries = new List<string>();

        public WritingArchive(string path)
        {
            var parent = Path.GetDirectoryName(path);
            Asserts.NotNull(parent);
            Asserts.That(Directory.Exists(parent));
            _path = path;
        }

        public void Add<T>(T obj)
        {
            _entries.Add(obj.ToFormattedJson());
        }

        public void AddAll<T>(IEnumerable<T> entries)
        {
            foreach (var entry in entries)
            {
                Add(entry);
            }
        }

        public void Dispose()
        {
            if (_entries.Count == 0)
            {
                return;
            }

            var num = 0;
            using (var s = new ZipOutputStream(File.Create(_path)))
            {
                s.UseZip64 = UseZip64.Dynamic;
                s.SetLevel(9); // 0 - store only to 9 - means best compression

                foreach (var entry in _entries)
                {
                    var fileName = string.Format("{0}.json", num++);
                    s.PutNextEntry(new ZipEntry(fileName));

                    var bytes = Encoding.Unicode.GetBytes(entry);
                    s.Write(bytes, 0, bytes.Length);
                }
                s.Finish();
                s.Close();
            }

            _entries.Clear();
        }
    }
}