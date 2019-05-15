using System;
using System.Linq.Expressions;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private IProfileService profileService;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IRepository<Profile>> profileRepository;
        private Profile profile;

        [OneTimeSetUp]
        public void Init()
        {
            profile = new Profile
            {
                AccessToken = "AccessToken",
                Id = 1,
                ExternalId = 1,
                SourceId = 1,
                Url = "Url",
                UserId = 1
            };
            profileRepository = new Mock<IRepository<Profile>>();
            unitOfWorkMock = new Mock<IUnitOfWork>();

            profileRepository
                .Setup(p => p.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<Profile, bool>>>()))
                .ReturnsAsync(profile);

            unitOfWorkMock
                .Setup(p => p.GetRepository<Profile>())
                .Returns(profileRepository.Object);

            profileService = new ProfileService(unitOfWorkMock.Object);
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

            profileRepository
                .Verify(p =>
                p.GetSingleOrDefaultAsync(It.Is<Expression<Func<Profile, bool>>>(function => function.Compile().Invoke(argProfile))));
        }
    }
}
