using System;
using System.Net.Http;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;
using Newtonsoft.Json;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

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
            Asserts.That(_ioUtils.FileExists(srcFilename));

            var content = CreateMultipartContent(srcFilename, "tmp.log");
            var response = _ioUtils.TransferByHttp(content, _hostAddress);
            var json = response.Content.ReadAsStringAsync().Result;

            Asserts.Not(json.IsNullOrEmpty(), Messages.ServerResponseEmpty);

            ExportResult<object> res = null;
            try
            {
                res = Deserialize(json);
            }
            catch (Exception)
            {
                Asserts.Fail(Messages.ServerResponseIncorrentFormat, json);
            }

            Asserts.NotNull(res, Messages.ServerResponseIncompatible);
            Asserts.That(res.Status == State.Ok, Messages.ServerResponseRequestFailure, res.Message);
        }

        private HttpContent CreateMultipartContent([NotNull] string file, [NotNull] string name)
        {
            var content = _ioUtils.ReadFile(file);
            return new MultipartFormDataContent
            {
                {new ByteArrayContent(content.AsBytes()), "file", name}
            };
        }

        private static ExportResult<object> Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<ExportResult<object>>(json);
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