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
    using System.Collections.Concurrent;
    using Talegen.Common.Messaging.Models;

    /// <summary>
    /// This class represents the messaging queue used for handling the processing of messages in the system.
    /// </summary>
    public static class MessagingQueue
    {
        /// <summary>
        /// Gets the thread-safe queue for handling messages.
        /// </summary>
        public static ConcurrentQueue<SenderMessage> Messages { get; } = new ConcurrentQueue<SenderMessage>();

        /// <summary>
        /// Adds a new sender message to the queue to be processed.
        /// </summary>
        /// <param name="message">Contains the message to process.</param>
        public static void Add(SenderMessage message)
        {
            Messages.Enqueue(message);
        }
    }
}