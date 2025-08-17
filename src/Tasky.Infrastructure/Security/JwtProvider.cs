using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tasky.Application.Abstractions;
using Tasky.Domain.Entities;

namespace Tasky.Infrastructure.Security;

public sealed class JwtProvider(IConfiguration cfg) : IJwtProvider
{
    public string Generate(User user)
    {
        var issuer = cfg["Jwt:Issuer"] ?? "tasky";
        var audience = cfg["Jwt:Audience"] ?? "tasky.web";
        var key = cfg["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
