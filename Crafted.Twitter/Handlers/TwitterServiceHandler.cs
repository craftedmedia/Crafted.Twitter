using System;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Routing;

using Crafted.Twitter.DataObjects;
using Crafted.Twitter.Helpers;

namespace Crafted.Twitter.Handlers
{
    public class TwitterServiceHandler : IHttpHandler, IRouteHandler
    {
        /// <summary>
        /// You will need to configure this handler in the web.config file of your 
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            
            string output;
            string path = context.Request.AppRelativeCurrentExecutionFilePath;

            switch (Path.GetFileNameWithoutExtension(path).ToLowerInvariant())
            {
                case "async-include":
                    output = BuildAsyncIncludes();
                    ResourceHelper.SetResponseType(context, path);
                    break;

                case "timelineservice":
                    output = GetTimelineData(context);
                    break;

                default:
                    output = ResourceHelper.Includes(context, path);
                    break;
            }

            context.Response.Write(output);

        }

        

        private string BuildAsyncIncludes()
        {
            StringBuilder outputBuilder = new StringBuilder();

            //get the tweet layout and wrap as js variable
            string tweetBase = string.Format("var tweetLayout = '{0}'", ResourceHelper.Includes(null, "tweet-layout.html").Replace(Environment.NewLine, ""));
            outputBuilder.AppendLine(tweetBase);

            string twitterJs = ResourceHelper.Includes(null, "twitter-builder.js");
            outputBuilder.AppendLine(twitterJs);

            return outputBuilder.ToString();
        }

        #endregion
        

        #region Service Members

        private string GetTimelineData(HttpContext context)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();

                TimelineParams inputParams = new TimelineParams();

                if (context.Request.Params["inputParameters"] != null)
                {
                    string jsonInput = HttpUtility.HtmlDecode(context.Request.Params["inputParameters"]);
                    inputParams = serializer.Deserialize<TimelineParams>(jsonInput);
                }

                if (string.IsNullOrEmpty(inputParams.ListName))
                    return serializer.Serialize(TimelineHelper.GetUserTimeline(inputParams));
                else
                    return serializer.Serialize(TimelineHelper.GetList(inputParams));
            }
            catch
            {
                context.Response.StatusCode = 500;
                return "Error retrieving timeline.";
            }

        }

        #endregion


        #region IRouteHandler Members

        /// <summary>
        /// Returns this to handle requestContext
        /// </summary>
        /// <param name="requestContext">
        /// The request Context.
        /// </param>
        /// <returns>the http handler implementation</returns>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this;
        }

        #endregion
    }
}
