using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.Dtos.Converters
{
    public static class ProfileConverter
    {
        public static ProfileViewModel ConvertProfileToViewModel(Profile profile)
        {
            return profile == null
                ? null
                : new ProfileViewModel
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
