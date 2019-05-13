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
                Id = 5,
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
            userService.CreateUserAsync(new UserRegisterModel());
            userRepositoryMock.Verify(p => p.Add(It.IsAny<User>()));
            unitOfWorkMock.Verify(p => p.SaveChangesAsync());
        }

        [Test]
        public void GetUserByIdAsync_Shoulde_Invoke_GetSingleOrDefault_With_Proper_Expression()
        {
            var userId = 4;
            userService.GetUserByIdAsync(userId);
            userRepositoryMock.Verify(p => p.GetByIdAsync(userId));
        }
    }
}
