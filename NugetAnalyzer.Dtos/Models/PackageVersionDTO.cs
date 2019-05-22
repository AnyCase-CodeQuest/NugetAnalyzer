using System;

namespace NugetAnalyzer.DTOs.Models
{
    public class PackageVersionDTO
    {
        public int Id { get; set; }

        public int Major { get; set; }

        public int Minor { get; set; }

        public int Build { get; set; }

        public int Revision { get; set; }

        public DateTime? PublishedDate { get; set; }

        public int PackageId { get; set; }
    }
}