using Microsoft.EntityFrameworkCore;
using EDG_B2B.Data;
using EDG_B2B.Entity;
using EDG_B2B.Entity.Enums;
using EDG_B2B.Models;

namespace EDG_B2B.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, Guid UserId)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string Message, KullaniciDto? User, string? Token)> LoginAsync(LoginRequest request);
        Task LogoutAsync(string jti);
        Task<Kullanici?> GetUserByIdAsync(Guid id);
        bool VerifyPassword(string password, string hash);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;
        private readonly IJwtService _jwtService;

        public AuthService(AppDbContext dbContext, IJwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        public async Task<(bool Success, string Message, Guid UserId)> RegisterAsync(RegisterRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            var existingUser = await _dbContext.Kullanicilar.FirstOrDefaultAsync(k => k.Email == normalizedEmail);
            if (existingUser != null)
                return (false, "Bu email zaten kayıtlı", Guid.Empty);

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                return (false, "Şifre en az 8 karakter olmalıdır.", Guid.Empty);

            var existingBayi = await _dbContext.Bayiler.FirstOrDefaultAsync(b => b.VergiNo == request.VergiNo);
            if (existingBayi != null)
                return (false, "Bu vergi numarası ile zaten bir bayi kaydı mevcut", Guid.Empty);

            var kullaniciId = Guid.NewGuid();

            var kullanici = new Kullanici
            {
                Id = kullaniciId,
                Email = normalizedEmail,
                Ad = request.Ad,
                Soyad = request.Soyad,
                Telefon = request.Telefon,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Durum = KullaniciDurumu.Beklemede,
                Aktif = false,
                Rol = KullaniciRolu.Bayi
            };

            var bayi = new Bayi
            {
                Id = Guid.NewGuid(),
                KullaniciId = kullaniciId,
                Unvan = request.Unvan,
                VergiNo = request.VergiNo,
                Adres = request.Adres
            };

            _dbContext.Kullanicilar.Add(kullanici);
            _dbContext.Bayiler.Add(bayi);
            await _dbContext.SaveChangesAsync();

            return (true, "Başvurunuz başarıyla alındı, admin onayı bekliyor", kullanici.Id);
        }

        public async Task<(bool Success, string Message, KullaniciDto? User, string? Token)> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            var kullanici = await _dbContext.Kullanicilar
                .Include(k => k.Bayi)
                .FirstOrDefaultAsync(k => k.Email == normalizedEmail);

            if (kullanici == null)
            {
                BCrypt.Net.BCrypt.Verify("dummy_password", "$2a$11$K3u7DkGj7mPskp.BvN1WCO3G6v3vR6mB5oB1tYy5aK4y5aK4y5aK4");
                return (false, "Email veya şifre yanlış", null, null);
            }

            if (!VerifyPassword(request.Password, kullanici.PasswordHash))
                return (false, "Email veya şifre yanlış", null, null);

            if (kullanici.Durum == KullaniciDurumu.Beklemede)
                return (false, "Hesabınız henüz admin tarafından onaylanmadı", null, null);

            if (kullanici.Durum == KullaniciDurumu.Reddedildi)
                return (false, "Başvurunuz reddedilmiştir. Lütfen destek ile iletişime geçin.", null, null);

            if (!kullanici.Aktif)
                return (false, "Hesabınız pasif durumda. Lütfen destek ile iletişime geçin.", null, null);

            var kullaniciDto = new KullaniciDto
            {
                Id = kullanici.Id,
                Email = kullanici.Email,
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                Durum = kullanici.Durum.ToString(),
                Rol = kullanici.Rol.ToString(),
                Unvan = kullanici.Bayi?.Unvan
            };

            var token = await _jwtService.GenerateTokenAsync(kullanici);

            return (true, "Başarıyla giriş yaptınız", kullaniciDto, token);
        }

        public async Task LogoutAsync(string jti)
        {
            await _jwtService.RevokeAsync(jti);
        }

        public async Task<Kullanici?> GetUserByIdAsync(Guid id)
        {
            return await _dbContext.Kullanicilar.FindAsync(id);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
