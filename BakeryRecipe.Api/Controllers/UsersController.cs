﻿using BakeryRecipe.Application.ClaimTokens;
using BakeryRecipe.Application.System.Users;
using BakeryRecipe.ViewModels.Pagination;
using BakeryRecipe.ViewModels.Response;
using BakeryRecipe.ViewModels.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BakeryRecipe.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public UsersController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var token = await _userService.Login(request);
            if (token == null)
            {
                return Ok(new
                {
                    code = 400,
                    Message = "Username or Password is Incorrect"
                });
            }

            try
            {
                var userPrincipalac = this.ValidateToken(token.AccessToken);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
                    IsPersistent = true,
                    AllowRefresh = true,
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipalac, authProperties);
            }
            catch (Exception)
            {
            }
            return Ok(token);
        }
        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            ClaimsPrincipal principal;
            principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            BaseResponse<string> response = new();
            string key = "Code";
            var cookieValue = Request.Cookies[key];
            if (!cookieValue.Equals(request.Code))
            {
                response.Code = "202";
                response.Message = "Invalid Code";
                return Ok(response);
            }
            RegisterResponseDTO rs = await _userService.Register(request);
            return Ok(rs);
        }
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenResponse refreshToken)
        {
            var rs = await _userService.RefreshToken(refreshToken);

            return Ok(rs);
        }

        [HttpGet("email")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, 1000000);
            string key = "Code";

            var result = await _userService.SendEmail(email, code.ToString());

            if (result)
            {
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddHours(1);
                Response.Cookies.Append(key, code.ToString(), cookieOptions);
                return Ok("Send succesfully");
            }

            return BadRequest("Send failed");

        }

        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _userService.ChangePassword(request.UserID, request.CurrentPassword, request.NewPass);

            return Ok(result);
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            BaseResponse<string> response = new();
            string key = "Code";
            var cookieValue = Request.Cookies[key];
            if (!cookieValue.Equals(request.Code))
            {
                response.Code = "202";
                response.Message = "Invalid Code";
                return Ok(response);
            }

            var result = await _userService.ForgotPassword(request.UserId, request.NewPassword);
            if (result)
            {
                response.Code = "200";
                response.Message = "Change Password Succesfully";
                CookieOptions cookieOptions = new CookieOptions();

                cookieOptions.Expires = DateTime.Now.AddHours(-1);
                Response.Cookies.Append(key, "", cookieOptions);
            }
            else
            {
                response.Code = "202";
                response.Message = "Change Password Unsuccesfully";
            }

            return Ok(response);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request, [FromRoute] Guid id)
        {
            BaseResponse<string> response = new();
            var result = await _userService.UpdateProfile(request,id);
            if (result)
            {
                response.Code = "200";
                response.Message = "Update Profile Succesfully";
            }
            else
            {
                response.Code = "202";
                response.Message = "Update Profile Unsuccesfully";
            }

            return Ok(response);
        }

    }
}
