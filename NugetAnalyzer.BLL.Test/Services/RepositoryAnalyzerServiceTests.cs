using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class RepositoryAnalyzerServiceTests
    {
        private const string NullRepositoryPath = null;
        private const string EmptyRepositoryPath = "";

        private readonly IList<string> TestPathsList = new List<string>
        {
            @"C:\temp\solution1"
        };

        private readonly string[] TestPathsArray = new string[]
        {
            @"C:\temp\solution1"
        };

        private readonly IList<string> TestNamesList = new List<string>
        {
            "solution1"
        };

        private IRepositoryAnalyzerService repositoryAnalyzerService;

        private Mock<IDirectoryService> directoryService;
        private Mock<IFileService> fileService;

        [SetUp]
        public void Init()
        {
            directoryService = new Mock<IDirectoryService>();
            fileService = new Mock<IFileService>();

            repositoryAnalyzerService = new RepositoryAnalyzerService(directoryService.Object, fileService.Object);
        }

        [Test]
        public void GetParsedRepository_ShouldThrowArgumentNullException_WhenRepositoryPathNull()
        {
            // Arrange
            directoryService.Setup(s => s.IsDirectoryExist(NullRepositoryPath)).Throws(new ArgumentNullException());
            
            // Assert
            Assert.Throws<ArgumentNullException>(() => repositoryAnalyzerService.GetParsedRepository(NullRepositoryPath));
        }

        [Test]
        public void GetParsedRepository_ShouldRepositoryNull_WhenRepositoryPathNotExist()
        {
            // Arrange
            directoryService.Setup(s => s.IsDirectoryExist(EmptyRepositoryPath)).Returns(false);

            // Act
            var repotitory = repositoryAnalyzerService.GetParsedRepository(EmptyRepositoryPath);

            // Assert
            Assert.IsNull(repotitory);
        }

        [Test]
        public void GetParsedRepository_ShouldRepositoryNotNull_WhenFrameworkAppType()
        {
            // Arrange
            directoryService.Setup(s => s.IsDirectoryExist(TestPathsList[0])).Returns(true);
            directoryService.Setup(s => s.GetDirectoryName(TestPathsList[0])).Returns(TestNamesList[0]);
            directoryService.Setup(s => s.GetDirectoriesPaths(TestPathsArray)).Returns(TestNamesList);

            fileService.Setup(s => s.GetFilesPaths(TestPathsList[0], "*.sln")).Returns(TestPathsArray);
            fileService.Setup(s => s.GetFilesPaths(TestPathsList[0], "*.csproj")).Returns(TestPathsArray);

            fileService.Setup(s => s.GetPackagesConfigFilePath(TestPathsList[0])).Returns(TestPathsList[0]);
            fileService.Setup(s => s.GetCsProjFilePath(TestPathsList[0])).Returns(TestPathsList[0]);

            fileService.Setup(s => s.GetFileContent(TestPathsList[0])).Returns(string.Empty);

            // Act
            var repotitory = repositoryAnalyzerService.GetParsedRepository(TestPathsList[0]);

            // Assert
            Assert.IsNotNull(repotitory);
        }
    }
}