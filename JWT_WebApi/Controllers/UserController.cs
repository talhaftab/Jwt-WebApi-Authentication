using JWT.WebApi.Business;
using JWT.WebApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT.WebApi.Web.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager userManager;

        public UserController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await this.userManager.GetAllUserAsync();
            return Ok(result);
        }
    }
}
