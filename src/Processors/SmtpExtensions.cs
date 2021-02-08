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

namespace Talegen.Common.Messaging.Processors
{
    using System;
    using System.Net.Mail;
    using System.Net.Mime;
    using Talegen.Common.Messaging.Models;

    /// <summary>
    /// This class contains extension methods for the SMTP processor.
    /// </summary>
    public static class SmtpExtensions
    {
        /// <summary>
        /// This message is used to convert a <see cref="SenderMessage" /> to a <see cref="MailMessage" /> object.
        /// </summary>
        /// <param name="message">Contains the sender message to convert.</param>
        /// <returns>Returns a new <see cref="MailMessage" /> object.</returns>
        public static MailMessage ToMailMessage(this SenderMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            MailMessage result = new MailMessage
            {
                From = new MailAddress(message.From.Address)
            };

            message.Recipients.ForEach(rcpt =>
            {
                result.To.Add(rcpt.Address);
            });

            result.Subject = message.Subject;

            if (!string.IsNullOrEmpty(message.TextBody))
            {
                result.Body = message.TextBody;
                result.IsBodyHtml = false;
            }

            if (message.IsHtml)
            {
                if (string.IsNullOrEmpty(message.TextBody))
                {
                    result.Body = message.HtmlBody;
                    result.IsBodyHtml = true;
                }
                else
                {
                    result.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.HtmlBody, new ContentType(message.HtmlContentType)));
                }
            }

            return result;
        }
    }
}