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
using System.Net.Http;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.IO
{
    public interface IIoUtils
    {
        HttpResponseMessage TransferByHttp([NotNull] HttpContent content, [NotNull] Uri targetUri);

        string GetTempFileName();
        string GetTempFileName(string extension);
        string[] GetFiles(string path, string searchPattern);
        string[] GetFilesRecursive(string path, string searchPattern);
        long GetFileSize(string fileName);

        /// <summary>
        ///     Creates or overrides a file. Creates non-existing parent directories.
        /// </summary>
        void CreateFile(string path);

        void CopyFile(string src, string trg);
        void MoveFile(string source, string target);
        bool FileExists(string fileName);
        void DeleteFile(string fileName);

        Stream OpenFile(string file, FileMode mode, FileAccess access);
        void WriteAllByte(byte[] bytes, string fileName);
        string ReadFile(string fileName);

        string GetTempDirectoryName();
        void CreateDirectory(string path);
        bool DirectoryExists(string path);
        void DeleteDirectoryWithContent(string path);

        IEnumerable<string> EnumerateFiles(string path);

        /// <summary>
        ///     unzips the given file to a temporary folder and returns its path
        /// </summary>
        string UnzipToTempFolder(string input);

        IReadingArchive ReadArchive(string fileName);
        IWritingArchive CreateArchive(string fileName);

        int CountLines(string fileName);
    }
}