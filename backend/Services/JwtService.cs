using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services
{
    public class JwtService(IConfiguration configuration)
    {
        public string GenerateToken(User user, Session session)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    configuration["Jwt:Authentication:Key"]
                        ?? throw new InvalidOperationException("JWT key not found")
                )
            );

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims =
            [
                new("nonce", Guid.CreateVersion7().ToString(), ClaimValueTypes.String),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString(), ClaimValueTypes.String),
                new(JwtRegisteredClaimNames.Jti, session.Id.ToString(), ClaimValueTypes.String),
                new(JwtRegisteredClaimNames.Email, user.Email, ClaimValueTypes.String),
                new(JwtRegisteredClaimNames.Name, user.FirstName, ClaimValueTypes.String),
                new(
                    JwtRegisteredClaimNames.Iat,
                    session.Creation.ToString(),
                    ClaimValueTypes.Integer64
                ),
            ];

            JwtSecurityToken token = new(
                issuer: configuration["Jwt:Authentication:Issuer"],
                audience: configuration["Jwt:Authentication:Audience"],
                claims: claims,
                expires: session.Expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
