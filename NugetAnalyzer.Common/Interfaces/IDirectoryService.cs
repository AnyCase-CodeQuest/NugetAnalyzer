namespace NugetAnalyzer.Common.Interfaces
{
    public interface IDirectoryService
    {
        bool Exists(string path);

        string GetName(string directoryPath);

        string GeneratePath(string directoryName);

        void Create(string path);

        void Delete(string path);
    }
}