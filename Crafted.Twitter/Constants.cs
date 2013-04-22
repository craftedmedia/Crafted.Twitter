using System;
using System.Collections.Generic;
using System.Text;

namespace Crafted.Twitter
{
    internal class Constants
    {

        internal class Configuration
        {

            public const string TwitterUrl = "http://www.twitter.com/";
            public const string HandlerPath = "~/TwitterHandler/";

        }
        
        internal class RegularExpressions
        {

            public const string TweetUserExpression = @"(?<!\S)@[a-zA-Z0-9_]{1,15}";
            public const string TweetHashTagExpression = @"(?<!\S)#[a-zA-Z]{1}\w+";
            public const string TweetLinkExpression = @"(?<!\S)http://\S+(?!\S)";

        }
    }
}
