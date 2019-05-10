namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IDirectoryService
    {
        bool IsDirectoryExists(string path);

        string GetDirectoryName(string directoryPath);

        void CreateDirectory(string path);

        void DeleteDirectory(string path);
    }
}