using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MSC.Core.Constants;
using MSC.Core.DB.Entities;
using MSC.Core.Extensions;

namespace MSC.Core.Services;

public class TokenService : ITokenService
{
    //the key will stay on the server, it will never go to the client
    private readonly SymmetricSecurityKey _key;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IConfiguration config, UserManager<AppUser> userManager)
    {
        var tokenKey = config.GetTokenKey(); //from app settings config using the extension created
        if(string.IsNullOrWhiteSpace(tokenKey))
            throw new Exception("TokenKey missing");

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        _userManager = userManager;
    }

    public async Task<string> CreateToken(AppUser user)
    {
        if(user == null)
            throw new Exception("User info missing");

        //claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim("Guid", user.Guid.ToString()),
            new Claim("DisplayName", user.DisplayName)
        };

        //get roles and add to the claims above with a custom name
        var roles = await _userManager.GetRolesAsync(user);
        if(roles != null)
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        else 
            claims.Add(new Claim(ClaimTypes.Role, SiteIdentityConstants.Role_Member)); //default it to Member when no roles found

        //signing credentials
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512);

        //describe the token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7), //expire after 7 days
            SigningCredentials = credentials
        };

        //token handler
        var tokenHandler = new JwtSecurityTokenHandler();

        //create token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        //write token
        var writeToken = tokenHandler.WriteToken(token);

        //return
        return writeToken;
    }
}
