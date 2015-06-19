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

using System.Globalization;
using KaVE.VS.FeedbackGenerator.Interactivity;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Presentation
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
        private readonly UploadUrlValidationRule _uploadUrlValidationRule;
        private readonly WebAccessPrefixValidationRule _webAccessPrefixValidationRule;

        public IInteractionRequest<Notification> ErrorNotificationRequest
        {
            get { return _errorNotificationRequest; }
        }

        public ExportSettings ExportSettings
        {
            get; set; 
        }

        public string UploadUrl
        {
            get { return ExportSettings.UploadUrl; }
            set { ExportSettings.UploadUrl = value; }

        }

        public string WebAccessPrefix
        {
            get { return ExportSettings.WebAccessPrefix; }
            set { ExportSettings.WebAccessPrefix = value; }
        }

        public OptionPageViewModel()
        {
            _errorNotificationRequest = new InteractionRequest<Notification>();
            _uploadUrlValidationRule = new UploadUrlValidationRule();
            _webAccessPrefixValidationRule = new WebAccessPrefixValidationRule();
        }

        public UploadValidation ValidateUploadInformation(string url, string prefix)
        {
            var uriIsValid = _uploadUrlValidationRule.Validate(url,CultureInfo.CurrentUICulture);
            var prefixIsValid = _webAccessPrefixValidationRule.Validate(prefix,CultureInfo.CurrentUICulture);

            if (!uriIsValid.IsValid || !prefixIsValid.IsValid)
            {
                ShowInformationInvalidMessage();
            }

            return new UploadValidation(uriIsValid.IsValid, prefixIsValid.IsValid);
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

    }
}