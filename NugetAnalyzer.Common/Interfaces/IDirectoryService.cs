namespace NugetAnalyzer.Common.Interfaces
{
    public interface IDirectoryService
    {
        bool Exists(string path);

        string GetName(string directoryPath);

        void Create(string path);

        void Delete(string path);
    }
}