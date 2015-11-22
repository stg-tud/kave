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
using Ionic.Zip;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.Json;
using KaVE.RS.Commons.Properties;
using Newtonsoft.Json;

namespace KaVE.RS.Commons.Utils
{
    public interface IPublisherUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="events"></param>
        /// <param name="stream"></param>
        /// <param name="progressCallback">Is called once for every processed event other than the UserProfileEvent.</param>
        /// <returns>True if events other than the UserProfileEvent were written to the stream.</returns>
        bool WriteEventsToZipStream(IEnumerable<IDEEvent> events, Stream stream, Action progressCallback);

        void UploadEventsByHttp(IIoUtils ioUtils, Uri hostAddress, MemoryStream stream);
    }

    [ShellComponent]
    public class PublisherUtils : IPublisherUtils
    {
        public bool WriteEventsToZipStream(IEnumerable<IDEEvent> events,
            Stream stream,
            Action progressCallback)
        {
            var i = 0;
            
            using (var zipFile = new ZipFile())
            {
                zipFile.UseZip64WhenSaving = Zip64Option.AsNecessary;
                foreach (var e in events)
                {
                    if (e is UserProfileEvent)
                    {
                        zipFile.AddEntry("UserProfileEvent.json", e.ToFormattedJson());
                    }
                    else
                    {
                        var fileName = (i++) + "-" + e.GetType().Name + ".json";
                        var json = e.ToFormattedJson();
                        zipFile.AddEntry(fileName, json);

                        progressCallback();
                    }
                }

                zipFile.Save(stream);
            }

            return i > 0;
        }

        public void UploadEventsByHttp(IIoUtils ioUtils, Uri hostAddress, MemoryStream stream)
        {
            Asserts.That(stream.CanRead);
            var content = CreateMultipartContent(stream, "tmp.zip");
            var response = ioUtils.TransferByHttp(content, hostAddress);
            var json = response.Content.ReadAsStringAsync().Result;

            Asserts.Not(string.IsNullOrEmpty(json), Messages.ServerResponseEmpty);

            ExportResult res;
            try
            {
                res = DeserializeResult(json);
            }
            catch (JsonException e)
            {
                throw new InvalidResponseException(Messages.ServerResponseIncorrentFormat.FormatEx(json), e);
            }

            Asserts.NotNull(res, Messages.ServerResponseIncompatible);
            Asserts.That(res.Status == State.Ok, Messages.ServerResponseRequestFailure, res.Message);
        }

        private static HttpContent CreateMultipartContent([NotNull] MemoryStream stream, [NotNull] string name)
        {
            return new MultipartFormDataContent
            {
                // Server refuses data if we use StreamContent instead, claiming it's not a valid zip file.
                {new ByteArrayContent(stream.ToArray()), "file", name}
            };
        }

        private static ExportResult DeserializeResult(string json)
        {
            return JsonConvert.DeserializeObject<ExportResult>(json);
        }

        internal enum State
        {
            Ok,
            Fail
        }

        private class ExportResult
        {
            [UsedImplicitly]
            public State Status;

            [UsedImplicitly]
            public string Message;
        }
    }
}