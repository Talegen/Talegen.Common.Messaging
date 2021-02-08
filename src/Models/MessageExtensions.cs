/*
 *
 * (c) Copyright Talegen, LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace Talegen.Common.Messaging.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Mail;
    using System.Resources;
    using Talegen.Common.Messaging.Properties;
    using Talegen.Common.Messaging.Templates;

    /// <summary>
    /// This class contains several extension methods for working with and supporting e-mail messages.
    /// </summary>
    public static class MessageExtensions
    {
        /// <summary>
        /// This method is used to create a new <see cref="SenderMessage" /> and populate the message content with optional token values.
        /// </summary>
        /// <param name="settings">Contains an instance of the <see cref="MessageSettingsModel" /> model.</param>
        /// <returns>Returns a <see cref="SenderMessage" /> result.</returns>
        public static SenderMessage CreateSenderMessage(MessageSettingsModel settings, List<Template> templates)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrWhiteSpace(settings.From))
            {
                throw new ArgumentOutOfRangeException(nameof(settings), string.Format(Resources.ParameterMustBeSpecifiedText, nameof(settings.From)));
            }

            if (settings.To == null || settings.To?.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings), string.Format(Resources.ParameterMustBeSpecifiedText, nameof(settings.To)));
            }

            // default to English if no language specified.
            string language = settings.CultureInfoOverride != null ? settings.CultureInfoOverride.Name : "en-US";

            // get text template
            string templateKey = $"{settings.TemplateName}_{language}";
            string textTemplateKey = templateKey + "_txt";
            string textBody = templates.Where(t => t.TemplateKey == textTemplateKey).Select(t => t.Content).FirstOrDefault();

            // get html template
            string htmlTemplateKey = templateKey + "_htm";
            string htmlBody = templates.Where(t => t.TemplateKey == htmlTemplateKey).Select(t => t.Content).FirstOrDefault();

            List<SenderMailAddress> recipients = new List<SenderMailAddress>();
            settings.To.ForEach(r =>
            {
                recipients.Add(new SenderMailAddress(r));
            });

            if (string.IsNullOrWhiteSpace(settings.TextBodyContentType))
            {
                settings.TextBodyContentType = "text/plain";
            }

            if (string.IsNullOrWhiteSpace(settings.HtmlBodyContentType))
            {
                settings.HtmlBodyContentType = "text/html";
            }

            return CreateSenderMessage(new SenderMailAddress(settings.From), recipients, settings.Subject, textBody, htmlBody, false, settings.Tokens, settings.TextBodyContentType, settings.HtmlBodyContentType);
        }

        /// <summary>
        /// This method is used to create a new <see cref="SenderMessage" /> and populate the message content with optional token values.
        /// </summary>
        /// <param name="from">Contains the message sender address.</param>
        /// <param name="to">Contains the message recipient address.</param>
        /// <param name="subject">Contains the message subject.</param>
        /// <param name="templateName">Contains a message template name to retrieve and load into the message body.</param>
        /// <param name="emailTemplateFolder">Contains the email template folder.</param>
        /// <param name="tokensList">Contains an optional tokens list for populating message body with token values.</param>
        /// <param name="resourceManager">Contains the resource manager used for retrieving template content.</param>
        /// <param name="cultureInfoOverride">Contains an optional locale string override for message body resource lookup.</param>
        /// <returns>Returns a new <see cref="SenderMessage" /> containing template message content.</returns>
        public static SenderMessage CreateSenderMessage(string from, string to, string subject, string templateName, string emailTemplateFolder, Dictionary<string, string> tokensList = null, ResourceManager resourceManager = null, CultureInfo cultureInfoOverride = null)
        {
            if (string.IsNullOrWhiteSpace(to))
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (string.IsNullOrWhiteSpace(emailTemplateFolder))
            {
                throw new ArgumentNullException(nameof(emailTemplateFolder));
            }

            if (!Directory.Exists(emailTemplateFolder))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.EmailTemplateDirectoryNotFoundErrorText, emailTemplateFolder));
            }

            // this will look for ; and allow for sending multiple recipients if to has multiple emails.
            return CreateSenderMessage(new SenderMailAddress(from), to.Split(";").Select(addr => new SenderMailAddress(addr)).ToList(), subject, templateName, emailTemplateFolder, tokensList, resourceManager, cultureInfoOverride);
        }

        /// <summary>
        /// This method is used to create a new <see cref="SenderMessage" /> and populate the message content with optional token values.
        /// </summary>
        /// <param name="from">Contains the message sender address.</param>
        /// <param name="to">Contains the message recipient address.</param>
        /// <param name="subject">Contains the message subject.</param>
        /// <param name="templateName">Contains a message template name to retrieve and load into the message body.</param>
        /// <param name="emailTemplateFolder">Contains email template folder.</param>
        /// <param name="tokensList">Contains an optional tokens list for populating message body with token values.</param>
        /// <param name="resourceManager">Contains the resource manager used for retrieving template content.</param>
        /// <param name="cultureInfoOverride">Contains an optional locale string override for message body resource lookup.</param>
        /// <returns>Returns a new <see cref="SenderMessage" /> containing template message content.</returns>
        public static SenderMessage CreateSenderMessage(string from, SenderMailAddress to, string subject, string templateName, string emailTemplateFolder, Dictionary<string, string> tokensList = null, ResourceManager resourceManager = null, CultureInfo cultureInfoOverride = null)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (string.IsNullOrWhiteSpace(emailTemplateFolder))
            {
                throw new ArgumentNullException(nameof(emailTemplateFolder));
            }

            if (!Directory.Exists(emailTemplateFolder))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.EmailTemplateDirectoryNotFoundErrorText, emailTemplateFolder));
            }

            return CreateSenderMessage(new SenderMailAddress(from), new List<SenderMailAddress>() { to }, subject, templateName, emailTemplateFolder, tokensList, resourceManager, cultureInfoOverride);
        }

        /// <summary>
        /// This method is used to create a new <see cref="SenderMessage" /> and populate the message content with optional token values.
        /// </summary>
        /// <param name="from">Contains the message sender address.</param>
        /// <param name="recipients">Contains a list of <see cref="SenderMailAddress" /> recipient objects.</param>
        /// <param name="subject">Contains the message subject.</param>
        /// <param name="templateName">Contains a message template name to retrieve and load into the message body.</param>
        /// <param name="emailTemplateFolder">Contains email template folder.</param>
        /// <param name="tokensList">Contains an optional tokens list for populating message body with token values.</param>
        /// <param name="resourceManager">Contains the resource manager used for retrieving template content.</param>
        /// <param name="cultureInfoOverride">Contains an optional locale string override for message body resource lookup.</param>
        /// <returns>Returns a new <see cref="SenderMessage" /> containing template message content.</returns>
        public static SenderMessage CreateSenderMessage(SenderMailAddress from, List<SenderMailAddress> recipients, string subject, string templateName, string emailTemplateFolder, Dictionary<string, string> tokensList = null, ResourceManager resourceManager = null, CultureInfo cultureInfoOverride = null)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (recipients == null)
            {
                throw new ArgumentNullException(nameof(recipients));
            }

            if (!recipients.Any())
            {
                throw new ArgumentOutOfRangeException(nameof(recipients));
            }

            if (string.IsNullOrWhiteSpace(templateName))
            {
                throw new ArgumentOutOfRangeException(nameof(templateName));
            }

            if (string.IsNullOrWhiteSpace(emailTemplateFolder))
            {
                throw new ArgumentOutOfRangeException(nameof(emailTemplateFolder));
            }

            if (!Directory.Exists(emailTemplateFolder))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.EmailTemplateDirectoryNotFoundErrorText, emailTemplateFolder));
            }

            // default to English if no language specified.
            string language = cultureInfoOverride != null ? cultureInfoOverride.Name : "en-US";
            string textContent = string.Empty;

            FileInfo textTemplateInfo = new FileInfo(Path.Combine(emailTemplateFolder, language, templateName + ".txt"));

            if (textTemplateInfo.Exists)
            {
                using StreamReader reader = textTemplateInfo.OpenText();
                textContent = reader.ReadToEnd();
            }

            string htmlContent = string.Empty;
            FileInfo htmlTemplateInfo = new FileInfo(Path.Combine(emailTemplateFolder, language, templateName + ".htm"));

            if (htmlTemplateInfo.Exists)
            {
                using StreamReader reader = htmlTemplateInfo.OpenText();
                htmlContent = reader.ReadToEnd();
            }

            return CreateSenderMessage(from, recipients, subject, textContent, htmlContent, false, tokensList);
        }

        /// <summary>
        /// This method is used to create a new <see cref="SenderMessage" /> and populate the message content with optional token values.
        /// </summary>
        /// <param name="from">Contains the message sender address.</param>
        /// <param name="to">Contains the message recipient address.</param>
        /// <param name="subject">Contains the message subject.</param>
        /// <param name="textBody">Contains the body of the message in text format.</param>
        /// <param name="htmlBody">Contains the body of the message in HTML format.</param>
        /// <param name="tokensList">Contains an optional tokens list for populating message body with token values.</param>
        /// <param name="textBodyContentType">Contains an optional text body content type.</param>
        /// <param name="htmlBodyContentType">Contains an optional HTML body content type.</param>
        /// <returns>Returns a new <see cref="SenderMessage" /> containing message content.</returns>
        public static SenderMessage CreateSenderMessage(SenderMailAddress from, SenderMailAddress to, string subject, string textBody, string htmlBody = "", Dictionary<string, string> tokensList = null, string textBodyContentType = "text/plain", string htmlBodyContentType = "text/html")
        {
            return CreateSenderMessage(from, new List<SenderMailAddress>() { to }, subject, textBody, htmlBody, false, tokensList, textBodyContentType, htmlBodyContentType);
        }

        /// <summary>
        /// This method is used to create a new <see cref="SenderMessage" /> and populate the message content with optional token values.
        /// </summary>
        /// <param name="from">Contains the message sender address.</param>
        /// <param name="recipients">Contains a list of <see cref="MailAddress" /> recipient objects.</param>
        /// <param name="subject">Contains the message subject.</param>
        /// <param name="textBody">Contains the body of the message in text format.</param>
        /// <param name="htmlBody">Contains the body of the message in HTML format.</param>
        /// <param name="recipientsVisible">
        /// Contains a value indicating whether the recipients are visible to each other. If false, each recipient shall receive their own message.
        /// </param>
        /// <param name="tokensList">Contains an optional tokens list for populating message body with token values.</param>
        /// <param name="textBodyContentType">Contains an optional text body content type.</param>
        /// <param name="htmlBodyContentType">Contains an optional HTML body content type.</param>
        /// <returns>Returns a new <see cref="SenderMessage" /> containing message content.</returns>
        public static SenderMessage CreateSenderMessage(SenderMailAddress from, List<SenderMailAddress> recipients, string subject, string textBody, string htmlBody = "", bool recipientsVisible = false, Dictionary<string, string> tokensList = null, string textBodyContentType = "text/plain", string htmlBodyContentType = "text/html")
        {
            if (tokensList != null)
            {
                if (!string.IsNullOrEmpty(textBody))
                {
                    textBody = textBody.ReplaceTokens(tokensList);
                }

                if (!string.IsNullOrEmpty(htmlBody))
                {
                    htmlBody = htmlBody.ReplaceTokens(tokensList);
                }
            }

            return new SenderMessage(from, recipients, subject, textBody, htmlBody, recipientsVisible, textBodyContentType, htmlBodyContentType);
        }

        /// <summary>
        /// This extension method is used to populate a message tokens list with commonly used values for an e-mail message.
        /// </summary>
        /// <param name="tokenValues">Contains the token values to add to.</param>
        /// <param name="requestedUri">Contains the requested Uri if any to use and base URL token value.</param>
        /// <param name="recipientUser">Contains the user to which the email is being sent.</param>
        /// <param name="senderUser">Contains the user to which the email is being sent from.</param>
        public static void InitializeBaseTokens(this Dictionary<string, string> tokenValues, Uri requestedUri = null, MessageUser recipientUser = null, MessageUser senderUser = null)
        {
            if (tokenValues == null)
            {
                throw new ArgumentNullException(nameof(tokenValues));
            }

            if (senderUser != null)
            {
                if (!tokenValues.ContainsKey(TemplateTokens.FromUserName))
                {
                    tokenValues.Add(TemplateTokens.FromUserName, senderUser.UserName);
                }

                if (!tokenValues.ContainsKey(TemplateTokens.FromFirstName))
                {
                    tokenValues.Add(TemplateTokens.FromFirstName, senderUser.FirstName);
                }

                if (!tokenValues.ContainsKey(TemplateTokens.FromFullName))
                {
                    tokenValues.Add(TemplateTokens.FromFullName, senderUser.FullName);
                }

                if (!tokenValues.ContainsKey(TemplateTokens.FromEmail))
                {
                    tokenValues.Add(TemplateTokens.FromEmail, senderUser.Email);
                }
            }

            if (recipientUser != null)
            {
                if (!tokenValues.ContainsKey(TemplateTokens.UserId))
                {
                    tokenValues.Add(TemplateTokens.UserId, recipientUser.UserId);
                }

                if (!tokenValues.ContainsKey(TemplateTokens.UserName))
                {
                    tokenValues.Add(TemplateTokens.UserName, recipientUser.UserName);
                }

                if (!tokenValues.ContainsKey(TemplateTokens.FirstName))
                {
                    tokenValues.Add(TemplateTokens.FirstName, recipientUser.FirstName);
                }

                if (!tokenValues.ContainsKey(TemplateTokens.FullName))
                {
                    tokenValues.Add(TemplateTokens.FullName, recipientUser.FullName);
                }

                if (!tokenValues.ContainsKey(TemplateTokens.UserEmail))
                {
                    tokenValues.Add(TemplateTokens.UserEmail, recipientUser.Email);
                }
            }

            if (requestedUri != null)
            {
                if (!tokenValues.ContainsKey(TemplateTokens.RequestedUrl))
                {
                    tokenValues.Add(TemplateTokens.RequestedUrl, requestedUri.ToString());
                }

                if (!tokenValues.ContainsKey(TemplateTokens.Url))
                {
                    tokenValues.Add(TemplateTokens.Url, requestedUri.GetLeftPart(UriPartial.Authority));
                }
            }
        }

        /// <summary>
        /// This method is used to render dates and times to the token values list for a recipient's specific culture.
        /// </summary>
        /// <param name="tokenValues">Contains the token values to add to.</param>
        /// <param name="recipientLanguageCode">Contains the recipient user locale code.</param>
        public static void InitializeDateTimeTokens(this Dictionary<string, string> tokenValues, string recipientLanguageCode = "")
        {
            if (tokenValues == null)
            {
                throw new ArgumentNullException(nameof(tokenValues));
            }

            CultureInfo recipientCulture = CultureInfo.CreateSpecificCulture(recipientLanguageCode);

            DateTime currentUtcNow = DateTime.UtcNow;
            DateTime currentNow = DateTime.Now;

            if (!tokenValues.ContainsKey(TemplateTokens.UtcDateTime))
            {
                tokenValues.Add(TemplateTokens.UtcDateTime, currentUtcNow.ToString(recipientCulture));
            }

            if (!tokenValues.ContainsKey(TemplateTokens.UtcDate))
            {
                tokenValues.Add(TemplateTokens.UtcDate, currentUtcNow.ToShortDateString());
            }

            if (!tokenValues.ContainsKey(TemplateTokens.DateTime))
            {
                tokenValues.Add(TemplateTokens.DateTime, currentNow.ToString(recipientCulture));
            }

            if (!tokenValues.ContainsKey(TemplateTokens.Date))
            {
                tokenValues.Add(TemplateTokens.Date, currentNow.ToShortDateString());
            }

            if (!tokenValues.ContainsKey(TemplateTokens.Time))
            {
                tokenValues.Add(TemplateTokens.Time, currentNow.ToShortTimeString());
            }
        }

        /// <summary>
        /// This extension is used to determine if a specified e-mail address string is within a MailAddressCollection of MailAddress objects.
        /// </summary>
        /// <param name="collection">Contains the collection of mail address objects to search.</param>
        /// <param name="emailAddress">Contains the e-mail address to find.</param>
        /// <returns>Returns a value indicating whether the e-mail address is a member of the specified mail address collection.</returns>
        public static bool Contains(this MailAddressCollection collection, string emailAddress)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                throw new ArgumentNullException(nameof(emailAddress));
            }

            return collection.Contains(new MailAddress(emailAddress));
        }
    }
}