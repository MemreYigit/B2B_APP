namespace WebApplication1.Models
{
    // Auth DTOs
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public UserDto User { get; set; }
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
    }

    public class ApprovalRequest
    {
        public string Notes { get; set; }
    }
}
