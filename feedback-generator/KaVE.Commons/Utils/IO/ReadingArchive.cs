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
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.Utils.IO
{
    public interface IReadingArchive : IDisposable
    {
        int Count { get; }
        bool HasNext();

        T GetNext<T>();
    }

    public class ReadingArchive : IReadingArchive
    {
        private readonly string _tempDir;
        private readonly string[] _files;
        private int _currentIndex;

        public ReadingArchive(string zipPath)
        {
            Asserts.That(File.Exists(zipPath));
            _tempDir = GetTemporaryDirectory();
            using (var zipFile = ZipFile.Read(zipPath))
            {
                zipFile.ExtractAll(_tempDir);
            }
            _files = Directory.EnumerateFiles(_tempDir, "*.json").ToArray();
        }

        public int Count
        {
            get { return _files.Length; }
        }

        public bool HasNext()
        {
            return _currentIndex < _files.Length;
        }

        public T GetNext<T>()
        {
            var json = File.ReadAllText(_files[_currentIndex++]);
            var obj = json.ParseJsonTo<T>();
            return obj;
        }

        public IList<T> GetAll<T>()
        {
            var all = new List<T>();
            while (HasNext())
            {
                all.Add(GetNext<T>());
            }
            return all;
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, true);
            }
        }

        private static string GetTemporaryDirectory()
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
    }
}