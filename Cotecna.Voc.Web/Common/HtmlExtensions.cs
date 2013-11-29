using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Cotecna.Voc.Web.Common
{
    /// <summary>
    /// This class contains some extension methods for personalizing the views
    /// </summary>
    public static class HtmlExtensions
    {
        #region LocalValidationSummary
        /// <summary>
        /// Generate my validation summary to adapt to our desings
        /// </summary>
        /// <param name="htmlHelper">htmlHelper</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString LocalValidationSummary(this HtmlHelper htmlHelper)
        {
            MvcHtmlString result = null;
            if (htmlHelper.ViewData.ModelState.IsValid) return result;

            int errors = 0;

            foreach (ModelState modelState in htmlHelper.ViewData.ModelState.Values)
            {
                errors += modelState.Errors.Count;
            }

            if (errors == 1)
                result = LocalValidationSummary(htmlHelper, Resources.Common.OneError);
            else if (errors > 1)
                result = LocalValidationSummary(htmlHelper, string.Format(Resources.Common.SomeErrors, errors));

            return result;
        }

        /// <summary>
        /// Generate my validation summary to adapt to our desings
        /// </summary>
        /// <param name="htmlHelper">htmlHelper</param>
        /// <param name="message">message for summary</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString LocalValidationSummary(this HtmlHelper htmlHelper, string message)
        {
            // Nothing to do if there aren't any errors
            if (htmlHelper.ViewData.ModelState.IsValid)
            {
                return null;
            }

            string messageSpan;

            TagBuilder spanTag = new TagBuilder("h5");
            if (!string.IsNullOrEmpty(message))
                spanTag.SetInnerText(message);

            messageSpan = spanTag.ToString(TagRenderMode.Normal) + Environment.NewLine;

            StringBuilder htmlSummary = new StringBuilder();
            TagBuilder unorderedList = new TagBuilder("ul");

            foreach (ModelState modelState in htmlHelper.ViewData.ModelState.Values)
            {
                foreach (ModelError modelError in modelState.Errors)
                {
                    string errorText = GetUserErrorMessageOrDefault(modelError, null /* modelState */);
                    if (!String.IsNullOrEmpty(errorText))
                    {
                        TagBuilder listItem = new TagBuilder("li");
                        listItem.SetInnerText(errorText);
                        htmlSummary.AppendLine(listItem.ToString(TagRenderMode.Normal));
                    }
                }
            }

            unorderedList.InnerHtml = htmlSummary.ToString();

            TagBuilder divContent = new TagBuilder("div");
            divContent.MergeAttribute("class", "error");
            divContent.InnerHtml = messageSpan + unorderedList.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(divContent.ToString(TagRenderMode.Normal));

        }

        private static string GetUserErrorMessageOrDefault(ModelError error, ModelState modelState)
        {
            if (!String.IsNullOrEmpty(error.ErrorMessage))
            {
                return error.ErrorMessage;
            }
            if (modelState == null)
            {
                return null;
            }

            string attemptedValue = (modelState.Value != null) ? modelState.Value.AttemptedValue : null;
            return String.Format(CultureInfo.CurrentCulture, Resources.Common.NotValidValue, attemptedValue);
        }

        #endregion
        
    }
}