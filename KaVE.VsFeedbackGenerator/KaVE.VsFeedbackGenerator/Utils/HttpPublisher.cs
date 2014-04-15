using System;
using System.Net.Http;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Newtonsoft.Json;
using NuGet;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public class HttpPublisher : IPublisher
    {
        private readonly Uri _hostAddress;
        private readonly IIoUtils _ioUtils;

        public HttpPublisher([NotNull] Uri hostAddress)
        {
            _hostAddress = hostAddress;
            _ioUtils = Registry.GetComponent<IIoUtils>();
        }

        public void Publish(string srcFilename)
        {
            Asserts.That(_ioUtils.FileExists(srcFilename), "Quelldatei existiert nicht");

            string json = null;
            try
            {
                var content = CreateMultipartContent(srcFilename, "tmp.log");
                var response = _ioUtils.TransferByHttp(content, _hostAddress);
                json = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Asserts.Fail(e.Message);
            }

            Asserts.NotNull(json, "asd");
            Asserts.Not(json.IsEmpty(), "asd");

            ExportResult<object> res = null;
            try
            {
                res = Deserialize(json);
            }
            catch (Exception e)
            {
                Asserts.Fail(e.Message);
            }

            Asserts.NotNull(res, "Inkompatible Antwort des Server");
            Asserts.That(res.Status == State.Ok, res.Message);
        }

        private HttpContent CreateMultipartContent([NotNull] string file, [NotNull] string name)
        {
            var content = _ioUtils.ReadFile(file);
            var formData = new MultipartFormDataContent
            {
                {new ByteArrayContent(content.GetBytes()), "file", name}
            };
            return formData;
        }

        private static ExportResult<object> Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<ExportResult<object>>(
                json,
                JsonLogSerialization.Settings);
        }

        internal enum State
        {
            Ok,
            Fail
        }

        internal class ExportResult<T>
        {
            public State Status;
            public string Message;
            public T Data;
        }
    }
}