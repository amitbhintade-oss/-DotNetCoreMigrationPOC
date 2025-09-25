using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Employee.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Employee.Services.AuthService
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _tokenExpiryMinutes;

        public JwtTokenService()
        {
            _jwtSecret = ConfigurationManager.AppSettings["JwtSecret"] ?? throw new InvalidOperationException("JwtSecret is missing from configuration");
            _jwtIssuer = ConfigurationManager.AppSettings["JwtIssuer"] ?? "MyEmployeeApi";
            _jwtAudience = ConfigurationManager.AppSettings["JwtAudience"] ?? "MyEmployeeApi";
            
            if (!int.TryParse(ConfigurationManager.AppSettings["TokenExpiryMinutes"], out _tokenExpiryMinutes))
            {
                _tokenExpiryMinutes = 60; // Default to 60 minutes
            }
        }

        public string GenerateToken(EmployeeRequest employee)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var claims = new[]
            {
                new Claim("EmpId", employee.EmpId.ToString()),
                new Claim("Username", employee.Username),
                new Claim(ClaimTypes.Role, employee.Role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenExpiryMinutes),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}