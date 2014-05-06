﻿using System;
using System.IO;
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

        void CreateFile(string path);
        void CopyFile(string src, string trg);
        string GetTempFileName();
        bool FileExists(string fileName);
        Stream OpenFile(string file, FileMode mode, FileAccess access);
        string ReadFile(string fileName);
        void MoveFile(string source, string target);
        void DeleteFile(string fileName);

        void CreateDirectory(string path);
        void DeleteDirectoryWithContent(string path);
    }

    [ShellComponent]
    public class IoUtils : IIoUtils
    {
        public HttpResponseMessage TransferByHttp(HttpContent content, Uri targetUri, int timeoutInSeconds)
        {
            Asserts.That(targetUri.Scheme == Uri.UriSchemeHttp, Messages.ServerRequestWrongScheme);

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
                    Asserts.Fail(Messages.ServerRequestNotAvailable);
                }
                Asserts.That(response.IsSuccessStatusCode, Messages.ServerResponseFailure);
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

        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
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
    }
}