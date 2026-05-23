using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventPay.API.Services.Auth
{
    public class AuthService : IAuthService
    {
       
          private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string? Login(string username, string password)
        {
            // Step 1: نتحقق من الـ username والـ password
            var adminUsername = _config["Admin:Username"];
            var adminPassword = _config["Admin:Password"];

            if (username != adminUsername || password != adminPassword)
                return null;

            // Step 2: نعمل الـ Token
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            // Step 3: نرجع الـ Token كـ string
            return new JwtSecurityTokenHandler().WriteToken(token);
        } 
    }
}
