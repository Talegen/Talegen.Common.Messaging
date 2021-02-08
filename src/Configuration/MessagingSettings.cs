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

namespace Talegen.Common.Messaging.Configuration
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains an enumerated list of persistence storage methods.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessagingType
    {
        /// <summary>
        /// Uses SMTP library.
        /// </summary>
        Smtp,

        /// <summary>
        /// Uses the Send grid API.
        /// </summary>
        SendGridApi,

        /// <summary>
        /// Available for implementing a fake in-memory messenger.
        /// </summary>
        Simulated
    }

    /// <summary>
    /// This class contains messaging settings.
    /// </summary>
    public class MessagingSettings
    {
        /// <summary>
        /// Gets or sets the type of the message system.
        /// </summary>
        /// <value>The type of the messaging.</value>
        public MessagingType MessagingType { get; set; }

        /// <summary>
        /// Gets or sets the support e-mail address.
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// Gets or sets the host name.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets or sets the host port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection shall use SSL.
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Gets or sets static Token values.
        /// </summary>
        /// <value>The static token values.</value>
        public Dictionary<string, string> TokenValues { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the interval in which queued messages are processed
        /// </summary>
        public int QueueProcessingIntervalSeconds { get; set; } = 5;
    }
}