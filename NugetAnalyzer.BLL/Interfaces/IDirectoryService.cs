namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IDirectoryService
    {
        bool IsDirectoryExist(string repositoryPath);

        string GetDirectoryName(string directoryPath);

        string CreateDirectoryForRepository();

        void DeleteRepository(string repositoryPath);
    }
}