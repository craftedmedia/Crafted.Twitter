using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Extension method for the html helper to output a twitter timeline, outputs as raw html
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="tweetCount"></param>
        /// <param name="showReplies"></param>
        /// <param name="includeRetweets"></param>
        /// <param name="screenName"></param>
        /// <param name="listName"></param>
        /// <param name="CssClass"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        public static MvcHtmlString RenderTwitterFeed(this HtmlHelper helper, int tweetCount, bool showReplies, bool includeRetweets, bool isAsync = false, string screenName = null, string listName = null, string CssClass = "tweet-container")
        {
            return MvcHtmlString.Create(Crafted.Twitter.Helpers.HtmlHelper.RenderTimelineIncludes(tweetCount, showReplies, includeRetweets, screenName, listName, CssClass, isAsync));
        }

    }
}
