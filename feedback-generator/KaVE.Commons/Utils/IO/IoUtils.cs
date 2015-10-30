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
using System.Net.Http;
using Ionic.Zip;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.Commons.Utils.IO
{
    public class IoUtils : IIoUtils
    {
        public void CopyFile(string src, string trg)
        {
            File.Copy(src, trg, true);
        }

        public virtual HttpResponseMessage TransferByHttp(HttpContent content, Uri targetUri)
        {
            // TODO @seb: currently only implemented in injectable version... get rid of the error messages and change that
            throw new NotImplementedException();
        }

        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        public string GetTempFileName(string extension)
        {
            var random = new Random();
            var temp = Path.GetTempPath();
            string file;
            do
            {
                file = Path.Combine(temp, "file-" + random.Next() + "." + extension);
            } while (File.Exists(file));
            CreateFile(file);
            return file;
        }

        public string GetTempDirectoryName()
        {
            var random = new Random();
            var temp = Path.GetTempPath();
            string dir;
            do
            {
                dir = Path.Combine(temp, "dir-" + random.Next());
            } while (Directory.Exists(dir));
            Directory.CreateDirectory(dir);
            return dir;
        }

        public long GetFileSize(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            return fileInfo.Length;
        }

        public IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateFiles(path);
        }

        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void WriteAllByte(byte[] bytes, string fileName)
        {
            File.WriteAllBytes(fileName, bytes);
        }

        public string ReadFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        public void DeleteDirectoryWithContent(string path)
        {
            Directory.Delete(path, true);
        }

        public Stream OpenFile(string file, FileMode mode, FileAccess access)
        {
            return new FileStream(file, mode, access);
        }

        public void MoveFile(string source, string target)
        {
            File.Move(source, target);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void CreateFile(string path)
        {
            CreateDirectory(Directory.GetParent(path).FullName);
            File.Create(path).Close();
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public string[] GetFilesRecursive(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
        }

        public string UnzipToTempFolder(string zipFile)
        {
            var tmp = Path.GetTempPath();
            var outFolder = Path.Combine(tmp, Path.GetRandomFileName());
            using (var zf = ZipFile.Read(zipFile))
            {
                zf.ExtractAll(outFolder);
            }
            return outFolder;
        }

        public IReadingArchive ReadArchive(string fileName)
        {
            Asserts.That(File.Exists(fileName));
            return new ReadingArchive(fileName);
        }

        public IWritingArchive CreateArchive(string fileName)
        {
            Asserts.Not(File.Exists(fileName));
            Asserts.That(Directory.GetParent(fileName).Exists);
            return new WritingArchive(fileName);
        }

        public int CountLines(string fileName)
        {
            return File.ReadLines(fileName).Count();
        }
    }
}