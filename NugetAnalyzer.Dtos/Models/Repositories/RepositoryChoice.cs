namespace NugetAnalyzer.Dtos.Models.Repositories
{
    public class RepositoryChoice
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string DefaultBranch { get; set; }
    }
}
