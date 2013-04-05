using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;

namespace Crafted.Twitter.Helpers
{

    /// <summary>
    /// Helper class for accessing config values
    /// </summary>
    internal static class ConfigHelper
    {
        public static object GetAppSetting(ConfigKey key)
        {
            return ConfigurationManager.AppSettings[key.ToString()];
        }

        public static string GetAppSettingString(ConfigKey key)
        {
            return GetAppSetting(key).ToString();
        }

        public static bool GetAppSettingBool(ConfigKey key, bool defaultValue)
        {
            return GetAppSetting<bool>(key, defaultValue);
        }

        public static long GetAppSettingLong(ConfigKey key, long defaultValue)
        {
            return GetAppSetting<long>(key, defaultValue);
        }

        public static T GetAppSetting<T>(ConfigKey key, T defaultValue)
        {
            //get the app setting value
            object appSetting = GetAppSetting(key);
            
            //if the app setting value is null then return the default
            if (appSetting == null)
                return defaultValue;

            //otherwise attempt to cast to the correct type
            Type targetType = typeof(T);
            try
            {
                if (targetType.IsSubclassOf(typeof(Enum)))
                {
                    return (T)Enum.Parse(targetType, appSetting.ToString());
                }
                else
                    return (T)Convert.ChangeType(appSetting, targetType);
            }
            catch
            {
                throw new Exception(string.Format("Requested configuration value with key '{0}' and value '{1}' cannot be converted to type '{2}'", key, appSetting, typeof(T)));
            }
        }


    }

    internal enum ConfigKey
    {
        HandlerPath,
        TwitterAPIKey,
        TwitterSecretKey,
        TwitterAccessToken,
        TwitterSecretToken,
        TwitterCacheEnabled,
        TwitterCacheDuration
    }
}