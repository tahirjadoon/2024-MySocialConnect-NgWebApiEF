using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MSC.Core.Extensions;

//check /Services/TokenService for the token that is being written
//https://www.jerriepelser.com/blog/useful-claimsprincipal-extension-methods/
public static class ClaimsPrincipalExtensions
{
    public static UserClaimGetDto GetUserClaims(this ClaimsPrincipal principal)
    {
        if(principal == null) return null;
        var claimsDto = new UserClaimGetDto()
        {
            UserName = principal.GetUserName(),
            Id = principal.GetId(),
            Guid = principal.GetGuid(),
            DisplayName = principal.GetDisplayName()
        };
        return claimsDto;
    }

    public static string GetUserName(this ClaimsPrincipal principal)
    {
        if (principal == null) return string.Empty;
        //return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
        return userName;
    }

    public static int GetId(this ClaimsPrincipal principal)
    {
        if (principal == null) return 0;
        //return principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var getId = 0;
        if(!string.IsNullOrWhiteSpace(id)){
            int.TryParse(id, out getId);
        }
        return getId;
    }

    public static Guid GetGuid(this ClaimsPrincipal principal)
    {
        var getGuid = Guid.Empty;
        if (principal == null) return getGuid;
        var guid = principal.FindFirst("Guid")?.Value;
        if (string.IsNullOrWhiteSpace(guid)) return getGuid;
        try
        {
            getGuid = new Guid(guid);
        }
        catch { }
        return getGuid;
    }

    public static string GetDisplayName(this ClaimsPrincipal principal)
    {
        if (principal == null) return string.Empty;
        var displayName = principal.FindFirst("DisplayName")?.Value;
        return displayName;
    }
}
