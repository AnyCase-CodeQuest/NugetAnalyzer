using System;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Converters
{
    internal class UserConverter
    {
        internal static User ConvertProfileToUser(Profile profile)
        {
            return profile == null ? null : new User
            {
                UserName = profile.UserName,
                Email = profile.Email,
                GitHubId = profile.GitHubId,
                GitHubUrl = profile.GitHubUrl,
                AvatarUrl = profile.AvatarUrl
            };
        }

        internal static Profile ConvertUserToProfile(User user)
        {
            return user == null ? null : new Profile
            {
                UserName = user.UserName,
                Email = user.Email,
                GitHubId = user.GitHubId,
                GitHubUrl = user.GitHubUrl,
                AvatarUrl = user.AvatarUrl,
                Id = user.Id
            };
        }
    }
}
