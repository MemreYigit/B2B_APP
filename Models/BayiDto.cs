namespace EDG_B2B.Models
{
    public class BayiListItemDto
    {
        public required Guid Id { get; set; }
        public required string Unvan { get; set; }
        public required string VergiNo { get; set; }
        public string? Email { get; set; }
        public string? Telefon { get; set; }
    }
}
