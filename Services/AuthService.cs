using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entity;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, int UserId)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginRequest request);
        Task<User?> GetUserByIdAsync(int id);
        bool VerifyPassword(string password, string hash);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;

        public AuthService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool Success, string Message, int UserId)> RegisterAsync(RegisterRequest request)
        {   
            var normalizedEmail = request.Email.Trim().ToLower();

            // 1. ASENKRON SORGULAMA: FirstOrDefault yerine FirstOrDefaultAsync kullanıldı
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            if (existingUser != null)
                return (false, "Bu email zaten kayıtlı", 0);

            // B2B Güvenliği için şifre uzunluğu kontrolü (Basit bir önlem)
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                return (false, "Şifre en az 8 karakter olmalıdır.", 0);

            var user = new User
            {
                Email = normalizedEmail, // E-postaları standardize edin
                FullName = request.FullName,
                CompanyName = request.CompanyName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = UserStatus.Pending, 
                Role = UserRole.User
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return (true, "Başvurunuz başarıyla alındı, admin onayı bekliyor", user.Id);
        }

        public async Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            // 1. ASENKRON SORGULAMA
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
    
            // 2. GÜVENLİK (Timing Attack Önlemi): 
            // Kullanıcı yoksa bile dummy (sahte) bir şifre hash'i doğrulayarak zamanlama analizi yapılmasını engelliyoruz.
            if (user == null)
            {
                // Rastgele bir BCrypt doğrulaması çalıştırarak süreyi eşitliyoruz
                BCrypt.Net.BCrypt.Verify("dummy_password", "$2a$11$K3u7DkGj7mPskp.BvN1WCO3G6v3vR6mB5oB1tYy5aK4y5aK4y5aK4");
                return (false, "Email veya şifre yanlış", null);
            }

            // 3. İŞ MANTIĞI: Durum kontrolleri netleştirildi
            if (user.Status == UserStatus.Pending)
                return (false, "Hesabınız henüz admin tarafından onaylanmadı", null);

            if (user.Status == UserStatus.Rejected)
                return (false, "Başvurunuz reddedilmiştir. Lütfen destek ile iletişime geçin.", null);

            // Şifre doğrulaması
            if (!VerifyPassword(request.Password, user.PasswordHash))
                return (false, "Email veya şifre yanlış", null);

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                CompanyName = user.CompanyName,
                Status = user.Status.ToString(),
                Role = user.Role.ToString()
            };

            return (true, "Başarıyla giriş yaptınız", userDto);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}