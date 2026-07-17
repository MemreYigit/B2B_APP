using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entity;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, int UserId)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string Message, UserDto? User, string? Token)> LoginAsync(LoginRequest request);
        Task<User?> GetUserByIdAsync(int id);
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

        public async Task<(bool Success, string Message, int UserId)> RegisterAsync(RegisterRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            if (existingUser != null)
                return (false, "Bu email zaten kayıtlı", 0);

            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
                return (false, "Şifre en az 8 karakter olmalıdır.", 0);

            // Company'yi ara, yoksa oluştur
            var company = await _dbContext.Companies
                .FirstOrDefaultAsync(c => c.TaxNumber == request.TaxNumber && !c.IsDeleted);

            if (company == null)
            {
                company = new Company
                {   
                    Name = request.CompanyName,
                    TaxNumber = request.TaxNumber,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.Companies.Add(company);
                await _dbContext.SaveChangesAsync();
            }

            var user = new User
            {
                Email = normalizedEmail,
                FullName = request.FullName,
                CompanyId = company.Id,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = UserStatus.Pending,
                Role = UserRole.User
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return (true, "Başvurunuz başarıyla alındı, admin onayı bekliyor", user.Id);
        }

        public async Task<(bool Success, string Message, UserDto? User, string? Token)> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            var user = await _dbContext.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

            if (user == null)
            {
                BCrypt.Net.BCrypt.Verify("dummy_password", "$2a$11$K3u7DkGj7mPskp.BvN1WCO3G6v3vR6mB5oB1tYy5aK4y5aK4y5aK4");
                return (false, "Email veya şifre yanlış", null, null);
            }

            if (user.Status == UserStatus.Pending)
                return (false, "Hesabınız henüz admin tarafından onaylanmadı", null, null);

            if (user.Status == UserStatus.Rejected)
                return (false, "Başvurunuz reddedilmiştir. Lütfen destek ile iletişime geçin.", null, null);

            if (!VerifyPassword(request.Password, user.PasswordHash))
                return (false, "Email veya şifre yanlış", null, null);

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                CompanyName = user.Company?.Name ?? "Atanmadı",
                Status = user.Status.ToString(),
                Role = user.Role.ToString()
            };

            var token = _jwtService.GenerateToken(user);

            return (true, "Başarıyla giriş yaptınız", userDto, token);
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