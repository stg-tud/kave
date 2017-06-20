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
using System.Net;
using System.Net.Http;
using JetBrains.Application;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using KaVE.RS.Commons.Properties;

namespace KaVE.RS.Commons.Injectables
{
    [ShellComponent]
    public class InjectableIoUtils : IoUtils
    {
        public override HttpResponseMessage TransferByHttp(HttpContent content, Uri targetUri)
        {
            var isHttp = targetUri.Scheme == Uri.UriSchemeHttp;
            var isHttps = targetUri.Scheme == Uri.UriSchemeHttps;
            Asserts.That(isHttp || isHttps, Messages.ServerRequestWrongScheme);

            using (var client = new HttpClient())
            {
                // allow self-signed certificates for upload
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
                try
                {
                    var response = client.PostAsync(targetUri, content).Result;
                    Asserts.That(
                        response.IsSuccessStatusCode,
                        Messages.ServerResponseFailure,
                        targetUri,
                        response.StatusCode);
                    return response;
                }
                catch (Exception e)
                {
                    var hre = e.InnerException as HttpRequestException;
                    if (hre != null)
                    {
                        var we = hre.InnerException as WebException;
                        if (we != null)
                        {
                            throw new Exception(
                                string.Format(Messages.ServerResponseFailure, targetUri, we.Message),
                                e);
                        }
                    }
                    throw new Exception(
                        string.Format(Messages.ServerRequestNotAvailable, targetUri),
                        e);
                }
            }
        }
    }
}