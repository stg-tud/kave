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
using Ionic.Zip;
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

            using (var zipFile = new ZipFile())
            {
                zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;
                var i = 0;
                foreach (var entry in _entries)
                {
                    var fileName = (i++) + ".json";
                    zipFile.AddEntry(fileName, entry);
                }
                zipFile.Save(_path);
            }
        }
    }
}