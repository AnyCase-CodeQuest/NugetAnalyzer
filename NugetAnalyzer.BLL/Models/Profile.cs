namespace NugetAnalyzer.BLL.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public string AvatarUrl { get; set; }

        public string GitHubUrl { get; set; }

        public int GitHubId { get; set; }
    }
}   