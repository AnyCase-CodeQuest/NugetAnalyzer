using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NugetAnalyzer.DTOs.Converters;
using NugetAnalyzer.DTOs.Models;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class ProfileServiceTests
    {
        private IProfileService profileService;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IRepository<Profile>> profileRepositoryMock;
        private Profile profileMock;
        private UserRegisterModel userMock;
        private ProfileConverter profileConverter;

        [OneTimeSetUp]
        public void Init()
        {
            profileConverter = new ProfileConverter();
            profileMock = new Profile
            {
                AccessToken = "AccessToken",
                Id = 1,
                ExternalId = 1,
                SourceId = 1,
                Url = "Url",
                UserId = 1
            };
            userMock = new UserRegisterModel
            {
                ExternalId = 1,
                AccessToken = "AccessToken"
            };
            profileRepositoryMock = new Mock<IRepository<Profile>>();
            unitOfWorkMock = new Mock<IUnitOfWork>();

            profileRepositoryMock
                .Setup(profileRepository => profileRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<Profile, bool>>>()))
                .ReturnsAsync(profileMock);

            profileRepositoryMock
                .Setup(profileRepository => profileRepository.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(profileMock);

            unitOfWorkMock
                .Setup(profileRepository => profileRepository.GetRepository<Profile>())
                .Returns(profileRepositoryMock.Object);
            profileService = new ProfileService(unitOfWorkMock.Object, profileConverter);
        }

        [Test]
        public void GetProfileBySourceIdAsync_Should_Invoke_GetSingleOrDefaultAsync_With_Proper_Expression()
        {
            var sourceId = 3;
            var externalId = 3;
            var argProfile = new Profile
            {
                SourceId = 3,
                ExternalId = 3
            };
            profileService.GetProfileBySourceIdAsync(sourceId, externalId);

            profileRepositoryMock
                .Verify(profileRepository =>
                profileRepository.GetSingleOrDefaultAsync(It.Is<Expression<Func<Profile, bool>>>(function => function.Compile().Invoke(argProfile))));
        }

        [Test]
        public async Task GetProfileForUserAsync_Should_Invoke_UpdateProfileAsync_When_ProfileNotNullAsync()
        {
            await profileService.GetProfileForUserAsync(userMock, 1);
            profileRepositoryMock.Verify(profileRepository => profileRepository.GetByIdAsync(It.IsAny<int>()));
            profileRepositoryMock.Verify(profileRepository => profileRepository.Update(profileMock));
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.SaveChangesAsync());
        }

        [Test]
        public async Task GetUserIdByExternalIdAsync_Should_Invoke_GetSingleOrDefaultAsync()
        {
            await profileService.GetUserIdByExternalIdAsync(1, 1);
            profileRepositoryMock.Verify(profileRepository => profileRepository.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<Profile, bool>>>()));
        }
    }
}
