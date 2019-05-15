using System;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.Dtos
{
    public class PackageViewModel
    {
        public int Id { get; set; }
        public DateTime LastUpdateTime { get; set; } = DateTime.Now;
        public string Name { get; set; }
        public PackageVersion Current { get; set; }
        public PackageVersion Latest { get; set; }
        public string Report { get; set; }
    }
}
