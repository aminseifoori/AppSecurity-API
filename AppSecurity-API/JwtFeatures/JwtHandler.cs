using AppSecurity_API.Dtos;
using AppSecurity_API.Entities;
using AppSecurity_API.Settings;
using Google.Apis.Auth;
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
        private readonly GoogleAuthentication goolgeSettings;
        public JwtHandler(IConfiguration _configuration, UserManager<User> _userManager)
        {
            configuration = _configuration;
            userManager = _userManager;
            serviceSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
            goolgeSettings = _configuration.GetSection(nameof(GoogleAuthentication)).Get<GoogleAuthentication>();
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
        public async Task<string> GenerateToken(User user)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { goolgeSettings.ClientId }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                //log an exception
                return null;
            }
        }
    }
}
