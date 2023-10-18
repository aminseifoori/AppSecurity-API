using AppSecurity_API.Dtos;
using AppSecurity_API.Entities;
using AppSecurity_API.JwtFeatures;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public AccountController(UserManager<User> _userManager, IMapper _mapper,
            JwtHandler _jwtHandler)
        {
            userManager = _userManager;
            mapper = _mapper;
            jwtHandler = _jwtHandler;
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

            await userManager.AddToRoleAsync(user, "User");
            return StatusCode(201);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await userManager.FindByNameAsync(login.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });
            var signingCredentials = jwtHandler.GetSigningCredentials();
            var claims = await jwtHandler.GetClaims(user);
            var tokenOptions = jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
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
    }
}
