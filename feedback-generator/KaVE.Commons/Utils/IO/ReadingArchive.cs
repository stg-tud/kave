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
    public interface IReadingArchive
    {
        int Count { get; }
        bool HasNext();
        T GetNext<T>();
        IList<T> GetAll<T>();
    }

    public class ReadingArchive : IReadingArchive
    {
        private List<string>.Enumerator _eventEnumerator;

        public ReadingArchive(string zipPath)
        {
            Count = 0;
            Asserts.That(File.Exists(zipPath));

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

            var events = new List<string>();
            var data = new byte[4096];
            using (var s = new ZipInputStream(File.OpenRead(zipPath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsFile)
                    {
                        var sb = new StringBuilder();
                        var size = s.Read(data, 0, data.Length);
                        while (size > 0)
                        {
                            sb.Append(Encoding.Unicode.GetString(data, 0, size));
                            size = s.Read(data, 0, data.Length);
                        }
                        events.Add(sb.ToString());
                    }
                }
            }

            _eventEnumerator = events.GetEnumerator();
        }

        public int Count { get; private set; }

        public bool HasNext()
        {
            return _eventEnumerator.MoveNext();
        }

        public T GetNext<T>()
        {
            if (_eventEnumerator.Current != null)
            {
                var obj = _eventEnumerator.Current.ParseJsonTo<T>();
                return obj;
            }
            throw new ArgumentOutOfRangeException();
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
    }
}