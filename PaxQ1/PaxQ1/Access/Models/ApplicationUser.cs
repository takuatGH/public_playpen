using Microsoft.AspNetCore.Identity;

namespace PaxQ1.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? refreshToken { get; set; }
        public DateTime refreshTokenExpiryTime { get; set; }
    }
}
