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

namespace Talegen.Common.Messaging.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// This class contains contextual values for sending messages.
    /// </summary>
    public class MessagingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext" /> class.
        /// </summary>
        public MessagingContext()
            : this(string.Empty, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingContext" /> class.
        /// </summary>
        /// <param name="from">Contains the from address.</param>
        /// <param name="tokenValues">Contains optional seed token key values.</param>
        public MessagingContext(string from, Dictionary<string, string> tokenValues = null)
        {
            this.From = from;
            this.TokenValues = tokenValues ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the e-mail address of the sending agent.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets a dictionary of token key value pairs for inserting into the message body of the e-mail.
        /// </summary>
        public Dictionary<string, string> TokenValues { get; }
    }
}