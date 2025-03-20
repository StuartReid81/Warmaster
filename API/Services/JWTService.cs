using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class JWTService(IConfiguration config) : IJWTService
{
    public string CreateToken(AppUser user)
    {
        // grab key from config
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key.");

        //ensure token key length is above minimum for security algorithm
        if(tokenKey.Length < 64) throw new Exception("Token key is too short"); 

        // generate key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        // create claim
        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.UserName)
        };

        //generate credentials
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //create descriptor with claim, expiry date and token signing credentials
        var tokenDescriptor = new SecurityTokenDescriptor{
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = credentials
        };

        //create new tokem handler
        var tokenHandler = new JwtSecurityTokenHandler();

        //generate token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        //retun token as string
        return tokenHandler.WriteToken(token);
    }
}