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

namespace Talegen.Common.Messaging.Senders
{
    using System.Threading;
    using System.Threading.Tasks;
    using Talegen.Common.Messaging.Models;

    /// <summary>
    /// A basic implementation of the <see cref="IMessageSender" /> that sends messages to the message queue.
    /// </summary>
    public class QueuedMessageSender : IMessageSender
    {
        /// <inheritdoc />
        /// <summary>
        /// This method is used to send a message for the application.
        /// </summary>
        /// <param name="message">Contains the mail message to send.</param>
        /// <returns>Returns a completed task.</returns>
        public void SendMessage(SenderMessage message)
        {
            MessagingQueue.Add(message);
        }

        /// <summary>
        /// This method is used to send a message for the application.
        /// </summary>
        /// <param name="message">Contains the mail message to send.</param>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns an async Task result.</returns>
        public Task SendMessageAsync(SenderMessage message, CancellationToken cancellationToken = default)
        {
            this.SendMessage(message);
            return Task.CompletedTask;
        }
    }
}