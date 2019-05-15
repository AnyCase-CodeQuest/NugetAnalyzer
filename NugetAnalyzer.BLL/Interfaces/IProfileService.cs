using System.Threading.Tasks;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.BLL.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileViewModel> GetProfileBySourceIdAsync(int sourceId, int externalId);

        Task UpdateProfileAsync(ProfileViewModel profile);
    }
}
