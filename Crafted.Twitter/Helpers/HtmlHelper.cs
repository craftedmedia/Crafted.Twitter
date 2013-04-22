using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using LinqToTwitter;

using Crafted.Twitter.DataObjects;

namespace Crafted.Twitter.Helpers
{
    public class HtmlHelper
    {
        /// <summary>
        /// Public method to build timeline output to be consumed by page.
        /// </summary>
        /// <param name="tweetCount"></param>
        /// <param name="showReplies"></param>
        /// <param name="includeRetweets"></param>
        /// <param name="screenName"></param>
        /// <param name="listName"></param>
        /// <param name="CssClass"></param>
        /// <param name="isAsync"></param>
        /// <returns>A string containing the html to output to the page</returns>
        public static string RenderTimelineIncludes(int tweetCount, bool showReplies, bool includeRetweets, string screenName = null, string listName = null, string CssClass = "tweet-container", bool isAsync = false)
        {

            TimelineParams inputParams = new TimelineParams();
            inputParams.TweetCount = tweetCount;
            inputParams.ShowReplies = showReplies;
            inputParams.ScreenName = screenName;
            inputParams.ListName = listName;
            inputParams.IncludeRetweets = includeRetweets;

            return RenderTimelineIncludes(inputParams, CssClass, isAsync);
        }

        internal static string RenderTimelineIncludes(TimelineParams inputParams, string CssClass = "tweet-container", bool isAsync = false)
        {

            RoutingHelper.RegisterRoutes();

            if (isAsync)
                return BuildAsyncTimelineOutput(inputParams, CssClass);
            else
                return BuildTimelineOutput(inputParams, CssClass);
        }

        internal static string BuildAsyncTimelineOutput(TimelineParams inputParams, string CssClass)
        {
            string asyncBaseHtml = ResourceHelper.Includes(null, "async-base.html");
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            asyncBaseHtml = string.Format(asyncBaseHtml, VirtualPathUtility.ToAbsolute(ConfigHelper.GetAppSettingString(ConfigKey.HandlerPath, Constants.Configuration.HandlerPath)), serializer.Serialize(inputParams), CssClass);

            return asyncBaseHtml;
        }

        /// <summary>
        /// Gets a timeline collection based on the supplied parameters and builds the html for output
        /// </summary>
        /// <param name="tweetCount"></param>
        /// <param name="showReplies"></param>
        /// <param name="screenName"></param>
        /// <param name="CssClass"></param>
        /// <returns></returns>
        public static string BuildTimelineOutput(TimelineParams inputParams, string CssClass = "tweet-container")
        {

            //string htmlBase = ResourceHelper.Includes(null, "tweet-layout.html");
            //Regex placeHolderExp = new Regex(@"(?<=\{)[\w\.]+(:\w+)?(?=\})");
            
            StringBuilder tweetBuilder = new StringBuilder();

            try
            {

                tweetBuilder.AppendLine(string.Format("<div class='{0}'>", CssClass));
                tweetBuilder.AppendLine("<ul>");

                List<Status> tweets;

                if (string.IsNullOrEmpty(inputParams.ListName))
                    tweets = TimelineHelper.GetUserTimeline(inputParams);
                else
                    tweets = TimelineHelper.GetList(inputParams);


                if (tweets != null && tweets.Count > 0)
                {
                    foreach (Status tweet in tweets)
                    {
                        DateTime tweetCreated = tweet.CreatedAt.ToLocalTime();

                        //string tweetOutput = htmlBase;
                        //Match match = placeHolderExp.Match(tweetOutput);
                        //while (match != null)
                        //{
                        //    MapTweetToHtml(tweet, tweetOutput, match.Value);
                        //    match = match.NextMatch();
                        //}

                        tweetBuilder.AppendLine("<li>");
                        tweetBuilder.AppendLine("<div class='user-container'>");
                        tweetBuilder.AppendLine(string.Format("<a class='user-link' href='{0}{1}' target='_blank'>", Constants.Configuration.TwitterUrl, tweet.User.Identifier.ScreenName));
                        tweetBuilder.AppendLine(string.Format("<img class='user-image' src='{0}' />", tweet.User.ProfileImageUrl));
                        tweetBuilder.AppendLine("</a>");
                        tweetBuilder.AppendLine("</div>");
                        tweetBuilder.AppendLine("<div class='tweet-container'>");
                        tweetBuilder.AppendLine(string.Format("<a class='tweet-screenName' href='{0}{1}' target='_blank'>{1}</a>", Constants.Configuration.TwitterUrl, tweet.User.Identifier.ScreenName));
                        tweetBuilder.AppendLine(string.Format("<p class='tweet-content'>{0}</p>", ParseTweetText(tweet.Text)));
                        tweetBuilder.AppendLine(string.Format("<span class='tweet-date'>{0}</span>", tweetCreated.ToString("dd MMM yyyy")));
                        tweetBuilder.AppendLine(string.Format("<span class='tweet-time'>{0}</span>", tweetCreated.ToString("hh:mm:ss")));
                        tweetBuilder.AppendLine(string.Format("<span class='tweet-time-passed'>{0}</span>", GetTimePassed(tweetCreated)));
                        tweetBuilder.AppendLine("</div>");
                        tweetBuilder.AppendLine("</li>");
                    }
                }

                tweetBuilder.AppendLine("</ul>");
                tweetBuilder.AppendLine("</div>");
            
            }
            catch
            {
                return "<p class='service-error'>Unable to contact twitter service.</p>";
            }

            return tweetBuilder.ToString();

        }

