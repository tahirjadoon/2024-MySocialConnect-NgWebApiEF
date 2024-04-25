using System;

namespace MSC.Core.Helper;

public class CloudinaryConfig
{
    public CloudinaryConfig()
    {
    }

    public CloudinaryConfig(string cloudName, string apiKey, string apiSecret, string folder)
    {
        CloudName = cloudName;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
        Folder = folder;
    }

    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
    public string Folder {get; set;}

    //consolidated property
    public string ApiEnvironmentVariable => $"CLOUDINARY_URL=cloudinary://{ApiKey}:{ApiSecret}@{CloudName}";
}
