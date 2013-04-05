using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Crafted.Twitter.Helpers
{
    internal static class CacheHelper
    {
        /// <summary>
        /// Returns an object from the cache with the matching cache key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetFromCache(CacheKey key, string paramString = "")
        {
            if (ConfigHelper.GetAppSettingBool(ConfigKey.TwitterCacheEnabled, false))
                return HttpRuntime.Cache.Get(key.ToString() + paramString);
            else
                return null;
        }

        /// <summary>
        /// Removes an object from the cache with the matching cache key
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveFromCache(CacheKey key, string paramString = "")
        {
            if (ConfigHelper.GetAppSettingBool(ConfigKey.TwitterCacheEnabled, false))
            {
                HttpRuntime.Cache.Remove(key.ToString() + paramString);
            }
        }

        /// <summary>
        /// Adds the supplied object to the cache
        /// </summary>
        /// <param name="key">The key enum to add the data under</param>
        /// <param name="cacheData">The data to add to the cache</param>
        /// <param name="cacheLength">The timeout in minutes for the cached data</param>
        public static void AddToCache(object cacheData, int cacheLength, CacheKey key, string paramString = "")
        {
            if (ConfigHelper.GetAppSettingBool(ConfigKey.TwitterCacheEnabled, false))
            {
                DateTime cacheTimeout = DateTime.Now.AddMinutes(cacheLength);

                HttpRuntime.Cache.Add(key.ToString() + paramString, cacheData, null, cacheTimeout, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
        }

    }


    internal enum CacheKey
    {
        UserTimeline
    }
}
