using AppSecurity_API.Entities;
using AppSecurity_API.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppSecurity_API.JwtFeatures
{
    public class JwtHandler
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<User> userManager;
        private readonly JwtSettings serviceSettings;
        public JwtHandler(IConfiguration _configuration, UserManager<User> _userManager)
        {
            configuration = _configuration;
            userManager = _userManager;
            serviceSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        }

        public SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(serviceSettings.JwtSecters);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        public async Task<List<Claim>> GetClaims(IdentityUser user)
        {
            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.Email)
            };
            var roles = await userManager.GetRolesAsync((User)user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
        public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: serviceSettings.ValidIssuer,
                audience: serviceSettings.ValidAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(serviceSettings.Expires)),
                signingCredentials: signingCredentials);
            return tokenOptions;
        }
    }
}
