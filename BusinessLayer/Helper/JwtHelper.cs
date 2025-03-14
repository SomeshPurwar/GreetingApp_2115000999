using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLayer.Helper
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtHelper> _logger;

        public JwtHelper(IConfiguration configuration, ILogger<JwtHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(string email, int userId)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GeneratePasswordResetToken(string email)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
                var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("email", email),
                    new Claim("isPasswordReset", "true")
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                string tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation($"✅ Generated Password Reset Token: {tokenString}");
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error generating password reset token: {ex.Message}");
                return null;
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
                var tokenHandler = new JwtSecurityTokenHandler();

                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogError("❌ Token is empty or null.");
                    return null;
                }

                if (!tokenHandler.CanReadToken(token))
                {
                    _logger.LogError($"❌ Token format is invalid: {token}");
                    return null;
                }

                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, validationParams, out _);
                _logger.LogInformation("✅ Token successfully validated.");
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("❌ Token expired.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}
