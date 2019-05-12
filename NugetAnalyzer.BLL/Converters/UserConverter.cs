using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Converters
{
    internal class UserConverter
    {
        internal static User ConvertProfileToUser(ProfileViewModel profile)
        {
            return profile == null
                ? null
                : new User
            {
                UserName = profile.UserName,
                Email = profile.Email,
                GitHubId = profile.GitHubId,
                GitHubUrl = profile.GitHubUrl,
                AvatarUrl = profile.AvatarUrl,
                GitHubToken = profile.AccessToken,
                Id = profile.Id
            };
        }

        internal static ProfileViewModel ConvertUserToProfile(User user)
        {
            return user == null
                ? null
                : new ProfileViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    GitHubId = user.GitHubId,
                    GitHubUrl = user.GitHubUrl,
                    AvatarUrl = user.AvatarUrl,
                    Id = user.Id,
                    AccessToken = user.GitHubToken
                };
        }
    }
}
