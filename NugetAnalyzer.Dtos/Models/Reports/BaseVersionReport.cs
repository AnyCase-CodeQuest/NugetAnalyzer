namespace NugetAnalyzer.Dtos.Models.Reports
{
    public abstract class BaseVersionReport
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public PackageVersionComparisonReport Report { get; set; }
    }
}
