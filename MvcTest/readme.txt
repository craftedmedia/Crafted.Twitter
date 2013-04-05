This library contains a web form control and an MVC HtmlHelper extension to add a twitter feed to your projects, it contains built in cacheing and both synchronous and asynchronous options.

To implement this you will have to add the following app settings;

<!-- Crafted.Twitter settings -->
<add key="HandlerPath" value="~/TwitterHandler/"/>
<add key="TwitterAPIKey" value="" />
<add key="TwitterSecretKey" value="" />
<add key="TwitterAccessToken" value="" />
<add key="TwitterSecretToken" value="" />
<add key="TwitterCacheEnabled" value="true"/>
<add key="TwitterCacheDuration" value="20"/>

You will need to generate the key and token values from your twitter account.



Web forms control ref:

<Crafted.Twitter:Timeline id="twitFeed" runat="server" CssClass="twitter-container" TweetCount="1" ShowReplies="true" ShowReTweets="false" ScreenName="" ListName="" 
IsAsync="true"></twit:Timeline>

None of these parameters are required, if the screenname is not specified it will fetch tweets associated with the authentication details.



MVC include:

@Html.Raw(Crafted.Twitter.Helpers.HtmlHelper.RenderTimelineIncludes(5, true, false, null, "tweet-container", true))

There are two overloads, for the first the parameters are as follows;
int tweetCount
bool showReplies
bool includeRetweets
string screenName = null
string CssClass = "tweet-container"
bool isAsync = false

If the screenname property is left as null then it will fetch tweets associated with the authentication details.


For the second the parameters are as follows;

int tweetCount
bool includeRetweets
string screenName
string listName
string CssClass = "tweet-container"
bool isAsync = false 
If the screenname property is left as null then it will fetch tweets associated with the authentication details.


The markup that the twitter feed outputs as is wrapped in a <ul> and is as follows;
<div class="tweet-container">
    <ul>
        <li>
            <div class="user-container">
                <a class="user-link" target="_blank" href="http://www.twitter.com/{User.Identifier.ScreenName}">
                    <img class="user-image" src="{User.ProfileImageUrl}">
                </a>
            </div>
            <div class="tweet-container">
                <a target="_blank" href="http://www.twitter.com/{User.Identifier.ScreenName}" class="tweet-screenName">{User.Identifier.ScreenName}</a>
                <p class="tweet-content">{Text:RichText}</p>
                <span class="tweet-date">{CreatedAt:Date}</span>
                <span class="tweet-time-passed">{CreatedAt:TimePassed}</span>
            </div>
        </li>
    </ul>
</div>
