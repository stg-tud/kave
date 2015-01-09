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
 *    - Uli Fahrer
 *    - Sebastian Proksch
 */

using System;
using KaVE.VsFeedbackGenerator.Interactivity;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public class UploadValidation
    {
        public UploadValidation(bool isUrlValid, bool isPrefixValid)
        {
            IsUrlValid = isUrlValid;
            IsPrefixValid = isPrefixValid;
        }

        public bool IsUrlValid { private set; get; }
        public bool IsPrefixValid { private set; get; }

        public bool IsValidUploadInformation
        {
            get { return IsUrlValid && IsPrefixValid; }
        }
    }

    public class OptionPageViewModel : ViewModelBase<OptionPageViewModel>
    {
        private readonly InteractionRequest<Notification> _errorNotificationRequest;

        public IInteractionRequest<Notification> ErrorNotificationRequest
        {
            get { return _errorNotificationRequest; }
        }

        public OptionPageViewModel()
        {
            _errorNotificationRequest = new InteractionRequest<Notification>();
        }

        public UploadValidation ValidateUploadInformation(string url, string prefix)
        {
            var uriIsValid = ValidateUri(url);
            var prefixIsEmpty = prefix.Length == 0;
            var prefixIsValid = prefixIsEmpty || ValidateUri(prefix);

            if (!uriIsValid || !prefixIsValid)
            {
                ShowInformationInvalidMessage();
            }

            return new UploadValidation(uriIsValid, prefixIsValid);
        }

        private void ShowInformationInvalidMessage()
        {
            _errorNotificationRequest.Raise(
                new Notification
                {
                    Caption = Properties.SessionManager.Options_Title,
                    Message = Properties.SessionManager.OptionPageErrorMessage,
                });
        }

        private static bool ValidateUri(string url)
        {
            Uri uri;
            return Uri.TryCreate(url, UriKind.Absolute, out uri) && HasSupportedScheme(uri);
        }

        private static bool HasSupportedScheme(Uri uri)
        {
            return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}