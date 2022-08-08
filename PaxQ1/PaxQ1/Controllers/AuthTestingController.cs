using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PaxQ1.Controllers
{
    [ApiController]
    [Route("q1/[controller]")]
    public class AuthTestingController : ControllerBase
    {
        [Authorize(Roles = "Owner")]
        [HttpGet("owner")]
        public string GetOwnerMessage()
        {
            return "I am an owner";
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public string GetAdminMessage()
        {
            return "I am an admin or higher";
        }

        [Authorize(Roles = "Security")]
        [HttpGet("security")]
        public string GetSecurityMessage()
        {
            return "I am a security or higher";
        }
    }
}