using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Crafted.Twitter.Helpers;
using Crafted.Twitter.DataObjects;

namespace Crafted.Twitter.Controls
{
    public class Timeline : WebControl
    {

#region "Public properties"

        public int TweetCount
        {
            get
            {
                if (ViewState["TweetCount"] == null)
                    return 5;
                else
                    return (int)ViewState["TweetCount"];
            }
            set
            {
                ViewState["TweetCount"] = value;
            }
        }

        public bool ShowReplies
        {
            get
            {
                if (ViewState["ShowReplies"] == null)
                    return false;
                else
                    return (bool)ViewState["ShowReplies"];
            }
            set
            {
                ViewState["ShowReplies"] = value;
            }
        }

        public bool ShowRetweets
        {
            get
            {
                if (ViewState["ShowRetweets"] == null)
                    return false;
                else
                    return (bool)ViewState["ShowRetweets"];
            }
            set
            {
                ViewState["ShowRetweets"] = value;
            }
        }

        public string ScreenName
        {
            get
            {
                if (ViewState["ScreenName"] == null)
                    return null;
                else
                    return ViewState["ScreenName"].ToString();
            }
            set
            {
                ViewState["ScreenName"] = value;
            }
        }

        public string ListName
        {
            get
            {
                if (ViewState["ListName"] == null)
                    return null;
                else
                    return ViewState["ListName"].ToString();
            }
            set
            {
                ViewState["ListName"] = value;
            }
        }

        public bool IsAsync
        {
            get
            {
                if (ViewState["IsAsync"] == null)
                    return false;
                else
                    return (bool)ViewState["IsAsync"];
            }
            set
            {
                ViewState["IsAsync"] = value;
            }
        }


#endregion

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            TimelineParams inputParams = new TimelineParams();
            inputParams.TweetCount = TweetCount;
            inputParams.ShowReplies = ShowReplies;
            inputParams.ScreenName = ScreenName;
            inputParams.IncludeRetweets = ShowRetweets;
            inputParams.ListName = ListName;

            writer.Write(HtmlHelper.RenderTimelineIncludes(inputParams, CssClass, IsAsync));
        }

    }
}
