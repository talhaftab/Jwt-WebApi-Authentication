using JWT.WebApi.Business;
using JWT.WebApi.Model;
using JWT.WebApi.Model.Request;
using JWT.WebApi.Model.Response;
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
        public async Task<ApiResponse<User>?> RegisterAsync(AuthRequest request)
        {
            var apiResponse = new ApiResponse<User>();
            if (request == null)
                return new ApiResponse<User>()
                {
                    Content = null,
                    Message = "Information is empty",
                    Status = System.Net.HttpStatusCode.NotFound
                };
            var user = await userManager.CheckUserAsync(request.EmailAddress);
            if (user is null)
            {
                var response = await userManager.RegisterUserAsync(request);
                return response;
            }
            apiResponse.Message = "User already exist";
            apiResponse.Status = System.Net.HttpStatusCode.OK;
            return apiResponse;
        }

        [HttpPost, Route("login")]
        public async Task<ApiResponse<AuthResponse>> LoginAsync(AuthRequest request)
        {
            var apiResponse = new ApiResponse<AuthResponse>();
            if (request == null)
                return new ApiResponse<AuthResponse>()
                {
                    Content = null,
                    Message = "Information is empty",
                    Status = System.Net.HttpStatusCode.NotFound
                };

            var user = await userManager.CheckUserAsync(request.EmailAddress);
            if (user is not null)
            {
                var isVerified = await authManager.VerifyPasswordAsync(user, request.Password);
                if (!isVerified)
                {
                    apiResponse.Message = "Wrong password";
                    apiResponse.Status = System.Net.HttpStatusCode.BadRequest;
                    return apiResponse;
                }
                var token = authManager.GenerateTokenAsync(user);
                apiResponse.Content = new AuthResponse() { Token = token };
                apiResponse.Message = "Token";
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
            var result = this.authManager.GetClaimName();
            return Ok(result);
        }
    }
}
