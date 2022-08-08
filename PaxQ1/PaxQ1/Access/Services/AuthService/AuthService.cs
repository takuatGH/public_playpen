using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PaxQ1.Authorization.Services.AuthService
{
    public class AuthService : IAuthService
    {
        public IConfiguration configuration { get; set;}

        public AuthService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public JwtSecurityToken createToken(List<Claim> claims)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]));
            return new JwtSecurityToken(issuer: configuration["JWT:ValidIssuer"],
                                        audience: configuration["JWT:ValidAudience"],
                                        expires: DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).AddSeconds(1),
                                        claims: claims,
                                        signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        }

        public string createRefreshToken()
        {
            var seed = new byte[64];
            using var randomNumber = RandomNumberGenerator.Create();
            randomNumber.GetBytes(seed);
            return Convert.ToBase64String(seed);
        }

        public ClaimsPrincipal? getPrincipalFromExpiredToken(string token)
        {
            var validationParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, validationParams, out SecurityToken securityToken);
            return principal;

        }
    }
}
