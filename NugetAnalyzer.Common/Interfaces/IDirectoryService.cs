namespace NugetAnalyzer.Common.Interfaces
{
    public interface IDirectoryService
    {
        bool Exists(string path);

        string GetName(string directoryPath);

        string GenerateClonePath();

        void Create(string path);

        void Delete(string path);
    }
}