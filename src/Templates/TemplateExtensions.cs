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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Talegen.Common.Messaging.Models;

    /// <summary>
    /// This class contains extensions to support the template engine.
    /// </summary>
    public static class TemplateExtensions
    {
        /// <summary>
        /// This method is used to replace $token$ values within the content string with matching key values from the dictionary specified.
        /// </summary>
        /// <param name="content">Contains the string to parse.</param>
        /// <param name="tokenValues">Contains a dictionary of string values to replace.</param>
        /// <returns>Returns the original content string with all tokens found replaced.</returns>
        public static string ReplaceTokens(this string content, Dictionary<string, string> tokenValues)
        {
            string result = content;

            if (tokenValues != null)
            {
                if (!tokenValues.ContainsKey("DATETIME"))
                {
                    tokenValues.Add("DATETIME", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }

                if (!tokenValues.ContainsKey("DATE"))
                {
                    tokenValues.Add("DATE", DateTime.UtcNow.ToShortDateString());
                }

                if (!tokenValues.ContainsKey("TIME"))
                {
                    tokenValues.Add("TIME", DateTime.UtcNow.ToShortTimeString());
                }

                result = tokenValues.Aggregate(result, (current, item) => current.Replace("$" + item.Key.ToUpperInvariant() + "$", item.Value, StringComparison.InvariantCultureIgnoreCase));
            }

            return result;
        }

        /// <summary>
        /// Replaces the matching token identities with <see cref="MessageUser" /> property values.
        /// </summary>
        /// <param name="content">The content to parse for tokens.</param>
        /// <param name="user">The <see cref="MessageUser" /> object used to populate values.</param>
        /// <returns>Returns the <paramref name="content" /> with updated token values replaced with matching property values.</returns>
        public static string ReplaceUserTokens(this string content, MessageUser user)
        {
            string result = content;
            result = result.Replace(TemplateTokens.UserId, user.UserId);
            result = result.Replace(TemplateTokens.UserEmail, user.Email);
            result = result.Replace(TemplateTokens.UserName, user.UserName);
            result = result.Replace(TemplateTokens.FirstName, user.FirstName);
            result = result.Replace(TemplateTokens.LastName, user.LastName);
            result = result.Replace(TemplateTokens.UserPictureUrl, user.PictureUrl.ToString());
            result = result.Replace(TemplateTokens.TimeZone, user.TimeZone);
            result = result.Replace(TemplateTokens.Locale, user.Locale);
            result = result.Replace(TemplateTokens.FullName, user.FullName);
            return result;
        }
    }
}