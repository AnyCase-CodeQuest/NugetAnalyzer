using System;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.Dtos.Converters;
using NugetAnalyzer.Dtos.Models;

namespace NugetAnalyzer.BLL.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepository<Profile> profilesRepository;

        public ProfileService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            profilesRepository = unitOfWork.GetRepository<Profile>();
        }

        public async Task<ProfileViewModel> GetProfileBySourceIdAsync(int sourceId, int idOnSource)
        {
            var profile = await profilesRepository.GetSingleOrDefaultAsync(p => p.SourceId == sourceId && p.IdOnSource == idOnSource);
            return ProfileConverter.ConvertProfileToViewModel(profile);
        }

        public async Task UpdateProfileAsync(ProfileViewModel profile)
        {
            var currentProfile = await profilesRepository.GetByIdAsync(profile.Id);
            currentProfile.AccessToken = profile.AccessToken;
            profilesRepository.Update(currentProfile);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
