using System.IO;
using System.Net.Http;
using KaVE.JetBrains.Annotations;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal static class HttpPostFileTransfer
    {
        public static TResponse TransferFile<TResponse>([NotNull] string targetUrl, [NotNull] string file, string name = null)
        {
            if (!File.Exists(file))
            {
                return default(TResponse);
            }
            return TransferContent<TResponse>(targetUrl, CreateHttpContent(file, name));
        }

        public static TResponse TransferContent<TResponse>([NotNull] string targetUrl, [NotNull] HttpContent content)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsync(targetUrl, content).Result;
                if (!response.IsSuccessStatusCode)
                {
                    return default(TResponse);
                }
                var serializedResponse = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TResponse>(serializedResponse, JsonLogSerialization.Settings);
            }
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