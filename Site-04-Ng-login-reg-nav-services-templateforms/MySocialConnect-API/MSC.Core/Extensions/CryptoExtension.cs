using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using MSC.Core.Dtos;

namespace MSC.Core.Extensions;

public static class CryptoExtension
{
    /// <summary>
    /// Compute HASh for the passed in value
    /// </summary>
    /// <param name="value"></param>
    /// <returns>HashKeyDto</returns>
    public static HashKeyDto ComputeHashHmacSha512(this string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Value is missing");

        var valueBytes = Encoding.UTF8.GetBytes(value);
        using var hmac = new HMACSHA512();
        var hash = hmac.ComputeHash(valueBytes);
        var salt = hmac.Key;
        var dto = new HashKeyDto(){ Hash = hash, Salt = salt };
        return dto;
    } 

    /// <summary>
    /// Compute HASh for the passed in value using the passed in salt. This is for reverse hash compute
    /// </summary>
    /// <param name="value"></param>
    /// <param name="salt"></param>
    /// <returns></returns>
    public static HashKeyDto ComputeHashHmacSha512(this string value, byte[] salt)
    {
        if(string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Value is missing");
        if(salt == null)
            throw new ValidationException("Key is missing");

        var valueBytes = Encoding.UTF8.GetBytes(value);
        using var hmac = new HMACSHA512(salt);
        var hash = hmac.ComputeHash(valueBytes);
        var saltN = hmac.Key;
        var dto = new HashKeyDto(){ Hash = hash, Salt = saltN };
        return dto;
    }
}