        private static void MapTweetToHtml(Status tweet, string uiMap, string match)
        {
            string[] matchParts = match.Split(':');

            
        }

        // Adds tags inside tweet text as required
        private static string ParseTweetText(string tweetText)
        {
            string parsedTweet = tweetText;

            //add links to twitter names
            if (parsedTweet.Contains("@"))
            {
                parsedTweet = Regex.Replace(parsedTweet, Constants.RegularExpressions.TweetUserExpression, new MatchEvaluator(s => string.Format("<a href='{0}{1}' target='_blank'>{2}</a>", Constants.Configuration.TwitterUrl, s.Value.Substring(1), s.Value)));
            }
            //add links to hash tags
            if (parsedTweet.Contains("#"))
            {
                parsedTweet = Regex.Replace(parsedTweet, Constants.RegularExpressions.TweetHashTagExpression, new MatchEvaluator(s => string.Format("<a href='{0}search?q=%23{1}&src=hash' target='_blank'>{2}</a>", Constants.Configuration.TwitterUrl, s.Value.Substring(1), s.Value)));
            }
            //make links in tweet text into html links
            if (parsedTweet.Contains("http://"))
            {
                parsedTweet = Regex.Replace(parsedTweet, Constants.RegularExpressions.TweetLinkExpression, new MatchEvaluator(s => string.Format("<a href='{0}' target='_blank'>{0}</a>", s.Value)));
            }

            return parsedTweet;
        }

        /// <summary>
        /// Returns a string representing the 
        /// </summary>
        /// <param name="pastDate"></param>
        /// <returns></returns>
        private static string GetTimePassed(DateTime pastDate)
        {
            string timeString = "";

            TimeSpan timePassed = DateTime.Now.Subtract(pastDate);

            if (timePassed < new TimeSpan(0, 0, 1, 0))
                timeString = "Less than a minute passed";
            else if (timePassed < new TimeSpan(0, 1, 0, 0))
                timeString = string.Format("{0} minute{1} ago", timePassed.Minutes, timePassed.Minutes == 1 ? "" : "s");
            else if (timePassed < new TimeSpan(1, 0, 0, 0))
                timeString = string.Format("{0} hour{1} ago", timePassed.Hours, timePassed.Hours == 1 ? "" : "s");
            else
                timeString = string.Format("{0} day{1} ago", timePassed.Days, timePassed.Days == 1 ? "" : "s");

            return timeString;
        }


    }
}
