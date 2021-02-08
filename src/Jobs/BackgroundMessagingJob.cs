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
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Talegen.Common.Messaging.Configuration;
    using Talegen.Common.Messaging.Properties;

    /// <summary>
    /// This class is called to process the message queue in a background process job.
    /// </summary>
    public sealed class BackgroundMessagingJob : IHostedService, IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Contains an instance of a logger.
        /// </summary>
        private readonly ILogger<BackgroundMessagingJob> logger;

        /// <summary>
        /// Contains the messaging service instance.
        /// </summary>
        private readonly BackgroundMessagingService service;

        /// <summary>
        /// Contains the messaging settings for the background service.
        /// </summary>
        private readonly MessagingSettings settings;

        /// <summary>
        /// Contains the execution count.
        /// </summary>
        private int executionCount;

        /// <summary>
        /// Contains a timer.
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Contains a value indicating if dispose has been called.
        /// </summary>
        private bool disposed;

        #endregion

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundMessagingJob" /> class.
        /// </summary>
        /// <param name="settings">Contains the messaging settings.</param>
        /// <param name="logger">Contains an optional logger interface.</param>
        public BackgroundMessagingJob(MessagingSettings settings, ILogger<BackgroundMessagingJob> logger = null)
        {
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.logger = logger;
            this.service = new BackgroundMessagingService(MessagingFactory.CreateProcessor(this.settings));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to start executing the background job timer.
        /// </summary>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the task to execute.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.logger?.LogInformation(Resources.LoggingBackgroundJobRunningText);
            this.timer = new Timer(this.DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(this.settings.QueueProcessingIntervalSeconds));

            // restore any messages from queue folder.
            this.service.RestoreQueue();

            return Task.CompletedTask;
        }

        /// <summary>
        /// This method is used to stop the execution of a background job.
        /// </summary>
        /// <param name="cancellationToken">Contains a cancellation token.</param>
        /// <returns>Returns the task result.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.logger?.LogInformation(Resources.LoggingBackgroundJobStoppingText);

            // store any messages in queue.
            this.service.StoreQueue();

            this.timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes of internal disposable objects.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(!this.disposed);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method is what executes the work to be performed.
        /// </summary>
        /// <param name="state">Contains thread state.</param>
        private void DoWork(object state)
        {
            Interlocked.Increment(ref this.executionCount);
            this.service.Process();
        }

        /// <summary>
        /// Disposes internal disposable objects.
        /// </summary>
        /// <param name="disposing">Contains a value indicating whether disposing is underway.</param>
        private void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.timer?.Dispose();
            }

            this.disposed = true;
        }

        #endregion
    }
}