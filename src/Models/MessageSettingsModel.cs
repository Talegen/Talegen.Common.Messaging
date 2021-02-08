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
    using System.Globalization;
    using System.Resources;

    /// <summary>
    /// This class represents message settings to be sent to create an email message.
    /// </summary>
    public class MessageSettingsModel
    {
        /// <summary>
        /// Gets or sets the sender address.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets a list of recipient addresses.
        /// </summary>
        public List<string> To { get; set; }

        /// <summary>
        /// Gets or sets the message subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the message template name to retrieve and load into the message body.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Gets or sets an optional text body content type.
        /// </summary>
        public string TextBodyContentType { get; set; }

        /// <summary>
        /// Gets or sets an optional HTML body content type.
        /// </summary>
        public string HtmlBodyContentType { get; set; }

        /// <summary>
        /// Gets or sets an optional tokens list for populating message body with token values.
        /// </summary>
        public Dictionary<string, string> Tokens { get; set; }

        /// <summary>
        /// Gets or sets the resource manager used for retrieving template content.
        /// </summary>
        public ResourceManager ResourceManager { get; set; }

        /// <summary>
        /// Gets or sets an optional locale string override for message body resource lookup.
        /// </summary>
        public CultureInfo CultureInfoOverride { get; set; }
    }
}