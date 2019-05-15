namespace NugetAnalyzer.Dtos.Models
{
    public class UserRegisterModel
    {
        public string UserName { get; set; }

        public string AvatarUrl { get; set; }

        public string Url { get; set; }

        public int ExternalId { get; set; }

        public string AccessToken { get; set; }

        public int SourceId { get; set; }

        public string Email { get; set; }
    }
}
