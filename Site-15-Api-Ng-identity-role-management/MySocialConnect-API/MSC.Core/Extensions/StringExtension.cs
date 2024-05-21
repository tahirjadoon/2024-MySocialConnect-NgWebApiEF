using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MSC.Core.Extensions;

public static class StringExtension
{
    public static string ToTitleCase(this string s)
    {
        if(string.IsNullOrWhiteSpace(s)) return s;

        var newS = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());
        return newS;
    }

    public static IEnumerable<T> StringSplitToType<T>(this string value, string delimiter = ",")
    {
        var defaultValue = default(IEnumerable<T>);

        if(string.IsNullOrWhiteSpace(value))
            return defaultValue;

        var result = value
                        .Split(new[] {delimiter}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToArray();
        if(result.Length <= 0 || (result.Length == 1 && string.IsNullOrWhiteSpace(result[0])))
            return defaultValue;

        var newO = Activator.CreateInstance<List<T>>();
        foreach(var item in result)
        {
            try{
                newO.Add((T)Convert.ChangeType(item, typeof(T)));
            }
            catch{}
        }

        if(!newO.Any())
            return defaultValue;

        return newO;
    }
}
