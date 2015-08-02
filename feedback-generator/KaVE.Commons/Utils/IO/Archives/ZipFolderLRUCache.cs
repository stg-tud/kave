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
using System.Text.RegularExpressions;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.Utils.IO.Archives
{
    public class ZipFolderLRUCache<T> : IDisposable where T : class
    {
        private readonly string _root;
        private readonly int _capacity;

        private readonly List<T> _accessOrderList = new List<T>();
        private readonly Dictionary<T, IWritingArchive> _openArchives = new Dictionary<T, IWritingArchive>();
        private readonly Dictionary<T, ZipFolder> _folders = new Dictionary<T, ZipFolder>();

        public ZipFolderLRUCache(string root, int capacity)
        {
            Asserts.That(Directory.Exists(root));
            Asserts.That(capacity > 0);

            _root = root;
            _capacity = capacity;
        }

        public IWritingArchive GetArchive(T key)
        {
            RefreshAccess(key);

            if (_openArchives.ContainsKey(key))
            {
                return _openArchives[key];
            }

            var folder = _folders.ContainsKey(key)
                ? _folders[key]
                : GetFolder(key);

            var wa = folder.CreateNewArchive();
            _openArchives.Add(key, wa);

            EnsureCapacityIsRespected();

            return wa;
        }

        private void EnsureCapacityIsRespected()
        {
            if (_accessOrderList.Count > _capacity)
            {
                var oldest = _accessOrderList.First();
                _accessOrderList.Remove(oldest);
                _openArchives[oldest].Dispose();
                _openArchives.Remove(oldest);
            }
        }

        private void RefreshAccess(T key)
        {
            _accessOrderList.Remove(key);
            _accessOrderList.Add(key);
        }

        private ZipFolder GetFolder(T key)
        {
            if (_folders.ContainsKey(key))
            {
                return _folders[key];
            }

            var folderName = GetTargetFolder(key);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            var folderUtil = new ZipFolder(folderName, key.ToFormattedJson());

            _folders[key] = folderUtil;

            return folderUtil;
        }

        private string GetTargetFolder(T key)
        {
            var regex = new Regex(@"[^a-zA-Z0-9,\-_/+$(){}[\]]");

            var relName = key.ToCompactJson();
            relName = relName.Replace('.', '/');
            relName = relName.Replace("\\\"", "\""); // quotes inside json
            relName = relName.Replace("\"", ""); // surrounding quotes
            relName = relName.Replace('\\', '/');
            relName = regex.Replace(relName, "_");

            return Path.Combine(_root, relName);
        }

        public bool IsCached(T key)
        {
            return _openArchives.ContainsKey(key);
        }

        public int Size
        {
            get { return _openArchives.Count; }
        }

        public void Dispose()
        {
            foreach (var wa in _openArchives.Values)
            {
                wa.Dispose();
            }
            _accessOrderList.Clear();
            _openArchives.Clear();
            _folders.Clear();
        }
    }
}