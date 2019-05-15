namespace NugetAnalyzer.BLL.Models
{
    public class BaseModel
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public BaseModel()
        { }

        public BaseModel(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
