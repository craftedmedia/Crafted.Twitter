using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqToTwitter;

namespace Crafted.Twitter.Helpers
{
    /// <summary>
    /// Provides helper methods for interacting with twitter
    /// </summary>
    public static class ContextHelper
    {

        /// <summary>
        /// gets an authorizer object using the details from the config
        /// </summary>
        /// <returns></returns>
        public static ITwitterAuthorizer GetSingleUserAuth()
        {

            // configure the OAuth object
            var auth = new SingleUserAuthorizer
            {
                Credentials = new InMemoryCredentials
                {
                    ConsumerKey = ConfigHelper.GetAppSettingString(ConfigKey.TwitterAPIKey),
                    ConsumerSecret = ConfigHelper.GetAppSettingString(ConfigKey.TwitterSecretKey),
                    OAuthToken = ConfigHelper.GetAppSettingString(ConfigKey.TwitterAccessToken),
                    AccessToken = ConfigHelper.GetAppSettingString(ConfigKey.TwitterSecretToken)
                }
            };

            return auth;
        }

        /// <summary>
        /// gets a context object using single user auth
        /// </summary>
        /// <returns></returns>
        public static TwitterContext GetContext()
        {
            //Return a new instance of the twitter context using single user auth
            return new TwitterContext(GetSingleUserAuth());
        }

        /// <summary>
        /// Gets the User object associated with the currently authenticated details
        /// </summary>
        /// <returns></returns>
        public static User GetAuthenticatedUser()
        {
            TwitterContext context = GetContext();

            var accounts = from acct in context.Account
                           where acct.Type == AccountType.VerifyCredentials
                           select acct;

            Account account = accounts.SingleOrDefault();

            return account.User;
        }

    }
}
