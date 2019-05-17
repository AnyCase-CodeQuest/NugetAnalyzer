using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileDTO> GetProfileBySourceIdAsync(int sourceId, int externalId);

        Task UpdateProfileAsync(ProfileDTO profile);

        Task<ProfileDTO> GetProfileForUserAsync(UserRegisterModel user, int sourceId);

        Task<int> GetUserIdByExternalIdAsync(int sourceId, int externalId);
    }
}
