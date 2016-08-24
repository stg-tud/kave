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

namespace KaVE.Commons.Utils.IO.Archives
{
    public interface IWritingArchive : IDisposable
    {
        int NumItemsAdded { get; }
        void Add<T>(T obj);
        void AddAll<T>(IEnumerable<T> entries);
    }

    public class WritingArchive : IWritingArchive
    {
        private readonly string _path;
        private ZipOutputStream _zos;

        public int NumItemsAdded { get; private set; }

        public WritingArchive(string path)
        {
            AssertParentExist(path);
            _path = path;
            NumItemsAdded = 0;
        }

        private static void AssertParentExist(string path)
        {
            const string err = "cannot create '{0}', parent does not exist";
            var parent = Path.GetDirectoryName(path);
            Asserts.NotNull(parent, err, path);
            Asserts.That(Directory.Exists(parent), err, path);
        }


        public void Add<T>(T obj)
        {
            if (obj != null)
            {
                InitZipIfRequired();

                var json = obj.ToCompactJson();
                var bytes = Encoding.UTF8.GetBytes(json);
                var fileName = "{0}.json".FormatEx(NumItemsAdded++);

                _zos.PutNextEntry(new ZipEntry(fileName) {Size = bytes.Length});
                _zos.Write(bytes, 0, bytes.Length);
                _zos.Flush();
            }
        }

        public void AddAll<T>(IEnumerable<T> entries)
        {
            foreach (var entry in entries)
            {
                Add(entry);
            }
        }

        private void InitZipIfRequired()
        {
            if (_zos == null)
            {
                _zos = new ZipOutputStream(File.Create(_path))
                {
                    UseZip64 = UseZip64.Dynamic
                };
                _zos.SetLevel(9); // 0 - store only to 9 - means best compression
            }
        }

        public void Dispose()
        {
            if (_zos != null)
            {
                _zos.Finish();
                _zos.Dispose();
                _zos = null;
            }
        }
    }
}