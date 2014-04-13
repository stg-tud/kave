using System;
using System.IO;
using System.Net.Http;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal static class HttpPostFileTransfer
    {
        public static void TransferFile([NotNull] string targetUrl, [NotNull] string file, string name = null)
        {
            Asserts.That(File.Exists(file), "Angegebene Datei existiert nicht");

            TransferContent(targetUrl, CreateHttpContent(file, name));
        }

        public static void TransferContent([NotNull] string targetUrl, [NotNull] HttpContent content)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 5);
                HttpResponseMessage response = null;
                try
                {
                    response = client.PostAsync(targetUrl, content).Result;
                }
                catch (Exception)
                {
                    Asserts.Fail("Server nicht erreichbar");
                }
                Asserts.That(!response.IsSuccessStatusCode, "Server Request fehlgeschlagen");

                var json = response.Content.ReadAsStringAsync().Result;
                var res = Deserialize(json);
                Asserts.NotNull(res, "Inkompatible Antwort des Server");
                Asserts.That(res.Status == State.Ok, res.Message);
            }
        }

        private static ExportResult<object> Deserialize(string json)
        {
            var res = JsonConvert.DeserializeObject<ExportResult<object>>(
                json,
                JsonLogSerialization.Settings);
            return res;
        }

        public static HttpContent CreateHttpContent([NotNull] string file, string name = null)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            var formData = new MultipartFormDataContent
            {
                {new ByteArrayContent(File.ReadAllBytes(file)), "file", name ?? Path.GetFileName(file)}
            };
            return formData;
        }
    }
}