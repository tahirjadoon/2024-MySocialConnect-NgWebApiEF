using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MSC.Core.DB.Entities;
using MSC.Core.Extensions;

namespace MSC.Core.Services;

public class TokenService : ITokenService
{
    //the key will stay on the server, it will never go to the client
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        var tokenKey = config.GetTokenKey(); //from app settings config using the extension created
        if(string.IsNullOrWhiteSpace(tokenKey))
            throw new Exception("TokenKey missing");

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

    }

    public string CreateToken(AppUser user)
    {
        if(user == null)
            throw new Exception("User info missing");

        //claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim("Guid", user.Guid.ToString())
        };

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
