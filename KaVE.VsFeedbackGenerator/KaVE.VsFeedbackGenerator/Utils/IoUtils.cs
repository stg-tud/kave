using System;
using System.IO;
using System.Net.Http;
using JetBrains.Application;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public interface IIoUtils
    {
        HttpResponseMessage TransferByHttp([NotNull] HttpContent content,
            [NotNull] Uri targetUri,
            int timeoutInSeconds = 5);

        void CopyFile(string src, string trg);
        string GetTempFileName();
        bool FileExists(string fileName);
        string ReadFile(string fileName);
    }

    [ShellComponent]
    public class IoUtils : IIoUtils
    {
        public HttpResponseMessage TransferByHttp(HttpContent content, Uri targetUri, int timeoutInSeconds)
        {
            Asserts.That(targetUri.Scheme == Uri.UriSchemeHttp, "Http-Upload erwartet Http-Adresse");

            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, timeoutInSeconds);
                try
                {
                    var response = client.PostAsync(targetUri, content).Result;
                    Asserts.That(!response.IsSuccessStatusCode, "Server Request fehlgeschlagen");
                    return response;
                }
                catch (Exception)
                {
                    Asserts.Fail("Server nicht erreichbar");
                }
            }

            // TODO review with Sven
            throw new AssertException("unreachable code");
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
    }
}