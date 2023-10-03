using AppSecurity_API.Dtos;
using AppSecurity_API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppSecurity_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;

        public AccountController(UserManager<User> _userManager, IMapper _mapper)
        {
            userManager = _userManager;
            mapper = _mapper;
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
            return StatusCode(201);
        }
    }
}
