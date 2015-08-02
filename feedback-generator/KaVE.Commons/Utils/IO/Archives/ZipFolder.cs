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
using System.Linq;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.IO.Archives
{
    public class ZipFolder
    {
        public const string MarkerFileName = ".zipfolder";

        private readonly string _root;
        private readonly string _metaData;

        private int _fileCounter;

        public ZipFolder(string root, string metaData = "")
        {
            Asserts.That(Directory.Exists(root));
            _root = root;
            _metaData = metaData;
        }

        public IWritingArchive CreateNewArchive()
        {
            CreateMarker();
            var fileName = CreateNextUnusedFileName();
            return new WritingArchive(fileName);
        }

        private void CreateMarker()
        {
            var markerName = Path.Combine(_root, MarkerFileName);
            if (!File.Exists(markerName))
            {
                File.WriteAllText(markerName, _metaData);
            }
        }

        private string CreateNextUnusedFileName()
        {
            var fileName = CreateNextFileName();
            while (File.Exists(fileName))
            {
                fileName = CreateNextFileName();
            }
            return fileName;
        }

        private string CreateNextFileName()
        {
            return Path.Combine(_root, string.Format("{0}.zip", _fileCounter++));
        }

        public int NumArchives()
        {
            return Directory.EnumerateFiles(_root, "*.zip").Count();
        }

        public IList<IReadingArchive> OpenAll()
        {
            return Directory.EnumerateFiles(_root, "*.zip").Select(Open).ToList();
        }

        public IReadingArchive Open(string path)
        {
            return new ReadingArchive(path);
        }
    }
}