using System.Collections.Generic;
using Moq;
using NugetAnalyzer.BLL.Interfaces;
using NugetAnalyzer.BLL.Services;
using NugetAnalyzer.DAL.Interfaces;
using NugetAnalyzer.Domain;
using NUnit.Framework;

namespace NugetAnalyzer.BLL.Test.Services
{
    [TestFixture]
    public class SourceServiceTests
    {
        private ISourceService sourceService;
        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IRepository<Source>> sourceRepositoryMock;
        private Source source;

        [OneTimeSetUp]
        public void Init()
        {
            source = new Source
            {
                Id = 1,
                Name = "GitHub"
            };

            sourceRepositoryMock = new Mock<IRepository<Source>>();
            unitOfWorkMock = new Mock<IUnitOfWork>();

            unitOfWorkMock
                .Setup(p => p.GetRepository<Source>())
                .Returns(sourceRepositoryMock.Object);
            sourceRepositoryMock
                .Setup(p => p.GetAllAsync())
                .ReturnsAsync(new List<Source> { source, source });

            sourceService = new SourceService(unitOfWorkMock.Object);
        }

        [Test]
        public void GetSourceList_Should_Invoke_GetAll()
        {
            sourceService.GetSourceList();
            sourceRepositoryMock.Verify(p => p.GetAllAsync());
        }
    }
}
