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
            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
            if (existingUser != null)
                return (false, "Bu email zaten kayıtlı", 0);

            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                CompanyName = request.CompanyName,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Status = UserStatus.Pending, // Başlangıçta Pending
                Role = UserRole.User
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return (true, "Başvurunuz admin onayı bekliyor", user.Id);
        }

        public async Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginRequest request)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return (false, "Email veya şifre yanlış", null);

            // Sadece onaylı kullanıcılar login olabilir
            if (user.Status != UserStatus.Approved)
                return (false, "Hesabınız henüz admin tarafından onaylanmadı", null);

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
