using System;
using System.Threading.Tasks;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Converters;
using NugetAnalyzer.DTOs.Models;

namespace NugetAnalyzer.BLL.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ProfileConverter profileConverter;
        private IRepository<Profile> profilesRepository;

        public ProfileService(IUnitOfWork unitOfWork, ProfileConverter profileConverter)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.profileConverter = profileConverter ?? throw new ArgumentNullException(nameof(profileConverter));
        }

        private IRepository<Profile> ProfileRepository
        {
            get
            {
                if (profilesRepository == null)
                {
                    profilesRepository = unitOfWork.GetRepository<Profile>();
                }

                return profilesRepository;
            }
        }

        public async Task<ProfileDTO> GetProfileForUserAsync(UserRegisterModel user, int sourceId)
        {
            var gitHubId = user.ExternalId;

            var profile = await GetProfileBySourceIdAsync(sourceId, gitHubId);

            if (profile != null)
            {
                profile.AccessToken = user.AccessToken;
                await UpdateProfileAsync(profile);
            }
            return profile;
        }

        public async Task<ProfileDTO> GetProfileBySourceIdAsync(int sourceId, int externalId)
        {
            var profile = await ProfileRepository
                .GetSingleOrDefaultAsync(currentProfile =>
                currentProfile.SourceId == sourceId && currentProfile.ExternalId == externalId);

            return profileConverter.ConvertProfileToDTO(profile);
        }

        public async Task UpdateProfileAsync(ProfileDTO profile)
        {
            var currentProfile = await ProfileRepository.GetByIdAsync(profile.Id);
            currentProfile.AccessToken = profile.AccessToken;
            ProfileRepository.Update(currentProfile);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<int> GetUserIdByExternalIdAsync(int sourceId, int externalId)
        {
            var profile = await ProfileRepository
                .GetSingleOrDefaultAsync(currentProfile =>
                currentProfile.SourceId == sourceId && currentProfile.ExternalId == externalId);

            return profile.UserId;
        }
    }
}
