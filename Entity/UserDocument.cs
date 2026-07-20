namespace WebApplication1.Entity
{
    // Kayıt aşamasında yüklenen İmza Sirküleri, Vergi Levhası vb. belgelerin
    // dosya yollarını tutar. Dosyanın kendisi diskte/blob storage'da saklanır.
    public class UserDocument
    {
        public int Id { get; set; }

        public required int UserId { get; set; }

        public UserDocumentType Tip { get; set; }

        public required string FilePath { get; set; }

        public string? OriginalFileName { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public bool IsVerified { get; set; } = false;

        // Navigation property
        public User User { get; set; } = null!;
    }

    public enum UserDocumentType
    {
        ImzaSirkuleri = 0,
        VergiLevhasi = 1,
        FaaliyetBelgesi = 2,
        Diger = 99
    }
}
