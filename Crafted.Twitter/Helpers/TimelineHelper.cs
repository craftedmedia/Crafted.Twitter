using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LinqToTwitter;
using Crafted.Twitter.DataObjects;

namespace Crafted.Twitter.Helpers
{
    /// <summary>
    /// Provides helper methods for interacting with the twitter timeline
    /// </summary>
    public static class TimelineHelper
    {

        /// <summary>
        /// calls the twitter api to get timeline results for a user
        /// </summary>
        /// <returns></returns>
        public static List<Status> GetUserTimeline(TimelineParams inputParams)
        {
            // check the cache for existing data
            List<Status> tweets = (List<Status>)CacheHelper.GetFromCache(CacheKey.UserTimeline, BuildParamString(inputParams));
            if (tweets == null)
            {
                tweets = new List<Status>();

                ulong lastTweetId = 0;
                string searchScreenName;

                // a valid context
                TwitterContext context = ContextHelper.GetContext();

                // if a screen name has not been provided then get the current users screen name and last tweet id
                if (string.IsNullOrEmpty(inputParams.ScreenName))
                {
                    User authenticatedUser = ContextHelper.GetAuthenticatedUser();
                    searchScreenName = authenticatedUser.Identifier.ScreenName;

                    if (authenticatedUser.Status != null)
                        lastTweetId = ulong.Parse(authenticatedUser.Status.StatusID);
                }
                // if a screen name has been provided then get the user and store thier last tweet id
                else
                {
                    searchScreenName = inputParams.ScreenName;

                    var users = from u in context.User
                                where u.Type == UserType.Show
                                    && u.ScreenName == inputParams.ScreenName
                                select u;

                    User user = users.SingleOrDefault();
                    lastTweetId = ulong.Parse(user.Status.StatusID);
                }

                try
                {
                    // Here it is necesary to get the tweets in chunks and add the results to the tweets list
                    // as the number of results returned from the api call is effectively up to the count supplied
                    // due to limitations in the twitter REST api

                    //set a max loops value as a backup
                    int maxloops = 5, count = 0;

                    while (tweets.Count < inputParams.TweetCount && count < maxloops)
                    {
                        //increment the count
                        count++;

                        List<Status> chunkList;

                        chunkList = (from tweet in context.Status
                                     where tweet.Type == StatusType.User
                                         && tweet.ExcludeReplies == !inputParams.ShowReplies
                                         && tweet.IncludeRetweets == inputParams.IncludeRetweets
                                         && tweet.Count == inputParams.TweetCount
                                         && tweet.ScreenName == searchScreenName
                                         && tweet.MaxID == lastTweetId
                                     orderby tweet.CreatedAt descending
                                     select tweet).ToList();

                        // add the chunk items into the tweet list
                        foreach (Status tweet in chunkList)
                        {
                            if (tweets.Count < inputParams.TweetCount)
                            {
                                tweets.Add(tweet);
                            }
                        }

                        // store the last tweet recieved so we dont get the same ones next loop (-1 to prevent re-retrieving the last tweet)
                        if (tweets.Count > 0)
                            lastTweetId = ulong.Parse(chunkList[chunkList.Count - 1].StatusID) - 1;
                    }

                    CacheHelper.AddToCache(tweets, ConfigHelper.GetAppSetting<int>(ConfigKey.TwitterCacheDuration, 20), CacheKey.UserTimeline, BuildParamString(inputParams));

                }
                catch (TwitterQueryException ex)
                {

                }
            }

            return tweets;
        }

        /// <summary>
        /// calls the twitter api to get tweets form a list
        /// </summary>
        /// <returns></returns>
        public static List<Status> GetList(TimelineParams inputParams)
        {
            // check the cache for existing data
            List<Status> tweets = (List<Status>)CacheHelper.GetFromCache(CacheKey.UserTimeline, BuildParamString(inputParams));
            if (tweets == null)
            {
                tweets = new List<Status>();

                string searchScreenName = inputParams.ScreenName;
                ulong lastTweetId = 0;

                // a valid context
                TwitterContext context = ContextHelper.GetContext();

                // if a screen name has not been provided then get the current users screen name
                if (string.IsNullOrEmpty(inputParams.ScreenName))
                {
                    User authenticatedUser = ContextHelper.GetAuthenticatedUser();
                    searchScreenName = authenticatedUser.Identifier.ScreenName;
                }
                
                try
                {
                    // Here it is necesary to get the tweets in chunks and add the results to the tweets list
                    // as the number of results returned from the api call is effectively up to the count supplied
                    // due to limitations in the twitter REST api

                    //set a max loops value as a backup
                    int maxloops = 5, count = 0;

                    while (tweets.Count < inputParams.TweetCount && count < maxloops)
                    {
                        //increment the count
                        count++;

                        List<Status> chunkList;

                        if (lastTweetId == 0)
                        {

                            chunkList = (from list in context.List
                                         where list.Type == ListType.Statuses
                                             && list.OwnerScreenName == searchScreenName
                                             && list.Slug == inputParams.ListName // name of list to get statuses for
                                             && list.Count == inputParams.TweetCount
                                         select list)
                                         .First().Statuses;
                        }
                        else
                        {
                            chunkList = (from list in context.List
                                              where list.Type == ListType.Statuses
                                                  && list.OwnerScreenName == searchScreenName
                                                  && list.Slug == inputParams.ListName // name of list to get statuses for
                                             && list.Count == inputParams.TweetCount
                                                  && list.MaxID == lastTweetId
                                              select list)
                                         .First().Statuses;
                        }
                        // add the chunk items into the tweet list
                        foreach (Status tweet in chunkList)
                        {
                            if (tweets.Count < inputParams.TweetCount && (inputParams.IncludeRetweets || (tweet.RetweetedStatus == null || tweet.RetweetedStatus.User == null)))
                            {
                                tweets.Add(tweet);
                            }
                        }

                        // store the last tweet recieved so we dont get the same ones next loop (-1 to prevent re-retrieving the last tweet)
                        if (tweets.Count > 0)
                            lastTweetId = ulong.Parse(chunkList[chunkList.Count - 1].StatusID) - 1;
                    }

                    CacheHelper.AddToCache(tweets, ConfigHelper.GetAppSetting<int>(ConfigKey.TwitterCacheDuration, 20), CacheKey.UserTimeline, BuildParamString(inputParams));

                }
                catch (TwitterQueryException ex)
                {

                }
            }

            return tweets;
        }

        /// <summary>
        /// Builds a parameter string to use as part of the cache key
        /// </summary>
        /// <returns></returns>
        private static string BuildParamString(TimelineParams inputParams)
        {
            return string.Format("TweetCount={0}&ShowReplies={1}&ScreenName={2}&IncludeRetweets={3}&ListName={4}", inputParams.TweetCount, inputParams.ShowReplies, inputParams.ScreenName, inputParams.IncludeRetweets, inputParams.ListName);
        }

    }
}
