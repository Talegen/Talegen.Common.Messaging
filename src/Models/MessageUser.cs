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
    using System;
    using System.Globalization;
    using System.Net.Mail;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class contains extended properties of the identity user record for assisting e-mail template insertion.
    /// </summary>
    public class MessageUser
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's first name from claims.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name from claims.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the user's profile image URL from claims.
        /// </summary>
        public Uri PictureUrl { get; set; }

        /// <summary>
        /// Gets or sets the user's time zone.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the user locale string.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets the user's culture info object based on the locale.
        /// </summary>
        public CultureInfo CultureInfo
        {
            get
            {
                CultureInfo result = CultureInfo.DefaultThreadCurrentCulture;

                if (!string.IsNullOrWhiteSpace(this.Locale))
                {
                    try
                    {
                        result = CultureInfo.GetCultureInfo(this.Locale);
                    }
                    catch
                    {
                        result = CultureInfo.GetCultureInfo(LocaleExtensions.DefaultLanguageCode);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the user's full name.
        /// </summary>
        public string FullName
        {
            get
            {
                string result = this.FirstName;
                string lastName = this.LastName;

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    result += " " + lastName;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a new mail address for the specified email user.
        /// </summary>
        public MailAddress MailAddress => new MailAddress(this.Email, this.FullName.Trim());
    }
}