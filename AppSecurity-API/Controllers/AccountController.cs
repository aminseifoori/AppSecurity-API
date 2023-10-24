using AppSecurity_API.Dtos;
using AppSecurity_API.Entities;
using AppSecurity_API.JwtFeatures;
using AutoMapper;
using EmailService.Interface;
using EmailService.Model;
using EmailService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.IdentityModel.Tokens.Jwt;

namespace AppSecurity_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly JwtHandler jwtHandler;
        private readonly IEmailSender emailSender;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> _userManager, IMapper _mapper,
            JwtHandler _jwtHandler, IEmailSender _emailSender, SignInManager<User> _signInManager)
        {
            userManager = _userManager;
            mapper = _mapper;
            jwtHandler = _jwtHandler;
            emailSender = _emailSender;
            signInManager = _signInManager;
        }

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUser)
        {
            if (createUser == null || !ModelState.IsValid)
                return BadRequest();

            var user = mapper.Map<User>(createUser);
            var result = await userManager.CreateAsync(user, createUser.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string?>
                {
                    {"token", token },
                    {"email", user.Email }
                };
            var callback = QueryHelpers.AddQueryString(createUser.ClientURI, param);
            List<EmailAddress> address = new List<EmailAddress>();
            address.Add(new EmailAddress { Address = createUser.Email, DisplayName = createUser.Email });
            var message = new Message(address, "Email Confirmation token", callback, null);
            await emailSender.SendEmailAsync(message);

            await userManager.AddToRoleAsync(user, "User");
            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await userManager.FindByNameAsync(login.Email);
            if (user == null)
                return BadRequest("Invalid Request");
            if (!await userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Email is not confirmed" });
            var test = await userManager.IsLockedOutAsync(user);
            //To active Lockout we need to use PasswordSignInAsync
            //if (!await userManager.CheckPasswordAsync(user, login.Password))
            //    return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });
            var result = await signInManager.PasswordSignInAsync(login.Email, login.Password, false, true);
            if (result.IsLockedOut)
                return Unauthorized(new AuthResponseDto { ErrorMessage = "The account is locked out" });
            if (!result.Succeeded)
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });

            var signingCredentials = jwtHandler.GetSigningCredentials();
            var claims = await jwtHandler.GetClaims(user);
            var tokenOptions = jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            //To reset failed count number after successfully signin
            await userManager.ResetAccessFailedCountAsync(user); 
            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }

        [HttpGet("AdminData")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminData()
        {
            var claims = User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();
            return Ok(claims);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();
                var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                    return BadRequest("Invalid Request");
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var param = new Dictionary<string, string?>
                    {
                        {"token", token },
                        {"email", forgotPasswordDto.Email }
                    };
                var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);
                List<EmailAddress> address = new List<EmailAddress>();
                address.Add(new EmailAddress { Address = forgotPasswordDto.Email, DisplayName = forgotPasswordDto.Email });
                var message = new Message(address, "Reset password token", callback, null);
                await emailSender.SendEmailAsync(message);

                return Ok();
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest(ex.ToString());
            }

        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return BadRequest("Invalid Request");
            var resetPassResult = await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }
            if(await userManager.IsLockedOutAsync(user))
            {
                await userManager.SetLockoutEndDateAsync(user, new DateTime(2000, 1, 1));
            }
            return Ok();
        }

        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string email, [FromQuery] string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Invalid Email Confirmation Request");
            var confirmResult = await userManager.ConfirmEmailAsync(user, token);
            if (!confirmResult.Succeeded)
                return BadRequest("Invalid Email Confirmation Request");
            return Ok();
        }
    }
}
