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
    using Microsoft.Extensions.DependencyInjection;
    using Talegen.Common.Core.Errors;
    using Talegen.Common.Messaging.Configuration;
    using Talegen.Common.Messaging.Jobs;

    /// <summary>
    /// This class contains extension methods for adding messaging middleware startup functionality.
    /// </summary>
    public static class StartupExtensions
    {
        /// <summary>
        /// Adds the Messaging service queue processing job and <see cref="MessagingSettings" /> in as an injectable singleton.
        /// </summary>
        /// <param name="services">Contains the services collection to add the provider to.</param>
        /// <param name="messageSettings">Contains the <see cref="MessagingSettings" /> object for settings.</param>
        /// <returns>Returns the modified services collection.</returns>
        public static IServiceCollection AddMessaging(this IServiceCollection services, MessagingSettings messageSettings)
        {
            // add the MessageSettings as singlton
            services.AddSingleton(service => messageSettings);

            // setup messaging processor. This is what will process any messages on the queue. Instatiated when the background messaging job is run.
            services.AddTransient((service) => MessagingFactory.CreateProcessor(messageSettings, service.GetService<IErrorManager>()));

            // This can be injected into any code that requires it via IMessageSender interface. It is what puts messages on the queue to be processed.
            services.AddTransient((service) => MessagingFactory.CreateSender(messageSettings));

            // add the background messaging job to process the messaging queue.
            services.AddHostedService<BackgroundMessagingJob>();

            return services;
        }
    }
}