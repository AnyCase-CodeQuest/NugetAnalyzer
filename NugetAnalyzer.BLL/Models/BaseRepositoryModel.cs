namespace NugetAnalyzer.BLL.Models
{
    public class BaseRepositoryModel
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public BaseRepositoryModel()
        { }

        public BaseRepositoryModel(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
