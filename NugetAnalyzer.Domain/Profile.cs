namespace NugetAnalyzer.Domain
{
    public class Profile : BaseEntity
    {
        public string Url { get; set; }

        public string AccessToken { get; set; }

        public int ExternalId { get; set; }

        public int UserId { get; set; }

        public int SourceId { get; set; }

        public Source Source { get; set; }

        public User User { get; set; }
    }
}
