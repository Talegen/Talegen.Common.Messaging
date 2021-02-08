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

namespace Talegen.Common.Messaging
{
    using System;
    using Talegen.Common.Messaging.Processors;
    using Talegen.Common.Messaging.Senders;
    using Talegen.Common.Messaging.Configuration;
    using Talegen.Common.Core.Errors;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// This class implements a factory pattern for creating a new message sender based on the settings specified.
    /// </summary>
    public static class MessagingFactory
    {
        /// <summary>
        /// Creates a new implementation of the <see cref="IMessageSender" /> interface based on the message sender specified in the settings.
        /// </summary>
        /// <param name="settings">Contains the message settings.</param>
        /// <param name="logger">Contains a logger instance for error reporting.</param>
        /// <param name="errorManager">Contains an optional error manager used for reporting errors.</param>
        /// <returns>Returns a new implementation of the <see cref="IMessageSender" /> interface.</returns>
        public static IMessageProcessor CreateProcessor(MessagingSettings settings, IErrorManager errorManager = null, ILogger logger = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            IMessageProcessor processor;

            switch (settings.MessagingType)
            {
                case MessagingType.Smtp:
                    processor = new SmtpMessageProcessor(settings, errorManager, logger);
                    break;

                case MessagingType.SendGridApi:
                    processor = new SendGridMessageProcessor(settings, errorManager, logger);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return processor;
        }

        /// <summary>
        /// Creates a new implementation of the <see cref="IMessageSender" /> interface based on the message sender specified in the settings.
        /// </summary>
        /// <param name="settings">Contains the message settings.</param>
        /// <returns>Returns a new implementation of the <see cref="IMessageSender" /> interface.</returns>
        public static IMessageSender CreateSender(MessagingSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            IMessageSender sender;

            switch (settings.MessagingType)
            {
                case MessagingType.Smtp:
                case MessagingType.SendGridApi:
                    sender = new QueuedMessageSender();
                    break;

                default:
                    throw new NotSupportedException();
            }

            return sender;
        }
    }
}