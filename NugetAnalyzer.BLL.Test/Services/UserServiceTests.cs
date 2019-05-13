using System;
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.Dtos.Models;
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
        private Mock<IRepository<User>> userRepositoryMock;
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

            userRepositoryMock = new Mock<IRepository<User>>();
            unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock
                .Setup(p => p.GetRepository<User>())
                .Returns(userRepositoryMock.Object);
            userRepositoryMock
                .Setup(p => p.GetSingleOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(userMock);

            userService = new UserService(unitOfWorkMock.Object);
        }

        [Test]
        public void CreateUserAsync_Should_Invoke_Add_Save()
        {
            userService.CreateUserAsync(new ProfileViewModel());
            userRepositoryMock.Verify(p => p.Add(It.IsAny<User>()));
            unitOfWorkMock.Verify(p => p.SaveChangesAsync());
        }

        [Test]
        public void UpdateGitHubToken_Should_Invoke_Update_Save()
        {
            var gitHubId = It.IsAny<int>();
            var user = new User { GitHubId = gitHubId };
            var gitHubToken = It.IsAny<string>();
            userService.UpdateUserAsync(new ProfileViewModel());
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
            userService.GetProfileByGitHubIdAsync(user.GitHubId);
            userRepositoryMock
                .Verify(p =>
                p.GetSingleOrDefaultAsync(It.Is<Expression<Func<User, bool>>>(expr => expr.Compile().Invoke(user))));
        }

        [Test]
        public void GetProfileByGitHubId_Should_Return_Profile()
        {
            var result = userService.GetProfileByGitHubIdAsync(5);
            Assert.NotNull(result);
        }

        [Test]
        public void GetProfileByUserName_Should_Invoke_GetSingleOrDefaultAsync()
        {
            var user = new User { UserName = "AntsiferauGodel" };
            userService.GetProfileByUserNameAsync(user.UserName);
            userRepositoryMock
                .Verify(x =>
                x.GetSingleOrDefaultAsync(It.Is<Expression<Func<User, bool>>>(func => func.Compile().Invoke(user))));
        }

        [Test]
        public void GetProfileByUserName_Should_Return_Profile()
        {
            var result = userService.GetProfileByUserNameAsync(It.IsAny<string>());
            Assert.NotNull(result);
        }
    }
}
