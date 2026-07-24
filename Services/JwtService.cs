using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EDG_B2B.Data;
using EDG_B2B.Entity;

namespace EDG_B2B.Services
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(Kullanici kullanici);
        Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
        Task RevokeAsync(string jti);
        Task RevokeAllForUserAsync(Guid kullaniciId);
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

        public async Task<string> GenerateTokenAsync(Kullanici kullanici)
        {
            // Tek aktif oturum politikası: yeni login, kullanıcının önceki aktif oturumlarını iptal eder.
            await RevokeAllForUserAsync(kullanici.Id);

            var jti = Guid.NewGuid().ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(_expirationMinutes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new Claim(ClaimTypes.Email, kullanici.Email),
                new Claim(ClaimTypes.Name, $"{kullanici.Ad} {kullanici.Soyad}"),
                new Claim(ClaimTypes.Role, kullanici.Rol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            _dbContext.KullaniciOturumlari.Add(new KullaniciOturumu
            {
                Jti = jti,
                KullaniciId = kullanici.Id,
                ExpiresAt = expiresAt
            });
            await _dbContext.SaveChangesAsync();

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
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

                var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                if (string.IsNullOrEmpty(jti))
                {
                    _logger.LogWarning("Token has no jti claim");
                    return null;
                }

                var session = await _dbContext.KullaniciOturumlari
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Jti == jti);

                if (session == null || session.IsRevoked)
                {
                    _logger.LogInformation("Token rejected: session revoked or not found for jti {Jti}", jti);
                    return null;
                }

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
            var session = await _dbContext.KullaniciOturumlari.FirstOrDefaultAsync(s => s.Jti == jti);
            if (session != null && !session.IsRevoked)
            {
                session.IsRevoked = true;
                session.RevokedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RevokeAllForUserAsync(Guid kullaniciId)
        {
            await _dbContext.KullaniciOturumlari
                .Where(s => s.KullaniciId == kullaniciId && !s.IsRevoked)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.IsRevoked, true)
                    .SetProperty(x => x.RevokedAt, DateTime.UtcNow));
        }
    }
}
