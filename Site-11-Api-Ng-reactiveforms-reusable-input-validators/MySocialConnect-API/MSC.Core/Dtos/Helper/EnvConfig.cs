using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MSC.Core.Dtos.Helper;

/// <summary>
/// structure is the same as appsettings.json. Add via DI container, check the programs class for details
/// </summary>
public class EnvConfig
{
    public Logging Logging { get; set; } 

    public ConnectionStrings ConnectionStrings { get; set; }
    public string DefaultConnectionString => ConnectionStrings?.DefaultConnection ?? string.Empty;

    public string AllowedHosts { get; set; }
    public List<string> AllowSpecificOrigins { get; set; }
    public string TokenKey { get; set; } 
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; }
}

public class Logging
{
    public LogLevel LogLevel { get; set; }
}

public class LogLevel
{
    public string Default { get; set; }

    [JsonPropertyName("Microsoft.AspNetCore")]
    public string MicrosoftAspNetCore { get; set; }
}
