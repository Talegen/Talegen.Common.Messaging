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
    using System.Xml.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// This class represents a message to be sent via the sender messaging system.
    /// </summary>
    public sealed class SenderMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SenderMessage" /> class.
        /// </summary>
        public SenderMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SenderMessage" /> class.
        /// </summary>
        /// <param name="from">Contains the sender address.</param>
        /// <param name="to">Contains a recipient address.</param>
        /// <param name="subject">Contains the message subject.</param>
        /// <param name="textBody">Contains the text message body.</param>
        /// <param name="htmlBody">Contains the HTML message body.</param>
        /// <param name="textBodyContentType">Contains the optional text body content type value.</param>
        /// <param name="htmlBodyContentType">Contains the optional HTML body content type value.</param>
        public SenderMessage(string from, string to = "", string subject = "", string textBody = "", string htmlBody = "", string textBodyContentType = "text/plain", string htmlBodyContentType = "text/html")
            : this(new SenderMailAddress(from), new SenderMailAddress(to), subject, textBody, htmlBody, textBodyContentType, htmlBodyContentType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SenderMessage" /> class.
        /// </summary>
        /// <param name="from">Contains the sender address.</param>
        /// <param name="to">Contains a recipient address.</param>
        /// <param name="subject">Contains the message subject.</param>
        /// <param name="textBody">Contains the text message body.</param>
        /// <param name="htmlBody">Contains the HTML message body.</param>
        /// <param name="textBodyContentType">Contains the optional text body content type value.</param>
        /// <param name="htmlBodyContentType">Contains the optional HTML body content type value.</param>
        public SenderMessage(SenderMailAddress from, SenderMailAddress to, string subject = "", string textBody = "", string htmlBody = "", string textBodyContentType = "text/plain", string htmlBodyContentType = "text/html")
            : this(from, new List<SenderMailAddress>() { to }, subject, textBody, htmlBody, false, textBodyContentType, htmlBodyContentType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SenderMessage" /> class.
        /// </summary>
        /// <param name="from">Contains the sender address.</param>
        /// <param name="recipients">Contains a list of recipient mail addresses.</param>
        /// <param name="subject">Contains the message subject.</param>
        /// <param name="textBody">Contains the text message body.</param>
        /// <param name="htmlBody">Contains the HTML message body.</param>
        /// <param name="recipientsVisible">
        /// Contains a value indicating whether the recipients are visible to each other. If false, each recipient will receive a single copy of the message.
        /// </param>
        /// <param name="textBodyContentType">Contains the optional text body content type value.</param>
        /// <param name="htmlBodyContentType">Contains the optional HTML body content type value.</param>
        public SenderMessage(SenderMailAddress from, List<SenderMailAddress> recipients, string subject = "", string textBody = "", string htmlBody = "", bool recipientsVisible = false, string textBodyContentType = "text/plain", string htmlBodyContentType = "text/html")
        {
            this.From = from ?? throw new ArgumentNullException(nameof(from));
            this.Recipients = recipients ?? throw new ArgumentNullException(nameof(recipients));

            this.RecipientsVisible = recipientsVisible;
            this.Subject = subject;
            this.TextBody = textBody;
            this.HtmlBody = htmlBody;
            this.TextContentType = textBodyContentType;
            this.HtmlContentType = htmlBodyContentType;
        }

        /// <summary>
        /// Gets a list of recipient addresses.
        /// </summary>
        public List<SenderMailAddress> Recipients { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the recipients are all included in a single message.
        /// </summary>
        public bool RecipientsVisible { get; set; }

        /// <summary>
        /// Gets or sets the from mail address for the message.
        /// </summary>
        public SenderMailAddress From { get; set; }

        /// <summary>
        /// Gets or sets the subject of the message.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the optional text body content type.
        /// </summary>
        public string TextContentType { get; set; }

        /// <summary>
        /// Gets or sets the text body of the message.
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// Gets or sets the HTML body of the message.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// Gets a value indicating whether the message contains HTML content.
        /// </summary>
        [XmlIgnore]
        public bool IsHtml => !string.IsNullOrWhiteSpace(this.HtmlBody);

        /// <summary>
        /// Gets or sets the optional HTML body content type.
        /// </summary>
        public string HtmlContentType { get; set; }

        /// <summary>
        /// This method is used to convert the message class into a string for reporting.
        /// </summary>
        /// <returns>Returns a JSON formatted string.</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}