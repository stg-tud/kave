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
 * 
 * Contributors:
 *    - Dennis Albrecht
 *    - Sebastian Proksch
 *    - Sven Amann
 *    - Uli Fahrer
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using JetBrains.Application;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface IIoUtils
    {
        HttpResponseMessage TransferByHttp([NotNull] HttpContent content,
            [NotNull] Uri targetUri,
            int timeoutInSeconds = 5);

        string GetTempFileName();
        string GetTempFileName(string extension);
        string[] GetFiles(string path, string searchPattern);
        long GetFileSize(string fileName);
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

        string Combine(params string[] paths);
        IEnumerable<string> EnumerateFiles(string path);
    }

    [ShellComponent]
    public class IoUtils : IIoUtils
    {
        public HttpResponseMessage TransferByHttp(HttpContent content, Uri targetUri, int timeoutInSeconds)
        {
            var isHttp = targetUri.Scheme == Uri.UriSchemeHttp;
            var isHttps = targetUri.Scheme == Uri.UriSchemeHttps;
            Asserts.That(isHttp || isHttps, Messages.ServerRequestWrongScheme);

            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, timeoutInSeconds);
                HttpResponseMessage response;
                try
                {
                    response = client.PostAsync(targetUri, content).Result;
                }
                catch (Exception)
                {
                    response = null;
                    Asserts.Fail(Messages.ServerRequestNotAvailable, targetUri);
                }
                Asserts.That(response.IsSuccessStatusCode, Messages.ServerResponseFailure, targetUri, response.StatusCode);
                return response;
            }
        }

        public void CopyFile(string src, string trg)
        {
            File.Copy(src, trg, true);
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

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
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

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFiles(path).Any();
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
            File.Create(path).Close();
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }
    }
}