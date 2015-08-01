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
using KaVE.Commons.Utils.IO;

namespace KaVE.RS.SolutionAnalysis
{
    public class TypeZipLRUCache<K> : IDisposable where K : class
    {
        private readonly string _root;
        private readonly int _capacity;

        private readonly List<K> _accessOrderList = new List<K>();
        private readonly Dictionary<K, IWritingArchive> _openArchives = new Dictionary<K, IWritingArchive>();
        private readonly Dictionary<K, TypeZipFolder> _folders = new Dictionary<K, TypeZipFolder>();


        public TypeZipLRUCache(string root, int capacity)
        {
            Asserts.That(Directory.Exists(root));
            Asserts.That(capacity > 0);

            _root = root;
            _capacity = capacity;
        }

        public IWritingArchive GetArchive(K type)
        {
            RefreshAccess(type);

            if (_openArchives.ContainsKey(type))
            {
                return _openArchives[type];
            }

            var folder = _folders.ContainsKey(type)
                ? _folders[type]
                : GetFolder(type);

            var wa = folder.CreateNewArchive();
            _openArchives.Add(type, wa);

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

        private void RefreshAccess(K type)
        {
            _accessOrderList.Remove(type);
            _accessOrderList.Add(type);
        }

        private TypeZipFolder GetFolder(K type)
        {
            if (_folders.ContainsKey(type))
            {
                return _folders[type];
            }

            var folderName = GetTargetFolder(type);
            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }
            var folderUtil = new TypeZipFolder(folderName);

            _folders[type] = folderUtil;

            return folderUtil;
        }

        private string GetTargetFolder(K type)
        {
            var regex = new Regex(@"[^a-zA-Z0-9._]");
            var relativeFolderName = type.ToString();
            relativeFolderName = regex.Replace(relativeFolderName, "_");
            return Path.Combine(_root, relativeFolderName);
        }

        public bool IsCached(K type)
        {
            return _openArchives.ContainsKey(type);
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