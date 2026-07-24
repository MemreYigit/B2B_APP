namespace EDG_B2B.Models
{
    // Auth DTOs
    public class RegisterRequest
    {
        public required string Email { get; set; }
        public required string Ad { get; set; }
        public required string Soyad { get; set; }
        public required string Password { get; set; }
        public required string Unvan { get; set; }
        public required string VergiNo { get; set; }
        public string? Adres { get; set; }
        public string? Telefon { get; set; }
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
        public KullaniciDto? User { get; set; }
        public string? Token { get; set; }
    }

    public class RegisterResponse
    {
        public required bool Success { get; set; }
        public required string Message { get; set; }
        public required Guid UserId { get; set; }
    }

    public class KullaniciDto
    {
        public required Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Ad { get; set; }
        public required string Soyad { get; set; }
        public required string Rol { get; set; }
        public required string Durum { get; set; }
        public string? Unvan { get; set; }
    }

    public class ApprovalRequest
    {
        public string? Notes { get; set; }
    }
}
