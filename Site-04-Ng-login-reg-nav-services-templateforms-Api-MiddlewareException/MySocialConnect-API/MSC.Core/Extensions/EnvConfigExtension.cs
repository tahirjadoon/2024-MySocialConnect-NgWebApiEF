﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MSC.Core.Constants;

namespace MSC.Core.Extensions;

public static class EnvConfigExtension
{
    #region Items

    public static string GetDefaultConnectionString(this IConfiguration config)
    {
        var connectionString = config.GetConnectionString(ConfigKeyConstants.DefaultConnection);
        return connectionString;
    }

    public static List<string> GetAllowSpecificOrigins(this IConfiguration config)
    {
        var allowSpecificOrigins = config.GetSectionValue<List<string>>(ConfigKeyConstants.AllowSpecificOrigins, null);
        return allowSpecificOrigins;
    }

    public static string GetTokenKey(this IConfiguration config)
    {
        var tokenKey = config.GetSectionValue<string>(ConfigKeyConstants.TokenKey, string.Empty);
        return tokenKey;
    }

    public static string GetLoggingLevelDefault(this IConfiguration config)
    {
        var loggingLevelDefault = config.GetSectionValue<string>(ConfigKeyConstants.LoggingLevelDefault, string.Empty);
        return loggingLevelDefault;
    }

    public static string GetLoggingLevelMsApnetCore(this IConfiguration config)
    {
        var loggingLevelDefault = config.GetSectionValue<string>(ConfigKeyConstants.LoggingLevelMsAspNetCore, string.Empty);
        return loggingLevelDefault;
    }

    #endregion items

    #region GetSectionValue

    public static T GetSectionValue<T>(this IConfiguration config, string sectionName)
    {
        if (!config.GetSection(sectionName).Exists())
        {
            return default(T);
        }
        var sValue = config.GetSection(sectionName).Get<T>();
        return sValue;
    }

    public static T GetSectionValue<T>(this IConfiguration config, string sectionName, T defaultValue)
    {
        if (!config.GetSection(sectionName).Exists())
        {
            return defaultValue;
        }

        var sValue = config.GetSection(sectionName).Get<T>();
        return sValue;
    }

    #endregion GetSectionValue
}
