using JWT.WebApi.Business;
using JWT.WebApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT.WebApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager authManager;
        private readonly IUserManager userManager;

        public AuthController(IAuthManager authManager, IUserManager userManager)
        {
            this.authManager = authManager;
            this.userManager = userManager;
        }
        [HttpPost, Route("register")]
        public async Task<ApiResponse<User>?> RegisterAsync(UserDto request)
        {
            var apiResponse = new ApiResponse<User>();
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
            var hasUser = await userManager.CheckUserAsync(user);
            if (hasUser is null)
            {
                var response = await userManager.RegisterUserAsync(user, request.Password);
                return response;
            }
            else
            {
                apiResponse.Content = null;
                apiResponse.Message = "User already exist";
                apiResponse.Status = System.Net.HttpStatusCode.OK;
            }
            return apiResponse;
        }

        [HttpPost, Route("login")]
        public async Task<ApiResponse<string>> LoginAsync(UserDto request)
        {
            var apiResponse = new ApiResponse<string>();
            if (request == null)
                return new ApiResponse<string>()
                {
                    Content = null,
                    Message = "Information is empty"
                };
            var user = new User()
            {
                UserName = request.UserName,
                EmailAddress = request.EmailAddress
            };

            var hasUser = await userManager.CheckUserAsync(user);
            if (hasUser is not null)
            {
                var isVerified = await authManager.VerifyPasswordAsync(hasUser, request.Password);
                if (!isVerified)
                {
                    apiResponse.Message = "Wrong password";
                    apiResponse.Status = System.Net.HttpStatusCode.BadRequest;
                    return apiResponse;
                }
                var token = authManager.GenerateTokenAsync(hasUser);
                apiResponse.Content = token;
                apiResponse.Message = "User can login with token";
                apiResponse.Status = System.Net.HttpStatusCode.OK;
            }
            else
            {
                apiResponse.Message = "User not found, make sure you're registered.";
                apiResponse.Status = System.Net.HttpStatusCode.BadRequest;
            }
            return apiResponse;
        }

        [HttpGet, Authorize]
        public IActionResult GetClaimName()
        {
            var result = this.userManager.GetClaimName();
            return Ok(result);
        }
    }
}
