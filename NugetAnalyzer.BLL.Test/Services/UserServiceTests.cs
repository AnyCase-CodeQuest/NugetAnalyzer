using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class UserServiceTests
    {
        private IUserService userService;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IUserRepository> userRepositoryMock;

        [OneTimeSetUp]
        public void Init()
        {
            userRepositoryMock = new Mock<IUserRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>(userRepositoryMock.Object);
            userService = new UserService(unitOfWorkMock.Object);
        }

        [Test]
        public void CreateUserAsync_Should_Invoke_Add_Save()
        {
            userService.CreateUserAsync(new Profile(), "Token");
            userRepositoryMock.Verify(p => p.Add(It.IsAny<User>()));
            unitOfWorkMock.Verify(p => p.SaveChangesAsync());
        }

        [Test]
        public void UpdateGitHubToken_Should_Invoke_Update_Save()
        {

        }
    }
}
