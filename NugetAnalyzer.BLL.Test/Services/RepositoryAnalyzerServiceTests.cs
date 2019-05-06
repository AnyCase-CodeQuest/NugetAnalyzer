using System;
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

        private IRepositoryAnalyzerService repositoryAnalyzerService;

        private Mock<IDirectoryService> directoryService;

        [SetUp]
        public void Init()
        {
            directoryService = new Mock<IDirectoryService>();

            repositoryAnalyzerService = new RepositoryAnalyzerService(directoryService.Object);
        }
    }
}