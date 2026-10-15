namespace Test.EFEntities
{
    public class SourceUser
    {
        // Matches your MS SQL table primary key schema

        public int Id { get; set; }
        public int ExternalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
