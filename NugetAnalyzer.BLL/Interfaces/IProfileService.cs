using System.Threading.Tasks;
using NugetAnalyzer.Domain.Enums;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDTO> GetProfileBySourceIdAsync(SourceType source, int externalId);

        Task UpdateProfileAsync(ProfileDTO profile);

        Task<ProfileDTO> GetProfileForUserAsync(UserRegisterModel user, SourceType source);

        Task<int> GetUserIdByExternalIdAsync(SourceType source, int externalId);
    }
}
