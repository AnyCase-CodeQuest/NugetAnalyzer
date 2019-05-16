namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IGitService
    {
        void CloneBranch(
            string repositoryUrl,
            string path,
            string userToken,
            string branchName = "master");
    }
}