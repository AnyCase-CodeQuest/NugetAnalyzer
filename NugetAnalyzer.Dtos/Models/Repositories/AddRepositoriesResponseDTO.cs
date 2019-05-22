using System.Collections.Generic;
using NugetAnalyzer.DTOs.Models.Enums;

namespace NugetAnalyzer.DTOs.Models.Repositories
{
    public class AddRepositoriesResponseDTO
    {
        public ResponseType ResponseType { get; set; }

        public ICollection<string> RepositoriesNames { get; set; }
    }
}
