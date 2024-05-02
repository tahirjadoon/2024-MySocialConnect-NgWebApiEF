using System.Text.Json;

namespace MSC.Core.Extensions;

public static class JsonExtensions
{
    /// <summary>
    /// ToJson
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="isCamelCase">Default is true</param>
    /// <returns></returns>
    public static string Serialize<T>(this T data, bool isCamelCase = true)
    {
        var jsonString = "";

        if(data == null) return jsonString;

        if(isCamelCase){
            var jsonOptions = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
            jsonString = JsonSerializer.Serialize<T>(data, jsonOptions);
        }
        else{
            jsonString = JsonSerializer.Serialize<T>(data);
        }

        return jsonString;
    }

    /// <summary>
    /// ToJson Indented
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="isCamelCase"></param>
    /// <returns></returns>
    public static string SerializeIndented<T>(this T data, bool isCamelCase = true)
    {
        var jsonString = "";

        if(data == null) return jsonString;

        var options = new JsonSerializerOptions { WriteIndented = true };
        if(isCamelCase)
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        jsonString = JsonSerializer.Serialize<T>(data);
        return jsonString;
    }

    /// <summary>
    /// From Json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static T Deserialize<T>(this string jsonString)
    {
        if(string.IsNullOrWhiteSpace(jsonString)) return default(T);
        var data = JsonSerializer.Deserialize<T>(jsonString);
        return data;
    }
}
