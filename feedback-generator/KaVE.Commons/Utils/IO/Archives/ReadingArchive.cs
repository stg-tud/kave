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
    public interface IReadingArchive : IDisposable
    {
        int Count { get; }
        bool HasNext();
        T GetNext<T>();
        IList<T> GetAll<T>();
    }

    public class ReadingArchive : IReadingArchive
    {
        public int Count { get; private set; }

        private readonly ZipInputStream _zipStream;
        private ZipEntry _nextEntry;
        private string _nextJson;

        public ReadingArchive(string zipPath)
        {
            Asserts.That(File.Exists(zipPath), "could not read zip '{0}'", zipPath);
            Count = 0;
            CountEntries(zipPath);

            _zipStream = new ZipInputStream(File.OpenRead(zipPath));
            FindNextUsableEntry();
        }

        private void CountEntries(string zipPath)
        {
            using (var s = new ZipInputStream(File.OpenRead(zipPath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsFile)
                    {
                        Count++;
                    }
                }
            }
        }

        private void FindNextUsableEntry()
        {
            ForwardStreamToNextFileEntry();
            while (_nextEntry != null && IsNullOrEmpty(_nextJson))
            {
                ForwardStreamToNextFileEntry();
            }
        }

        private void ForwardStreamToNextFileEntry()
        {
            while ((_nextEntry = _zipStream.GetNextEntry()) != null)
            {
                if (_nextEntry.IsFile)
                {
                    var data = new byte[4096];
                    var sb = new StringBuilder();
                    var size = _zipStream.Read(data, 0, data.Length);
                    while (size > 0)
                    {
                        sb.Append(Encoding.UTF8.GetString(data, 0, size));
                        size = _zipStream.Read(data, 0, data.Length);
                    }
                    _nextJson = sb.ToString();
                    return;
                }
            }
            _nextJson = null;
        }

        public bool HasNext()
        {
            return _nextJson != null;
        }

        private static bool IsNullOrEmpty(string json)
        {
            return string.IsNullOrEmpty(json) || "null".Equals(json);
        }

        public T GetNext<T>()
        {
            Asserts.That(HasNext());
            var json = _nextJson;
            FindNextUsableEntry();
            return json.ParseJsonTo<T>();
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

        public IEnumerable<T> GetAllLazy<T>()
        {
            while (HasNext())
            {
                yield return GetNext<T>();
            }
        }

        public void Dispose()
        {
            _zipStream.Dispose();
        }
    }
}