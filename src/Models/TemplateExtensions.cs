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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Talegen.Common.Messaging.Templates;

    /// <summary>
    /// This class contains template rendering methods for messages and text within the application.
    /// </summary>
    public static class TemplateExtensions
    {
        /// <summary>
        /// This method is used to load a template message body from a template file.
        /// </summary>
        /// <param name="templateFolderPath">Contains the path to the template folder.</param>
        /// <param name="templateFileName">Contains the file name of the template to load.</param>
        /// <param name="cultureInfoOverride">Contains a culture info override to render a specific template language.</param>
        /// <returns>Returns the message body of the template.</returns>
        public static string LoadTemplate(string templateFolderPath, string templateFileName, CultureInfo cultureInfoOverride = null)
        {
            // default to English if no language specified.
            var language = cultureInfoOverride != null ? cultureInfoOverride.Name : "en-US";
            string content = string.Empty;
            FileInfo templateFileInfo = RetrieveTemplateInfo(templateFolderPath, language, templateFileName);

            if (!templateFileInfo.Exists && language != "en-US")
            {
                templateFileInfo = RetrieveTemplateInfo(templateFolderPath, "en-US", templateFileName);
            }

            if (templateFileInfo.Exists)
            {
                using (StreamReader reader = templateFileInfo.OpenText())
                {
                    content = reader.ReadToEnd();
                }
            }

            return content;
        }

        /// <summary>
        /// This method is used to return both text and HTML versions of a message template.
        /// </summary>
        /// <param name="templateFolderPath">Contains the template folder path.</param>
        /// <param name="templateName">Contains the template name prefix without extension.</param>
        /// <param name="cultureInfoOverride">Contains an optional culture override.</param>
        /// <returns>Returns both text and HTML versions of a specified template.</returns>
        public static Tuple<string, string> LoadTemplates(string templateFolderPath, string templateName, CultureInfo cultureInfoOverride = null)
        {
            // default to English if no language specified.
            string textContent = LoadTemplate(templateFolderPath, templateName + ".txt", cultureInfoOverride);
            string htmlContent = LoadTemplate(templateFolderPath, templateName + ".htm", cultureInfoOverride);
            return new Tuple<string, string>(textContent, htmlContent);
        }

        /// <summary>
        /// This method is used to render a dictionary of token values to the message body of a template.
        /// </summary>
        /// <param name="messageBodyText">Contains the message body text of the template to render.</param>
        /// <param name="tokensList">Contains the token value dictionary used to insert values into the message body.</param>
        /// <returns>Returns the message body with token values found in the tokens list.</returns>
        public static string RenderTemplateBody(string messageBodyText, Dictionary<string, string> tokensList)
        {
            return messageBodyText.ReplaceTokens(tokensList);
        }

        /// <summary>
        /// This method is used to render token values into a specific template file body and return the results.
        /// </summary>
        /// <param name="templateFolderPath">Contains the path to the template folder.</param>
        /// <param name="templateFileName">Contains the file name of the template to load.</param>
        /// <param name="tokensList">Contains the token value dictionary used to insert values into the message body.</param>
        /// <param name="cultureInfoOverride">Contains a culture info override to render a specific template language.</param>
        /// <returns>Returns the message body of the specified template with token values found in the tokens list inserted where noted.</returns>
        public static string RenderTemplateFile(string templateFolderPath, string templateFileName, Dictionary<string, string> tokensList = null, CultureInfo cultureInfoOverride = null)
        {
            return RenderTemplateBody(LoadTemplate(templateFolderPath, templateFileName, cultureInfoOverride), tokensList);
        }

        /// <summary>
        /// This method is used to render token values into a specific template file body and return the results.
        /// </summary>
        /// <param name="templateFolderPath">Contains the path to the template folder.</param>
        /// <param name="templateName">Contains the file name of the template to load.</param>
        /// <param name="tokensList">Contains the token value dictionary used to insert values into the message body.</param>
        /// <param name="cultureInfoOverride">Contains a culture info override to render a specific template language.</param>
        /// <returns>Returns the message body of the specified template with token values found in the tokens list inserted where noted.</returns>
        public static string RenderTemplate(string templateFolderPath, string templateName, Dictionary<string, string> tokensList = null, CultureInfo cultureInfoOverride = null)
        {
            return RenderTemplateFile(templateFolderPath, templateName + ".txt", tokensList, cultureInfoOverride);
        }

        /// <summary>
        /// This method is used to load and render both text and HTML templates for a specified template name.
        /// </summary>
        /// <param name="templateFolderPath">Contains the template folder path.</param>
        /// <param name="templateName">Contains the template name prefix without extension.</param>
        /// <param name="tokensList">Contains the token value dictionary used to insert values into the message body.</param>
        /// <param name="cultureInfoOverride">Contains an optional culture override.</param>
        /// <returns>Returns both text and HTML versions of a specified template with token values found in the tokens list inserted where noted.</returns>
        public static Tuple<string, string> RenderTemplates(string templateFolderPath, string templateName, Dictionary<string, string> tokensList = null, CultureInfo cultureInfoOverride = null)
        {
            Tuple<string, string> templateBodies = LoadTemplates(templateFolderPath, templateName, cultureInfoOverride);
            string textResult = RenderTemplateBody(templateBodies.Item1, tokensList);
            string htmlResult = RenderTemplateBody(templateBodies.Item2, tokensList);
            return new Tuple<string, string>(textResult, htmlResult);
        }

        /// <summary>
        /// This method is used to build a template path and retrieve file information.
        /// </summary>
        /// <param name="templateFolderPath">Contains the template folder path.</param>
        /// <param name="language">Contains the template language to load.</param>
        /// <param name="templateFileName">Contains the template file name.</param>
        /// <returns>Returns the File info object for the template path.</returns>
        private static FileInfo RetrieveTemplateInfo(string templateFolderPath, string language, string templateFileName)
        {
            return new FileInfo(Path.Combine(templateFolderPath, language, templateFileName));
        }
    }
}