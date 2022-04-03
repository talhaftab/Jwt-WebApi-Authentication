using JWT.WebApi.Business;
using JWT.WebApi.Model;
using JWT.WebApi.Model.Request;
using JWT.WebApi.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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
                var authResponse = await authManager.GenerateTokenAsync(user, HttpContext.Connection.RemoteIpAddress.ToString());
                apiResponse.Content = new AuthResponse()
                {
                    Token = authResponse.Token,
                    RefreshToken = authResponse.RefreshToken
                };
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

        //[HttpPost, Route("refresh-token")]
        //public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        //{
        //    if (!ModelState.IsValid)
        //        return new ApiResponse<AuthResponse>
        //        {
        //            Message = "Token must be provided",
        //            Status = System.Net.HttpStatusCode.BadRequest
        //        };
        //    string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
        //    var token = GetJwtToken(request.ExpiredToken);
        //    var userRefreshToken = await this.userManager.GetFirstOrDefaultRefreshTokenAsync(request, ipAddress);
        //    //var validateResponse = ValidateDetails(token, userRefreshToken);
        //}

        //private object ValidateDetails(JwtSecurityToken token, UserRefreshToken userRefreshToken)
        //{

        //}

        private JwtSecurityToken GetJwtToken(string expiredToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(expiredToken);
        }

        [HttpGet, Authorize]
        public IActionResult GetClaimName()
        {
            var result = this.authManager.GetClaimName();
            return Ok(result);
        }
    }
}
