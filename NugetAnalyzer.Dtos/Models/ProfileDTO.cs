namespace NugetAnalyzer.DTOs.Models
{
    public class ProfileDTO
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public string AccessToken { get; set; }

        public int ExternalId { get; set; }

        public int UserId { get; set; }

        public int SourceId { get; set; }
    }
}
