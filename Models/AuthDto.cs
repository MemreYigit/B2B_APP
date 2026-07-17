namespace WebApplication1.Models
{
    // Auth DTOs
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string Password { get; set; }
        public required string CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponse
    {
        public required bool Success { get; set; }
        public required string Message { get; set; }
        public UserDto? User { get; set; }
        public string? Token { get; set; }
    }

    public class RegisterResponse
    {
        public required bool Success { get; set; }
        public required string Message { get; set; }
        public required int UserId { get; set; }
    }

    public class UserDto
    {
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string CompanyName { get; set; }
        public required string Status { get; set; }
        public required string Role { get; set; }
    }

    public class ApprovalRequest
    {
        public string? Notes { get; set; }
    }
}
