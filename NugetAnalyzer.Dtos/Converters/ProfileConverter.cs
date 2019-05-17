using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.Dtos.Converters
{
    public class ProfileConverter
    {
        public ProfileDTO ConvertProfileToDTO(Profile profile)
        {
            return profile == null
                ? null
                : new ProfileDTO
                {
                    Id = profile.Id,
                    ExternalId = profile.ExternalId,
                    SourceId = profile.SourceId,
                    AccessToken = profile.AccessToken,
                    Url = profile.Url,
                    UserId = profile.UserId
                };
        }
    }
}
