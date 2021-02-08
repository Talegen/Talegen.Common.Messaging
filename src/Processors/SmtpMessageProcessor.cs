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
    using System.Net;
    using System.Net.Mail;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Talegen.Common.Core.Errors;
    using Talegen.Common.Messaging.Configuration;
    using Talegen.Common.Messaging.Models;
    using Talegen.Common.Messaging.Properties;

    /// <summary>
    /// This class contains the logic for processing queued messages using a common SMTP messaging service.
    /// </summary>
    public class SmtpMessageProcessor : IMessageProcessor
    {
        /// <summary>
        /// Contains an instance of the error manager.
        /// </summary>
        private readonly IErrorManager errorManager;

        /// <summary>
        /// The messaging settings.
        /// </summary>
        private readonly MessagingSettings settings;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmtpMessageProcessor" /> class.
        /// </summary>
        /// <param name="settings">Contains message settings.</param>
        /// <param name="errorManager">Contains an instance of the error manager.</param>
        /// <param name="logger">Contains a logger instance for error reporting.</param>
        public SmtpMessageProcessor(MessagingSettings settings, IErrorManager errorManager, ILogger logger)
        {
            this.settings = settings;
            this.errorManager = errorManager;
            this.logger = logger;
        }

        /// <summary>
        /// This method is used to send a message for the application.
        /// </summary>
        /// <param name="message">Contains the mail message to send.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns an async Task result.</returns>
        public async Task ProcessMessageAsync(SenderMessage message, CancellationToken cancellationToken = default)
        {
            // new up a new SMTP client
            SmtpClient client = new SmtpClient(this.settings.HostName, this.settings.Port);

            try
            {
                // setup credentials if necessary
                if (!string.IsNullOrWhiteSpace(this.settings.UserName))
                {
                    client.Credentials = new NetworkCredential(this.settings.UserName, this.settings.Password);
                    client.UseDefaultCredentials = false;
                }

                client.EnableSsl = this.settings.UseSsl;

                using (var emailMessage = message.ToMailMessage())
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // send the message
                        await client.SendMailAsync(emailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, Resources.ProcessorSmtpErrorText);
                this.errorManager?.Critical(ex, ErrorCategory.Application);
                throw;
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}