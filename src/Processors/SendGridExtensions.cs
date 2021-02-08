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
    using System.Collections.Generic;
    using System.Linq;
    using SendGrid.Helpers.Mail;
    using Talegen.Common.Messaging.Models;

    /// <summary>
    /// This class contains extension methods for the SendGrid processor.
    /// </summary>
    public static class SendGridExtensions
    {
        /// <summary>
        /// This message is used to convert a <see cref="SenderMessage" /> to a <see cref="SendGridMessage" /> object.
        /// </summary>
        /// <param name="message">Contains the sender message to convert.</param>
        /// <returns>Returns a new <see cref="SendGridMessage" /> object.</returns>
        public static SendGridMessage ToSendGridMessage(this SenderMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            List<EmailAddress> allRecipients = message.Recipients.Select(rcpt => new EmailAddress(rcpt.Address)).ToList();

            return MailHelper.CreateSingleEmailToMultipleRecipients(new EmailAddress(message.From.Address),
                allRecipients,
                message.Subject,
                message.TextBody,
                message.HtmlBody);
        }
    }
}