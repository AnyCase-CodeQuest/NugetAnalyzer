using System;

namespace NugetAnalyzer.Domain
{
    public abstract class PackageBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Build { get; set; }

        public int Revision { get; set; }

        public DateTime PublishedDate { get; set; }
    }
}