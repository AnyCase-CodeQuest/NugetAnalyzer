using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.Common.Interfaces;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture(Category = "UnitTests")]
    public class RepositoryAnalyzerServiceTests
    {
        private const string TestPackagesConfigFileContent =
            @"<?xml version=""1.0"" encoding=""utf-8""?>
                <packages>
                  <package id = ""Antlr"" targetFramework=""net46"" />
                  <package id = ""Autofac"" version=""4.9.1"" targetFramework=""net46"" />
                  <package id = ""Autofac.Mvc5"" version=""4.0.2"" targetFramework=""net46"" />
                  <package id = ""EntityFramework"" version=""6.2.0"" targetFramework=""net46"" />
                </packages>";

        private const string TestCsProjFileContent =
           @"<Project Sdk=""Microsoft.NET.Sdk.Web"">
              <PropertyGroup>
                <TargetFramework>netcoreapp2.2</TargetFramework>
                <AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include = ""DiffPlex"" Version=""1.4.4"" />
                <PackageReference Include = ""Microsoft.AspNetCore.App"" />
                <PackageReference Include = ""Microsoft.AspNetCore.Http.Abstractions"" Version=""2.2.0"" />
                <PackageReference Include = ""Microsoft.AspNetCore.Razor.Design"" Version=""2.2.0"" PrivateAssets=""All"" />
                <PackageReference Include = ""Microsoft.VisualStudio.Web.CodeGeneration.Design"" Version=""2.2.0"" />
                <PackageReference Include = ""NLog.Web.AspNetCore"" Version=""4.8.1"" />
              </ItemGroup>
            </Project> ";

        private const string SolutionSearchPattern = "*.sln";
        private const string CsProjSearchPattern = "*.csproj";
        private const string PackagesConfigSearchPattern = "packages.config";

        private readonly IList<string> TestList = new List<string>
        {
            @"C:\temp\solution"
        };
        private readonly string[] TestArray = new string[]
        {
            @"C:\temp\solution"
        };

        private Mock<IDirectoryService> directoryServiceMock;
        private Mock<IFileService> fileServiceMock;

        private IRepositoryAnalyzerService repositoryAnalyzerService;

        [SetUp]
        public void Init()
        {
            directoryServiceMock = new Mock<IDirectoryService>();
            fileServiceMock = new Mock<IFileService>();

            repositoryAnalyzerService = new RepositoryAnalyzerService(
                directoryServiceMock.Object,
                fileServiceMock.Object);
        }

        [Test]
        public void GetParsedRepositoryAsync_ShouldThrowArgumentNullException_WhenRepositoryPathNull()
        {
            // Arrange
            const string NullRepositoryPath = null;
            directoryServiceMock
                .Setup(directoryService => directoryService.Exists(NullRepositoryPath))
                .Throws(new ArgumentNullException());

            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => repositoryAnalyzerService.GetParsedRepositoryAsync(NullRepositoryPath));
        }

        [Test]
        public async Task GetParsedRepositoryAsync_ShouldRepositoryNull_WhenRepositoryPathNotExist()
        {
            // Arrange
            const string EmptyRepositoryPath = "";
            directoryServiceMock
                .Setup(directoryService => directoryService.Exists(EmptyRepositoryPath))
                .Returns(false);

            // Act
            var repotitory = await repositoryAnalyzerService.GetParsedRepositoryAsync(EmptyRepositoryPath);

            // Assert
            Assert.IsNull(repotitory);
        }

        [Test]
        public async Task GetParsedRepositoryAsync_ShouldRepositoryNotNullAndPackagesCount3_WhenFrameworkAppType()
        {
            // Arrange
            directoryServiceMock
                .Setup(directoryService => directoryService.Exists(TestList[0]))
                .Returns(true);
            directoryServiceMock.Setup(directoryService => directoryService.GetName(TestList[0]))
                .Returns(TestList[0]);
            fileServiceMock
                .Setup(fileService => fileService.GetFilesPaths(TestList[0], SolutionSearchPattern))
                .Returns(TestArray);
            fileServiceMock
                .Setup(fileService => fileService.GetFilesDirectoriesPaths(TestArray))
                .Returns(TestList);
            fileServiceMock
                .Setup(fileService => fileService.GetFilesPaths(TestList[0], CsProjSearchPattern))
                .Returns(TestArray);
            fileServiceMock
                .Setup(fileService => fileService.GetFilePath(TestList[0], PackagesConfigSearchPattern))
                .Returns(TestList[0]);
            fileServiceMock
                .Setup(fileService => fileService.GetContentAsync(TestList[0]))
                .ReturnsAsync(TestPackagesConfigFileContent);

            // Act
            var repotitory = await repositoryAnalyzerService.GetParsedRepositoryAsync(TestList[0]);

            // Assert
            Assert.IsNotNull(repotitory);
            Assert.AreEqual(3, repotitory.Solutions.First().Projects.First().Packages.Count);
        }

        [Test]
        public async Task GetParsedRepositoryAsync_ShouldRepositoryNotNullAndPackagesCount5_WhenCoreAppType()
        {
            // Arrange
            directoryServiceMock
                .Setup(directoryService => directoryService.Exists(TestList[0]))
                .Returns(true);
            directoryServiceMock
                .Setup(directoryService => directoryService.GetName(TestList[0]))
                .Returns(TestList[0]);
            fileServiceMock
                .Setup(fileService => fileService.GetFilesPaths(TestList[0], SolutionSearchPattern))
                .Returns(TestArray);
            fileServiceMock
                .Setup(fileService => fileService.GetFilesDirectoriesPaths(TestArray))
                .Returns(TestList);
            fileServiceMock
                .Setup(fileService => fileService.GetFilesPaths(TestList[0], CsProjSearchPattern))
                .Returns(TestArray);
            fileServiceMock
                .Setup(fileService => fileService.GetFilePath(TestList[0], PackagesConfigSearchPattern))
                .Returns((string)null);
            fileServiceMock
                .Setup(fileService => fileService.GetFilePath(TestList[0], CsProjSearchPattern))
                .Returns(TestList[0]);
            fileServiceMock
                .Setup(fileService => fileService.GetContentAsync(TestList[0]))
                .ReturnsAsync(TestCsProjFileContent);

            // Act
            var repotitory = await repositoryAnalyzerService.GetParsedRepositoryAsync(TestList[0]);

            // Assert
            Assert.IsNotNull(repotitory);
            Assert.AreEqual(5, repotitory.Solutions.First().Projects.First().Packages.Count);
        }
    }
}