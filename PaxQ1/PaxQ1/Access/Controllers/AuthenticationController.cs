using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaxQ1.Authorization.Services.AuthService;
using PaxQ1.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PaxQ1.Authorization.Controllers
{
    [Route("q1/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IAuthService authService;

        public AuthenticationController(
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IAuthService service)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            authService = service;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register newUser)
        {
            //Check if the user is already in the database

            if (await userManager.FindByNameAsync(newUser.Email) == null)
            {
                ApplicationUser user = new()
                {
                    Email = newUser.Email,
                    UserName = newUser.Username,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var result = await userManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                    if(!await roleManager.RoleExistsAsync(newUser.Role))
                    {
                        roleManager.CreateAsync(new IdentityRole(newUser.Role));
                    }
                    var createdRole = await userManager.AddToRolesAsync(user, PaxRoleManager.GetRoles(newUser.Role));
                    if (createdRole is not null)
                    {
                        return Ok(new Response("Success", "Successfully created user"));
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "Internal server error"));
                    }

                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "Unable to create user"));
                }

            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response("Error", "User already exists"));
            }

        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login loginUser)
        {
            var user = await userManager.FindByNameAsync(loginUser.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                var roles = await userManager.GetRolesAsync(user);
                //Generate a claim aaf of the role, will allow service level authorization, 

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                JwtSecurityToken token = authService.createToken(claims);
                string refreshToken = authService.createRefreshToken();

                _ = int.TryParse(authService.configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

                user.refreshToken = refreshToken;
                user.refreshTokenExpiryTime = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).AddDays(refreshTokenValidityInDays);

                await userManager.UpdateAsync(user);
                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    tokenExpiration = token.ValidTo,
                    refreshToken,
                });
            }

            return StatusCode(StatusCodes.Status401Unauthorized, new Response("Error", "Invalid details"));
        }
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(Token token)
        {
            if (token == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response("Error", "Supplied tokens are invalid"));
            }

            string? accessToken = token.accessToken;
            string? refreshToken = token.refreshToken;
            ClaimsPrincipal identity = authService.getPrincipalFromExpiredToken(accessToken);
            var user = await userManager.FindByNameAsync(identity.Identity.Name);
            if (user == null || user.refreshToken != refreshToken || user.refreshTokenExpiryTime <= DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc))
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response("Error", "Invalid tokens!"));
            }

            JwtSecurityToken newToken = authService.createToken(identity.Claims.ToList());
            string newRefreshToken = authService.createRefreshToken();
            user.refreshToken = newRefreshToken;
            await userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newToken),
                refreshToken = newRefreshToken,
            });
        }

    }
}
