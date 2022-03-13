using JWT.WebApi.Business;
using JWT.WebApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT.WebApi.Web.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthManager authManager;
        private readonly IUserManager userManager;


        public UserController(IAuthManager authManager, IUserManager userManager)
        {
            this.authManager = authManager;
            this.userManager = userManager;

        }

        [HttpPost, Route("register")]
        public async Task<ApiResponse<User>> RegisterAsync(UserDto request)
        {
            if (request == null)
                return new ApiResponse<User>()
                {
                    Content = null,
                    Message = "Information is empty"
                };
            var user = new User()
            {
                UserName = request.UserName,
                EmailAddress = request.EmailAddress
            };
            var response = await userManager.RegisterUserAsync(user, request.Password);
            return response;
        }

        //[HttpPost, Route("login")]
        //public async Task<ActionResult<string>> LoginAsync(UserDto request)
        //{
        //    if (user.UserName != request.UserName)
        //        return BadRequest("User not found.");
        //    if (!await authManager.VerifyPasswordAsync(user, request.Password))
        //        return BadRequest("Password is not matched.");
        //    return "Here is your token";
        //}

    }
}
