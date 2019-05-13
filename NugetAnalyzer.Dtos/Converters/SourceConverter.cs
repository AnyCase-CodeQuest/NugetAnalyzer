using System.Collections.Generic;
using System.Linq;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.Dtos.Converters
{
    public static class SourceConverter
    {
        public static ICollection<SourceViewModel> ConvertSourceListToViewModelList(IReadOnlyCollection<Source> sources)
        {
            return sources.Select(ConvertSourceToViewModel).ToList();
        }

        public static SourceViewModel ConvertSourceToViewModel(Source source)
        {
            return source == null
                ? null
                : new SourceViewModel
                {
                    Id = source.Id,
                    Name = source.Name
                };
        }
    }
}
