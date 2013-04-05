using System;
using System.Collections.Generic;
using System.Text;

namespace Crafted.Twitter.DataObjects
{
    public class TimelineParams
    {
        private int tweetCount;
        private bool showReplies;
        private bool includeRetweets;
        private string screenName;
        private string listName;

        //The number of tweets to return when querying, defaults to 5
        public int TweetCount
        {
            get { return tweetCount > 0 ? tweetCount : 5; }
            set { tweetCount = value; }
        }
        //Whether to return replies in the results
        public bool ShowReplies
        {
            get { return showReplies; }
            set { showReplies = value; }
        }
        //Whether to return retweets in the results
        public bool IncludeRetweets
        {
            get { return includeRetweets; }
            set { includeRetweets = value; }
        }
        //the screen name to request tweets for, if left blank will get tweets for the authenticated screenname
        public string ScreenName
        {
            get { return screenName; }
            set { screenName = value; }
        }
        //the list name to request tweets from
        public string ListName
        {
            get { return listName; }
            set { listName = value; }
        }
    }
}
