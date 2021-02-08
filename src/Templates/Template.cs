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

namespace Talegen.Common.Messaging.Templates
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Contains an enumerated list of template types.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TemplateType
    {
        /// <summary>
        /// Message Content Type
        /// </summary>
        Message
    }

    /// <summary>
    /// This class represents a content template within the application.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Gets or sets the template identifier.
        /// </summary>
        /// <value>The template identifier.</value>
        [Key]
        public Guid TemplateId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets a unique identity for the stored content template.
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string TemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the template content type.
        /// </summary>
        [Column(TypeName = "varchar(20)")]
        public TemplateType TemplateType { get; set; } = TemplateType.Message;

        /// <summary>
        /// Gets or sets the related language code for the template.
        /// </summary>
        [MaxLength(5)]
        public string LanguageCode { get; set; }

        /// <summary>
        /// Gets or sets the image binary data.
        /// </summary>
        public string Content { get; set; }
    }
}