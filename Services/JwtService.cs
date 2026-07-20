using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Entity;

namespace WebApplication1.Services
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(User user);
        ClaimsPrincipal? ValidateToken(string token);
        Task RevokeAsync(string jti);
        Task RevokeAllForUserAsync(int userId);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, AppDbContext dbContext, ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
            _secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? "B2BApp";
            _audience = _configuration["Jwt:Audience"] ?? "B2BAppUsers";
            if (!int.TryParse(_configuration["Jwt:ExpirationMinutes"], out _expirationMinutes))
                _expirationMinutes = 60;

            if (_secretKey.Length < 32)
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var jti = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(_expirationMinutes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("CompanyId", user.CompanyId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            _dbContext.UserSessions.Add(new UserSession
            {
                Jti = jti,
                UserId = user.Id,
                ExpiresAt = expiresAt
            });
            await _dbContext.SaveChangesAsync();

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogInformation("Token expired");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return null;
            }
        }

        public async Task RevokeAsync(string jti)
        {
            var session = await _dbContext.UserSessions.FirstOrDefaultAsync(s => s.Jti == jti);
            if (session != null && !session.IsRevoked)
            {
                session.IsRevoked = true;
                session.RevokedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RevokeAllForUserAsync(int userId)
        {
            await _dbContext.UserSessions
                .Where(s => s.UserId == userId && !s.IsRevoked)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.IsRevoked, true)
                    .SetProperty(x => x.RevokedAt, DateTime.UtcNow));
        }
    }
}
