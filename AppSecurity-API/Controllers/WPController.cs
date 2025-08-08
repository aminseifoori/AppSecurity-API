using AppSecurity_API.Dtos;
using AppSecurity_API.Entities;
using AppSecurity_API.JwtFeatures;
using AppSecurity_API.MockData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppSecurity_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WPController : ControllerBase
    {
        private readonly JwtHandler jwtHandler;

        public WPController(JwtHandler _jwtHandler)
        {
            jwtHandler = _jwtHandler;
        }
        [HttpPost("post-login")]
        public async Task<IActionResult> PostLogin([FromBody] LoginDto login)
        {
            if (login.Email == "reza.seafoori@gmail.com" && login.Password == "Rez@1986")
            {
                User user = new User
                {
                    FirstName = "Reza",
                    LastName = "Seifoori",
                    Email = login.Email,
                    UserName = login.Email,
                    Id = Guid.NewGuid().ToString(),

                };
                var token = await jwtHandler.GenerateToken(user);
                return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
            }
            return BadRequest();
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string email, string password)
        {
            if (email == "reza.seafoori@gmail.com" && password == "Rez@1986")
            {
                User user = new User
                {
                    FirstName = "Reza",
                    LastName = "Seifoori",
                    Email = email,
                    UserName = email,
                    Id = Guid.NewGuid().ToString(),

                };
                var token = await jwtHandler.GenerateToken(user);
                return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
            }
            return BadRequest();
        }

        [HttpGet("flight-slist")]
        public IActionResult FlightsList()
        {
            return Ok(FlightList.GetMockFlights());
        }

        [Authorize]
        [HttpGet("secure-flights-list")]
        public IActionResult SecureFlightsList()
        {
            return Ok(FlightList.GetMockFlights());
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteReocrds([FromRoute]string id)
        {
            if (string.IsNullOrWhiteSpace(id) || id == "0")
            {
                return BadRequest("Invalid ID provided.");
            }

            // Perform deletion logic here
            return Ok(new { message = "Record deleted" });
        }

    }
}
