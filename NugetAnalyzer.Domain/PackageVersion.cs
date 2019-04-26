using System;
using System.Collections.Generic;

namespace NugetAnalyzer.Domain
{
    public class PackageVersion
    {
        public int Id { get; set; }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Build { get; set; }

        public int Revision { get; set; }

        public DateTime? PublishedDate { get; set; }

        public int PackageId { get; set; }

        public Package Package { get; set; }

        public ICollection<ProjectPackageVersion> ProjectPackages { get; set; }
    }
}