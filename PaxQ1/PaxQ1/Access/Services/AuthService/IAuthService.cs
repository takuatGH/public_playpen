using PaxQ1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PaxQ1.Authorization.Services.AuthService
{
    public interface IAuthService
    {
        public IConfiguration configuration { get;}
        public JwtSecurityToken createToken(List<Claim> claims);
        public string createRefreshToken();
        public ClaimsPrincipal? getPrincipalFromExpiredToken(string token);
    }
}
