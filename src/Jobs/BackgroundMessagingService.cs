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

namespace Talegen.Common.Messaging.Jobs
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Logging;
    using Talegen.Common.Core.Extensions;
    using Talegen.Common.Messaging.Models;
    using Talegen.Common.Messaging.Processors;
    using Talegen.Common.Messaging.Properties;

    /// <summary>
    /// This class will process messages in the messaging queue using the configured message processor.
    /// </summary>
    public sealed class BackgroundMessagingService
    {
        /// <summary>
        /// Contains an instance of a logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Contains the message queue path where unsent messages are stored and loaded on stop and start of application.
        /// </summary>
        private readonly string messageQueuePath;

        /// <summary>
        /// Contains the messaging processor.
        /// </summary>
        private readonly IMessageProcessor processor;

        /// <summary>
        /// Contains a value indicating whether the queue should be processing.
        /// </summary>
        private bool processing = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundMessagingService" /> class.
        /// </summary>
        /// <param name="processor">Contains an optional pre-defined processor</param>
        /// <param name="tempMessageStorePath">Contains a temporary message storage path directory.</param>
        /// <param name="logger">Contains an optional logger for error reporting.</param>
        public BackgroundMessagingService(IMessageProcessor processor, string tempMessageStorePath = "App_Data\\Messages", ILogger logger = null)
        {
            this.processor = processor ?? throw new ArgumentNullException(nameof(processor));
            this.messageQueuePath = Path.Combine(Environment.CurrentDirectory, tempMessageStorePath);
            this.logger = logger;
        }

        /// <summary>
        /// This method is used to execute and process the queue messaging.
        /// </summary>
        public void Process()
        {
            this.logger?.LogDebug(Resources.LoggingProcessingBackgroundJobText, this.processor.ToString());

            // execute queue processing
            while (this.processing && MessagingQueue.Messages.TryDequeue(out SenderMessage message))
            {
                try
                {
                    this.processor.ProcessMessageAsync(message);
                }
                catch (Exception ex)
                {
                    this.logger?.LogError(ex, Resources.MessageSenderErrorText);
                }
            }
        }

        /// <summary>
        /// This method is used to store the remaining queue items to disk during a shutdown event.
        /// </summary>
        public void StoreQueue()
        {
            // if the queue message path exists...
            if (this.CheckMessagePath())
            {
                // stop processing....
                this.processing = false;

                // execute queue storage
                while (MessagingQueue.Messages.TryDequeue(out SenderMessage message))
                {
                    if (message != null)
                    {
                        try
                        {
                            string tempFilePath = Path.Combine(this.messageQueuePath, $"{Guid.NewGuid()}.msg");

                            // the file should never exist, but check anyway...
                            if (!File.Exists(tempFilePath))
                            {
                                message.Serialize(tempFilePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.logger?.LogError(ex, Resources.MessageSenderErrorText);
                        }
                    }
                    else
                    {
                        this.logger?.LogError(Resources.LoggingErrorQueueLoadText);
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to restore queue items from the disk during a startup
        /// </summary>
        public void RestoreQueue()
        {
            // if there are messages...
            if (this.CheckMessagePath())
            {
                string[] files = Directory.GetFiles(this.messageQueuePath);

                // for each file file in the message queue path.
                foreach (string filePath in files)
                {
                    try
                    {
                        FileInfo file = new FileInfo(filePath);

                        if (file.Exists)
                        {
                            SenderMessage message = file.Deserialize<SenderMessage>();
                            if (message != null)
                            {
                                // add message to queue
                                MessagingQueue.Add(message);

                                // remove the file from disk storage
                                file.Delete();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger?.LogError(ex, Resources.StorageDirectoryCommandErrorText, this.messageQueuePath, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to check on the existence of the message queue path, and if it doesn't exist, it is created.
        /// </summary>
        /// <returns>Returns a value indicating if the path exists.</returns>
        public bool CheckMessagePath()
        {
            // if the folder doesn't exist...
            if (!Directory.Exists(this.messageQueuePath))
            {
                try
                {
                    Directory.CreateDirectory(this.messageQueuePath);
                }
                catch (Exception ex)
                {
                    this.logger?.LogError(ex, Resources.StorageDirectoryCommandErrorText, this.messageQueuePath, ex.Message);
                }
            }

            return Directory.Exists(this.messageQueuePath);
        }
    }
}