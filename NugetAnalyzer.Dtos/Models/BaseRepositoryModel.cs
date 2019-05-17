namespace NugetAnalyzer.DTOs.Models
{
    public class BaseRepositoryModel
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public BaseRepositoryModel()
        { }

        public BaseRepositoryModel(string name, string path)
        {
            Name = name;
            Path = path;
        }
    }
}
