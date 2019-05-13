﻿namespace NugetAnalyzer.BLL.Models
{
    public abstract class ModelWithReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public PackageVersionComparisonReport Report { get; set; }
    }
}
