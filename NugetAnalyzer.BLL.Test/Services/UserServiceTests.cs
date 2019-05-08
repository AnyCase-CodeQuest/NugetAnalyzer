using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Models;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class UserServiceTests
    {
        private IUserService userService;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IUsersRepository> userRepositoryMock;
        private User userMock;

        [OneTimeSetUp]
        public void Init()
        {
            userMock = new User
            {
                AvatarUrl = @"https://avatars1.githubusercontent.com/u/46676069?v=4",
                Email = @"aaaaa@mail.ru",
                GitHubId = 46676069,
                Id = 5,
                GitHubToken = "Token",
                GitHubUrl = @"https://github.com/AntsiferauGodel",
                UserName = "AntsiferauGodel"
            };

            userRepositoryMock = new Mock<IUsersRepository>();
            unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock
                .SetupGet(p => p.UsersRepository)
                .Returns(userRepositoryMock.Object);
            userRepositoryMock
                .Setup(p => p.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(userMock);


            userService = new UserService(unitOfWorkMock.Object);
        }

        [Test]
        public void CreateUserAsync_Should_Invoke_Add_Save()
        {
            userService.CreateUserAsync(new Profile(), It.IsAny<string>());
            userRepositoryMock.Verify(p => p.Add(It.IsAny<User>()));
            unitOfWorkMock.Verify(p => p.SaveChangesAsync());
        }

        [Test]
        public void UpdateGitHubToken_Should_Invoke_Update_Save()
        {
            var gitHubId = It.IsAny<int>();
            var user = new User { GitHubId = gitHubId };
            var gitHubToken = It.IsAny<string>();
            userService.UpdateGitHubToken(gitHubId, gitHubToken);
            userRepositoryMock
                .Verify(p =>
                p.GetSingleOrDefaultAsync(It.Is<Expression<Func<User, bool>>>(expr => expr.Compile().Invoke(user))));
            userRepositoryMock.Verify(p => p.Update(It.IsAny<User>()));
            unitOfWorkMock.Verify(p => p.SaveChangesAsync());
        }

        [Test]
        public void GetProfileByGitHubId_Should_Invoke_GetSingleOrDefaultAsync()
        {
            var user = new User { GitHubId = 5 };
            userService.GetProfileByGitHubId(user.GitHubId);
            userRepositoryMock
                .Verify(p =>
                p.GetSingleOrDefaultAsync(It.Is<Expression<Func<User, bool>>>(expr => expr.Compile().Invoke(user))));
        }

        [Test]
        public void GetProfileByGitHubId_Should_Return_Profile()
        {
            var result = userService.GetProfileByGitHubId(5);
            Assert.NotNull(result);
        }

        [Test]
        public void GetProfileByUserName_Should_Invoke_GetSingleOrDefaultAsync()
        {
            var user = new User { UserName = "AntsiferauGodel" };
            userService.GetProfileByUserName(user.UserName);
            userRepositoryMock
                .Verify(x =>
                x.GetSingleOrDefaultAsync(It.Is<Expression<Func<User, bool>>>(func => func.Compile().Invoke(user))));
        }

        [Test]
        public void GetProfileByUserName_Should_Return_Profile()
        {
            var result = userService.GetProfileByUserName(It.IsAny<string>());
            Assert.NotNull(result);
        }

        [Test]
        public void GetGitHubTokenByGitHubId_Should_Invoke_GetGitHubTokenByGitHubId()
        {
            var gitHubId = It.IsAny<int>();
            userService.GetGitHubTokenByGitHubId(gitHubId);
            userRepositoryMock.Verify(p => p.GetGitHubTokenByGitHubId(gitHubId));
        }
    }
}
