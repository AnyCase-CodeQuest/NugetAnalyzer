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

        private const string NullRepositoryPath = null;
        private const string EmptyRepositoryPath = "";

        private readonly IList<string> TestList = new List<string>
        {
            @"C:\temp\solution"
        };
        private readonly string[] TestArray = new string[]
        {
            @"C:\temp\solution"
        };

        private Mock<IDirectoryService> directoryService;
        private Mock<IFileService> fileService;

        private IRepositoryAnalyzerService repositoryAnalyzerService;

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
        public void GetParsedRepository_ShouldRepositoryNotNullAndPackagesCount4_WhenFrameworkAppType()
        {
            // Arrange
            directoryService.Setup(s => s.IsDirectoryExist(TestList[0])).Returns(true);
            directoryService.Setup(s => s.GetDirectoryName(TestList[0])).Returns(TestList[0]);
            fileService.Setup(s => s.GetFilesPaths(TestList[0], "*.sln")).Returns(TestArray);
            directoryService.Setup(s => s.GetDirectoriesPaths(TestArray)).Returns(TestList);            
            fileService.Setup(s => s.GetFilesPaths(TestList[0], "*.csproj")).Returns(TestArray);
            fileService.Setup(s => s.GetPackagesConfigFilePath(TestList[0])).Returns(TestList[0]);
            fileService.Setup(s => s.GetFileContent(TestList[0])).Returns(TestPackagesConfigFileContent);

            // Act
            var repotitory = repositoryAnalyzerService.GetParsedRepository(TestList[0]);

            // Assert
            Assert.IsNotNull(repotitory);
            Assert.AreEqual(4, repotitory.Solutions[0].Projects[0].Packages.Count);
        }

        [Test]
        public void GetParsedRepository_ShouldRepositoryNotNullAndPackagesCount6_WhenCoreAppType()
        {
            // Arrange
            directoryService.Setup(s => s.IsDirectoryExist(TestList[0])).Returns(true);
            directoryService.Setup(s => s.GetDirectoryName(TestList[0])).Returns(TestList[0]);
            fileService.Setup(s => s.GetFilesPaths(TestList[0], "*.sln")).Returns(TestArray);
            directoryService.Setup(s => s.GetDirectoriesPaths(TestArray)).Returns(TestList);
            fileService.Setup(s => s.GetFilesPaths(TestList[0], "*.csproj")).Returns(TestArray);
            fileService.Setup(s => s.GetPackagesConfigFilePath(TestList[0])).Returns((string)null);
            fileService.Setup(s => s.GetCsProjFilePath(TestList[0])).Returns(TestList[0]);
            fileService.Setup(s => s.GetFileContent(TestList[0])).Returns(TestCsProjFileContent);

            // Act
            var repotitory = repositoryAnalyzerService.GetParsedRepository(TestList[0]);

            // Assert
            Assert.IsNotNull(repotitory);
            Assert.AreEqual(6, repotitory.Solutions[0].Projects[0].Packages.Count);
        }
    }
}